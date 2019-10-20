using SW.Content.Factories;
using SW.Content.Schema;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SW.Content
{
    public class ContentSchemaNodeFactory
    {
        class DefaultFactory : IContentSchemaNodeFactory
        {
            
            readonly IContentSchemaNodeFactory[] _schemaFactories;

            public DefaultFactory()
            {
                

                _schemaFactories = new IContentSchemaNodeFactory[]
                    {
                        // ORDER IS IMPORTANT !!
                        new FromContentNode(),
                        new FromJToken(ContentFactory.Default),
                        new FromClrString(),
                        new FromClrBoolean(),
                        new FromClrDateTime(),
                        new FromClrNumberType(),
                        new FromClrDictionary(ContentFactory.Default, this),
                        new FromClrIEnumerable(ContentFactory.Default, this),
                        new FromClrPoco(ContentFactory.Default, this)
                    };
            }
            
            public ITypeDef CreateSchemaNodeFrom(Type type)
            {
                if (type == null) throw new ArgumentNullException(nameof(type));

                var schema = _schemaFactories
                    .Select(f_ => f_.CreateSchemaNodeFrom(type))
                    .FirstOrDefault(v => v != null);

                if (schema != null) return schema;

                throw new NotSupportedException($"Cannot create a content schema node from type '{type.AssemblyQualifiedName}'");
            }
        }

        public static IContentSchemaNodeFactory Default { get; } = new DefaultFactory();
    }
}
