using System;
using System.Collections.Generic;
using System.Text;

namespace SW.Content.Factories
{
    public class FromClrString : IContentFactory
    {
        public IContentNode CreateFrom(object obj)
        {
            if (!(obj is string)) return null;
            return new ContentText((string)obj);
        }
    }
}
