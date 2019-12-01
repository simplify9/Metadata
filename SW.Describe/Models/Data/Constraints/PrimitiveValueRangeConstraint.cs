using System;
using System.Collections.Generic;
using System.Text;

namespace SW.Describe.Models
{
    
    public class PrimitiveValueRangeConstraint : ITypeConstraintSpecs
    {
        PrimitiveValueRangeConstraint()
        {

        }

        public object Min { get; private set; }

        public object Max { get; private set; }

        public PrimitiveValueRangeConstraint(object min, object max)
        {
            Min = min;
            Max = max;
        }

        public string GetConstraintName() => Constants.Constraints.ValueRange;
    }
}
