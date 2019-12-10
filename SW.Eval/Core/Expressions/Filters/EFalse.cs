using System;
using System.Collections.Generic;
using System.Text;

namespace SW.Eval
{
    public class EFalse : EvalFilterBase, IEquatable<EFalse>
    {
        public static EFalse Singleton { get; } = new EFalse();

        EFalse() { }
        
        public override bool Equals(object obj) => Equals(obj as EFalse);
        
        public bool Equals(EFalse other) => other != null;
        
        public override int GetHashCode() => 2049151605;
        
        public override string ToString() => "FALSE";

        public override bool IsMatch(IPayload[] input) => false;

        public static bool operator ==(EFalse expression1, EFalse expression2)
        {
            return EqualityComparer<EFalse>.Default.Equals(expression1, expression2);
        }

        public static bool operator !=(EFalse expression1, EFalse expression2)
        {
            return !(expression1 == expression2);
        }
    }
}
