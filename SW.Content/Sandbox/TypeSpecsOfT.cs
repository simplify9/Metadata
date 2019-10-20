using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SW.Content.Sandbox
{
    public class TypeSpecs<TContent>

    {
        TypeSpecs() { }

        public ITypeConstraint<TContent>[] Constraints { get; private set; }

        public TypeSpecs(IEnumerable<ITypeConstraint<TContent>> constraints)
        {
            if (constraints == null) throw new ArgumentNullException(nameof(constraints));
            
            Constraints = constraints.ToArray();
        }


    }
}
