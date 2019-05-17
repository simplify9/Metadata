using System;
using System.Collections.Generic;
using System.Text;

namespace SW.Content.Factories
{
    class FromContentNode : IContentFactory
    {
        public IContentNode CreateFrom(object obj)
        {
            if (!(obj is IContentNode value)) return null;
            return value;
        }
    }
}
