using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SW.Describe.Models
{
    
    public class ListItemCountConstraint : ITypeConstraintSpecs
    {
        ListItemCountConstraint()
        {

        }

        public int? MinItemCount { get; private set; }

        public int? MaxItemCount { get; private set; }

        public ListItemCountConstraint(int? minItems, int? maxItems)
        {
            MinItemCount = minItems;
            MaxItemCount = maxItems;
        }

        public string GetConstraintName() => Constants.Constraints.ListItemCount;
    }
}
