using System;
using System.Collections.Generic;
using System.Text;

namespace SW.Metadata.Core.Mapping
{
    class FromContentNodeProvider : IContentReader
    {
        public IContentNode CreateFrom(ContentReaderMap r, object obj)
        {
            if (!(obj is IContentNode value)) return null;
            return value;
        }
    }
}
