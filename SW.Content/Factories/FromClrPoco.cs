using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using SW.Content.Schema;
using SW.Content.Utils;

namespace SW.Content.Factories
{
    public class FromClrPoco : IContentNodeFactory, IContentSchemaNodeFactory
    {
        readonly IContentNodeFactory _memberFactory;
        readonly IContentSchemaNodeFactory _schemaFactory;

        public FromClrPoco(IContentNodeFactory memberFactory, IContentSchemaNodeFactory schemaFactory)
        {
            _memberFactory = memberFactory;
            _schemaFactory = schemaFactory;
        }

        public IContentNode CreateFrom(object obj)
        {
            var t = obj.GetType();
            var keyValuePairs = t
                .GetProperties(BindingFlags.Instance | BindingFlags.Public)
                .Select(p => new KeyValuePair<string, object>(p.Name, p.GetValue(obj)));

            return new ContentObject(keyValuePairs, obj, _memberFactory);
        }

        public ITypeDef CreateSchemaNodeFrom(Type type)
        {
            if (type == typeof(object)) return new TypeDef<IContentNode>();
            
            var props = type.GetProperties(BindingFlags.Instance | BindingFlags.Public)
                .Select(p => new
                {
                    p.Name,
                    Type = _schemaFactory.CreateSchemaNodeFrom(p.PropertyType)
                });

            var typeDef = new TypeDef<ContentObject>();
            foreach (var prop in props)
            {
                typeDef = typeDef.WithProperty(prop.Name, false, prop.Type);
            }

            return typeDef;
        }
    }
}
