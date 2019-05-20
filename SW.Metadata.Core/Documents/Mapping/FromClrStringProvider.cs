using System;
using System.Collections.Generic;
using System.Text;

namespace SW.Metadata.Core.Mapping
{
    public class FromClrStringProvider : IContentReader
    {
        public IContentNode CreateFrom(ContentReaderMap r, object obj)
        {
            if (!(obj is string)) return null;
            return new ContentText((string)obj);
        }
    }
}
