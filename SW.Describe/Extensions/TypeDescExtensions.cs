using SW.Describe.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SW.Describe
{
    public static class TypeDescExtensions
    {
        public static TypeDesc WithConstraints(this TypeDesc typeDesc, params TypeConstraint[] additional)
        {
            return new TypeDesc(typeDesc.Constraints.Concat(additional));
        }
    }
}
