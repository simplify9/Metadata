using System;
using System.Collections.Generic;
using System.Text;

namespace SW.Content.Factories
{
    public class FromClrNull : IContentFactory
    {
        public IContentNode CreateFrom(object obj)
        {
            if (obj != null) return null;
            return new ContentNull();
        }
    }
}
