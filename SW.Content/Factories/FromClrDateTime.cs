using SW.Content.Schema;
using SW.Content.Utils;
using System;
using System.Collections.Generic;
using System.Text;

namespace SW.Content.Factories
{
    public class FromClrDateTime : IContentNodeFactory, IContentSchemaNodeFactory
    {
        
        public IContentNode CreateFrom(object obj)
        {
            if (!(obj is DateTime || obj is DateTime?)) return null;
            return new  ContentDateTime((DateTime)obj);
        }

        public ITypeDef CreateSchemaNodeFrom(Type type)
        {
            if (!(type == typeof(DateTime) || type == typeof(DateTime?))) return null;
            var schema = new TypeDef<ContentDateTime>();
            return (type == typeof(DateTime))
                ? (ITypeDef)schema
                : new MustBeOneOf(new ITypeDef[]
                {
                    schema,
                    new TypeDef<ContentNull>()
                });

        }
    }
}
