using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SW.Describe.Models
{
    
    public class EBool : EPrimitive<bool>, IBool
    {
        
        public EBool(bool value) : base(value)
        {
            
        }
        
    }
}
