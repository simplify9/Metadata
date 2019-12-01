using System;
using System.Collections.Generic;
using System.Text;

namespace SW.Describe.Models
{
    public class ENumber<T> : EPrimitive<T>, INumber where T : struct, IComparable
    {
        
        public ENumber(T value) : base(value)
        {
            
        }
    }
}
