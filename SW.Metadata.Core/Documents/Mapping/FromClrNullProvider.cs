using System;
using System.Collections.Generic;
using System.Text;

namespace SW.Metadata.Core.Mapping
{
    public class FromClrNullProvider : IContentReader
    {
        public IContentNode CreateFrom(ContentReaderMap r, object obj)
        {
            if (obj != null) return null;
            return new ContentNull();
        }
    }
}
