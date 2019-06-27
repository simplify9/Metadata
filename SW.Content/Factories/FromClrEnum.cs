using System;
using System.Collections.Generic;
using System.Text;
using SW.Content.Schema;
using SW.Content.Utils;

namespace SW.Content.Factories
{
    public class FromClrEnum : IContentFactory, IContentSchemaNodeFactory
    {
        string GetEntityName(Type t)
        {
            return t.FullName;
        }

        public IContentNode CreateFrom(object from)
        {
            var t = from.GetType();
            t = t.IsNullableType() ? Nullable.GetUnderlyingType(t) : t;

            if (!typeof(Enum).IsAssignableFrom(t)) return null;

            return new ContentEntity(GetEntityName(t), from.ToString());
        }

        public IMust CreateSchemaNodeFrom(Type t)
        {
            var nullable = t.IsNullableType();
            t = nullable ? Nullable.GetUnderlyingType(t) : t;

            var schema = new MustBeEntityWithName(GetEntityName(t), new ContentSchemaRule[] { });
            return !nullable
                ? (IMust)schema
                : new MustBeOneOf(new IMust[]
                {
                    schema,
                    new MustBeNull()
                });
        }
    }
}
