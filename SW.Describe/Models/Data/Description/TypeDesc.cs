using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SW.Describe.Models
{
    public class TypeDesc
    {
        static readonly TypeConstraint[] _noConstraints = Array.Empty<TypeConstraint>();
        
        TypeDesc()
        {

        }

        public static TypeDesc AnyData { get; } = new TypeDesc(_noConstraints);

        public static TypeDesc FromBaseType(string typeUri)
        {
            return new TypeDesc(new TypeConstraint(new BaseTypeConstraint(typeUri)));
        }

        public TypeConstraint[] Constraints { get; private set; }

        public TypeDesc(IEnumerable<TypeConstraint> constraints = null)
            : this(constraints?.ToArray() ?? _noConstraints)
        {
            
        }

        public TypeDesc(params TypeConstraint[] constraints)
        {
            Constraints = constraints;
        }
    }
}
