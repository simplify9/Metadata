using System;
using System.Collections.Generic;
using System.Text;
using SW.Content.Schema;
using SW.Content.Utils;

namespace SW.Content.Factories
{
    public class FromClrEnum : IContentNodeFactory, IContentSchemaNodeFactory
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

        public ITypeDef CreateSchemaNodeFrom(Type t)
        {
            var nullable = t.IsNullableType();
            t = nullable ? Nullable.GetUnderlyingType(t) : t;

            var schema = new TypeDef<ContentEntity>()
                .WithConstraint("_entity_type", new EntityTypeConstraint(GetEntityName(t)));
            return !nullable
                ? (ITypeDef)schema
                : new MustBeOneOf(new ITypeDef[]
                {
                    schema,
                    new TypeDef<ContentNull>()
                });
        }
    }
}
