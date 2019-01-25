using System;
using System.Collections.Generic;
using System.Text;

namespace SW.Metadata.Core
{
    public interface IDocumentFilter
    {
        DocumentFilterType Type { get; }

        bool IsMatch(DocumentContentReader document);
    }
}
