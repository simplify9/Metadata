using System;
using System.Collections.Generic;
using System.Text;

namespace SW.Content.Factories
{
    public class FromClrBoolean : IContentFactory
    {
        
        public IContentNode CreateFrom(object obj)
        {
            if (!(obj is bool || obj is bool?)) return null;
            return new ContentBoolean((bool)obj);
        }
    }
}
