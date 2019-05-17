using System;
using System.Collections.Generic;
using System.Text;

namespace SW.Metadata.Core.Mapping
{
    public class FromClrBooleanProvider : IContentReader
    {
        
        public IContentNode CreateFrom(ContentReaderMap r, object obj)
        {
            if (!(obj is bool || obj is bool?)) return null;
            return new ContentBoolean((bool)obj);
        }
    }
}
