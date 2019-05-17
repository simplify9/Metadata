using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SW.Content
{
    public class ContentNumber : ContentPrimitive<decimal>
    {
        
        public ContentNumber(decimal value) : base(value)
        {
            
        }
        
        public override string CreateMatchKey()
        {
            return _value.ToString("n6");
        }
        
    }
}
