using System;
using System.Collections.Generic;
using System.Text;

namespace SW.Describe.Models
{
    
    public class ListItemTypeConstraint : ITypeConstraintSpecs
    {
        ListItemTypeConstraint()
        {

        }

        public TypeDesc Item { get; private set; }

        public ListItemTypeConstraint(TypeDesc item)
        {
            Item = item ?? throw new ArgumentNullException(nameof(item));
        }

        public string GetConstraintName() => Constants.Constraints.ListItemType;

    }
}
