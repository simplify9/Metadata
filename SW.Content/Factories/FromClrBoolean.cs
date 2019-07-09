using System;
using System.Collections.Generic;
using System.Text;
using SW.Content.Schema;
using SW.Content.Utils;

namespace SW.Content.Factories
{
    public class FromClrBoolean : IContentNodeFactory, IContentSchemaNodeFactory
    {
     
        

        public IContentNode CreateFrom(object obj)
        {
            if (!(obj is bool || obj is bool?)) return null;
            return new ContentBoolean((bool)obj);
        }

        public IMust CreateSchemaNodeFrom(Type type)
        {
            if (!(type == typeof(bool) || type == typeof(bool?))) return null;
            var schema = new MustHaveType<ContentBoolean>(new ContentSchemaRule[] { });
            return (type == typeof(bool))
                ? (IMust)schema
                : new MustBeOneOf(new IMust[]
                {
                    schema,
                    new MustBeNull()
                });
             
        }
    }
}
