using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SW.Metadata.Core
{
    public class ContentDateTime : ContentPrimitive<DateTime>
    {
        public const string Regex = @"\d\d\d\d-(0?[1-9]|1[0-2])-(0?[1-9]|[12][0-9]|3[01])(T(00|[0-9]|1[0-9]|2[0-3]):([0-9]|[0-5][0-9]):([0-9]|[0-5][0-9])(Z|(\+\d\d:\d\d)))?";

        public ContentDateTime(DateTime value) : base(value)
        {
            
        }
        
        public override string CreateMatchKey()
        {
            return _value.ToString("o");
        }
    }
}
