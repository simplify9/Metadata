using System;
using System.Collections.Generic;
using System.Text;

namespace SW.Describe.Models
{
    
    public class PropertyRequiredConstraint : ITypeConstraintSpecs
    {
        PropertyRequiredConstraint()
        {

        }

        public string PropertyName { get; private set; }
        
        public PropertyRequiredConstraint(string propertyName)
        {
            PropertyName = propertyName ?? throw new ArgumentNullException(nameof(propertyName));
        }

        public string GetConstraintName() => $"{Constants.Constraints.PropertyRequired}:{PropertyName}";
    }
}
