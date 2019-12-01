using System;
using System.Collections.Generic;
using System.Text;

namespace SW.Describe.Models
{
    
    public class EntityTypeConstraint : ITypeConstraintSpecs
    {
        EntityTypeConstraint()
        {

        }

        public string TypeName { get; private set; }

        public EntityTypeConstraint(string typeName)
        {
            TypeName = typeName ?? throw new ArgumentNullException(nameof(typeName));
        }

        public string GetConstraintName() => Constants.Constraints.EntityType;
    }
}
