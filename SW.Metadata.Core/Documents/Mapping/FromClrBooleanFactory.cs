using System;
using System.Collections.Generic;
using System.Text;

namespace SW.Metadata.Core.Mapping
{
    public class FromClrBooleanFactory : IDocumentValueFactory
    {
        
        public IDocumentValue CreateFrom(object obj)
        {
            if (!(obj is bool || obj is bool?)) return null;
            return new BooleanValue((bool)obj);
        }
    }
}
