using SW.Content.Utils;
using System;
using System.Collections.Generic;
using System.Text;

namespace SW.Content.Factories
{
    public class FromClrNull : IContentNodeFactory
    {
        
        public IContentNode CreateFrom(object obj)
        {
            if (obj != null) return null;
            return ContentNull.Singleton;
        }
    }
}
