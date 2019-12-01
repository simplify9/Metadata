using System;
using System.Collections.Generic;
using System.Text;

namespace SW.Describe.Models
{
    public interface IContentFilter
    {
        ContentFilterType Type { get; }

        bool IsMatch(IPayload document);
    }
}
