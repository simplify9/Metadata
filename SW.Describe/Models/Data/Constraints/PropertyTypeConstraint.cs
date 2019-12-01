using System;
using System.Collections.Generic;
using System.Text;

namespace SW.Describe.Models
{
    
    public class PropertyTypeConstraint : ITypeConstraintSpecs
    {
        PropertyTypeConstraint()
        {

        }

        public string Property { get; private set; }

        public TypeDesc PropertyType { get; private set; }
        
        public PropertyTypeConstraint(string propertyName, TypeDesc propertyType)
        {
            Property = propertyName ?? throw new ArgumentNullException(nameof(propertyName));
            PropertyType = propertyType ?? throw new ArgumentNullException(nameof(propertyType));
        }

        public string GetConstraintName() => $"{Constants.Constraints.PropertyType}:{Property}";

    }
}
