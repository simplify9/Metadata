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

        public ITypeDef CreateSchemaNodeFrom(Type type)
        {
            if (!(type == typeof(bool) || type == typeof(bool?))) return null;
            var schema = new TypeDef<ContentBoolean>();
            return (type == typeof(bool))
                ? (ITypeDef)schema
                : new MustBeOneOf(new ITypeDef[]
                {
                    schema,
                    new TypeDef<ContentNull>()
                });
             
        }
    }
}
