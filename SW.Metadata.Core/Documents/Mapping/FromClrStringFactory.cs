using System;
using System.Collections.Generic;
using System.Text;

namespace SW.Metadata.Core.Mapping
{
    public class FromClrStringFactory : IDocumentValueFactory
    {
        public IDocumentValue CreateFrom(object obj)
        {
            if (!(obj is string)) return null;
            return new TextValue((string)obj);
        }
    }
}
