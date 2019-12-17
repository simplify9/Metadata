using System;
using System.Collections.Generic;
using System.Text;

namespace SW.Eval
{
    public class ETrue : EvalFilterBase, IEquatable<ETrue>
    {
        
        public static ETrue Singleton { get; } = new ETrue();

        ETrue() { }
        
        public override bool Equals(object obj) => Equals(obj as ETrue);
        
        public bool Equals(ETrue other) => other != null;
        
        public override int GetHashCode() => 2049151605;
        
        public static bool operator ==(ETrue expression1, ETrue expression2)
        {
            return EqualityComparer<ETrue>.Default.Equals(expression1, expression2);
        }

        public static bool operator !=(ETrue expression1, ETrue expression2)
        {
            return !(expression1 == expression2);
        }

        public override string ToString() => "true";
        
        public override bool IsMatch(IPayload[] input) => true;
    }
}
