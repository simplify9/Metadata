using System;
using System.Collections.Generic;
using System.Text;

namespace SW.Metadata.Core.Mapping
{
    class FromDocumentValueFactory : IDocumentValueFactory
    {
        public IDocumentValue CreateFrom(object obj)
        {
            if (!(obj is IDocumentValue value)) return null;
            return value;
        }
    }
}
