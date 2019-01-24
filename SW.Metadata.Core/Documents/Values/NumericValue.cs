using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SW.Metadata.Core
{
    public class NumericValue : DocumentValueBase<decimal>
    {
        
        public NumericValue(decimal value) : base(value)
        {
            
        }
        
        public override string CreateMatchKey()
        {
            return _value.ToString("n6");
        }
        
    }
}
