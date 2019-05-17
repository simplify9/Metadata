using System;
using System.Collections.Generic;
using System.Text;

namespace SW.Metadata.Core.Mapping
{
    public class FromClrDateTimeProvider : IContentReader
    {
        

        public IContentNode CreateFrom(ContentReaderMap r, object obj)
        {
            if (!(obj is DateTime || obj is DateTime?)) return null;
            return new ContentDateTime((DateTime)obj);
        }
    }
}
