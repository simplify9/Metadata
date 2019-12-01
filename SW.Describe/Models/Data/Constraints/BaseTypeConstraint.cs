using System;
using System.Collections.Generic;
using System.Text;

namespace SW.Describe.Models
{
    
    public class BaseTypeConstraint : ITypeConstraintSpecs
    {
        BaseTypeConstraint()
        {

        }

        public string BaseTypeUri { get; private set; }

        public BaseTypeConstraint(string baseTypeUri)
        {
            BaseTypeUri = baseTypeUri ?? throw new ArgumentNullException(nameof(baseTypeUri));
        }

        public string GetConstraintName() => Constants.Constraints.BaseType;
    }
}
