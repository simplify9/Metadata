using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace SW.Describe.Models
{
    
    public class DateOrTimeConstraint : ITypeConstraintSpecs
    {
        DateOrTimeConstraint()
        {

        }

        public bool HasTime { get; private set; }

        public DateOrTimeConstraint(bool hasTime)
        {
            HasTime = hasTime;
        }

        public string GetConstraintName() => Constants.Constraints.DateOrTime;
    }
}
