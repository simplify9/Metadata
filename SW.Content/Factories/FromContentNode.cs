using System;
using System.Collections.Generic;
using System.Text;
using SW.Content.Schema;

namespace SW.Content.Factories
{
    class FromContentNode : IContentFactory, IContentSchemaNodeFactory
    {
        public IContentNode CreateFrom(object obj)
        {
            if (!(obj is IContentNode value)) return null;
            return value;
        }

        public IMust CreateSchemaNodeFrom(Type type)
        {
            if (type == typeof(IContentNode)) return new CanBeAnything();
            if (typeof(IContentNode).IsAssignableFrom(type))
            {

                var schemaType = typeof(MustHaveType<>).MakeGenericType(type);
                return Activator.CreateInstance(schemaType, new ContentSchemaRule[] { } as object)
                    as IMust;
            }
            return null;
        }
    }
}
