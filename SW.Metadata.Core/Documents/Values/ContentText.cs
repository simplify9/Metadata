using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SW.Metadata.Core
{
    public class ContentText : ContentPrimitive<string>
    {
        
        public ContentText(string value) : base(value)
        {
            
        }

        public override string ToString()
        {
            return $"\"{_value}\"";
        }
    }
}
