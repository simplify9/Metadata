using System;
using System.Collections.Generic;
using System.Text;

namespace SW.Content
{
    public interface IContentFactory
    {
        IContentNode CreateFrom(object from);
    }
}
