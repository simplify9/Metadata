using System;
using System.Collections.Generic;
using System.Text;

namespace SW.Metadata.Core.Mapping
{
    public class FromClrNullFactory : IDocumentValueFactory
    {
        public IDocumentValue CreateFrom(object obj)
        {
            if (obj != null) return null;
            return new NullValue();
        }
    }
}
