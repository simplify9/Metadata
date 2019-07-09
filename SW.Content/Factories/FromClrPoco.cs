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

        public IMust CreateSchemaNodeFrom(Type type)
        {
            if (type == typeof(object)) return new CanBeAnything();

            var props = type.GetProperties(BindingFlags.Instance | BindingFlags.Public)
                .Select(p => new ContentProperty(p.Name, 
                    _schemaFactory.CreateSchemaNodeFrom(p.PropertyType),
                    false));

            return new MustBeObject(props, new ContentSchemaRule[] { });
        }
    }
}
