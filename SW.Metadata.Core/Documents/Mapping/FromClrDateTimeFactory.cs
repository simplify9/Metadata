using System;
using System.Collections.Generic;
using System.Text;

namespace SW.Metadata.Core.Mapping
{
    public class FromClrDateTimeFactory : IDocumentValueFactory
    {
        

        public IDocumentValue CreateFrom(object obj)
        {
            if (!(obj is DateTime || obj is DateTime?)) return null;
            return new DateTimeValue((DateTime)obj);
        }
    }
}
