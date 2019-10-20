using System;
using System.Collections.Generic;
using System.Text;
using SW.Content.Schema;
using SW.Content.Utils;

namespace SW.Content.Factories
{
    class FromContentNode : IContentNodeFactory, IContentSchemaNodeFactory
    {
        public IContentNode CreateFrom(object obj)
        {
            if (!(obj is IContentNode value)) return null;
            return value;
        }

        public ITypeDef CreateSchemaNodeFrom(Type type)
        {
            if (type == typeof(IContentNode)) return new TypeDef<IContentNode>();
            if (typeof(IContentNode).IsAssignableFrom(type))
            {

                var schemaType = typeof(TypeDef<>).MakeGenericType(type);
                return Activator.CreateInstance(schemaType)
                    as ITypeDef;
            }
            return null;
        }
    }
}
