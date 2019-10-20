using System;
using System.Collections.Generic;
using System.Text;

namespace SW.Content.Filters
{
    public interface IContentFilter
    {
        //ContentFilterType Type { get; }

        bool IsMatch(IContentNode document);
    }
}
