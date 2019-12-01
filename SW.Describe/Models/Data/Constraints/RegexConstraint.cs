using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace SW.Describe.Models
{
    
    public class RegexConstraint : ITypeConstraintSpecs
    {
        RegexConstraint()
        {

        }

        public string Regex { get; private set; }

        public RegexConstraint(string regex)
        {
            Regex = regex ?? throw new ArgumentNullException(nameof(regex));
        }

        public string GetConstraintName() => Constants.Constraints.Regex;

    }
}
