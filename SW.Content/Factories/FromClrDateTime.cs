using SW.Content.Schema;
using System;
using System.Collections.Generic;
using System.Text;

namespace SW.Content.Factories
{
    public class FromClrDateTime : IContentFactory, IContentSchemaNodeFactory
    {
        
        public IContentNode CreateFrom(object obj)
        {
            if (!(obj is DateTime || obj is DateTime?)) return null;
            return new  ContentDateTime((DateTime)obj);
        }

        public IMust CreateSchemaNodeFrom(Type type)
        {
            if (!(type == typeof(DateTime) || type == typeof(DateTime?))) return null;
            var schema = new MustBeDateTime(true, true, new ContentSchemaRule[] { });
            return (type == typeof(DateTime))
                ? (IMust)schema
                : new MustBeOneOf(new IMust[]
                {
                    schema,
                    new MustBeNull()
                });

        }
    }
}
