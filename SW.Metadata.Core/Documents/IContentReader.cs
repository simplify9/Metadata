using System;
using System.Collections.Generic;
using System.Text;

namespace SW.Metadata.Core
{
    public interface IContentReader
    {
        IContentNode CreateFrom(ContentReaderMap registry, object obj);
    }
}
