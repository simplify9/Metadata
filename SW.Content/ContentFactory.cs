using SW.Content.Factories;
using SW.Content.Schema;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SW.Content
{
    public static class ContentFactory
    {
        class DefaultFactory : IContentFactory
        {
            readonly IContentFactory[] _factories;
            
            public DefaultFactory()
            {
                _factories = new IContentFactory[]
                    {
                        // ORDER IS IMPORTANT !!
                        new FromClrNull(),
                        new FromContentNode(),
                        new FromJToken(this),
                        new FromClrString(),
                        new FromClrBoolean(),
                        new FromClrDateTime(),
                        new FromClrNumberType(),
                        new FromClrDictionary(this, ContentSchemaNodeFactory.Default),
                        new FromClrEnumerable(this, ContentSchemaNodeFactory.Default),
                        new FromClrPoco(this, ContentSchemaNodeFactory.Default)
                    };
                
            }

            public IContentNode CreateFrom(object from)
            {
                var value = _factories
                    .Select(f_ => f_.CreateFrom(from))
                    .FirstOrDefault(v => v != null);

                if (value != null) return value;

                throw new NotSupportedException($"Cannot create a content node from type '{from?.GetType().AssemblyQualifiedName}'");
            }

            
        }

        public static IContentFactory Default { get; } = new DefaultFactory();
    }
}
