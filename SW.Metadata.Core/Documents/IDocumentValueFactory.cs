using System;
using System.Collections.Generic;
using System.Text;

namespace SW.Metadata.Core
{
    public interface IDocumentValueFactory
    {
        IDocumentValue CreateFrom(object obj);
    }
}
