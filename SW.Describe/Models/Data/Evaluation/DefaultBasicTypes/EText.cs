using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SW.Describe.Models
{
    
    public class EText : EPrimitive<string>, IText
    {
        
        public EText(string value) : base(value)
        {
            
        }
        
    }
}
