using System;
using System.Collections.Generic;
using System.Text;

namespace SW.Describe.Models
{
    
    public class WholeNumberConstraint : ITypeConstraintSpecs
    {
        static readonly WholeNumberConstraint _singleton = new WholeNumberConstraint();

        public static WholeNumberConstraint Create() => _singleton;

        WholeNumberConstraint()
        {

        }

        public string GetConstraintName() => Constants.Constraints.WholeNumber;
    }
}
