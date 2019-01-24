using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SW.Metadata.Core
{
    public class TextValue : DocumentValueBase<string>
    {
        
        public TextValue(string value) : base(value)
        {
            
        }

        public override string ToString()
        {
            return $"\"{_value}\"";
        }
    }
}
