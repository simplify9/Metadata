using SW.Metadata.Core.Mapping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SW.Metadata.Core
{
    public class ContentReaderMap
    {
        readonly IContentReader[] _providers =
        {
            // ORDER IS IMPORTANT !!
            new FromClrNullProvider(),
            new FromContentNodeProvider(),
            new FromJTokenProvider(),
            new FromClrStringProvider(),
            new FromClrBooleanProvider(),
            new FromClrDateTimeProvider(),
            new FromClrNumberTypeProvider(),
            new FromClrDictionaryProvider(),
            new FromClrEnumerableProvider(),
            new FromClrPocoProvider()
        };

        public ContentReaderMap()
        {

        }

        public virtual IContentNode CreateValue(object from)
        {
            var value = _providers
                .Select(f_ => f_.CreateFrom(this, from))
                .Where(v => v != null)
                .FirstOrDefault();
            if (value != null) return value;
            throw new NotSupportedException($"Cannot create a document value from type '{from?.GetType().AssemblyQualifiedName}'");
        }

    }
}
