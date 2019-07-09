using SW.Content.Utils;
using System;
using System.Collections.Generic;
using System.Text;

namespace SW.Content
{
    public interface IContentNodeFactory
    {
        IContentNode CreateFrom(object from);
    }
}
