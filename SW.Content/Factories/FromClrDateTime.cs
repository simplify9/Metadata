using System;
using System.Collections.Generic;
using System.Text;

namespace SW.Content.Factories
{
    public class FromClrDateTime : IContentFactory
    {
        

        public IContentNode CreateFrom(object obj)
        {
            if (!(obj is DateTime || obj is DateTime?)) return null;
            return new ContentDateTime((DateTime)obj);
        }
    }
}
