using System;
using System.Collections.Generic;
using System.Text;

namespace SW.Eval
{
    public class FalseFilter : EvalFilterBase, IEquatable<FalseFilter>
    {
        public static FalseFilter Singleton { get; } = new FalseFilter();

        FalseFilter() { }
        
        public override bool Equals(object obj) => Equals(obj as FalseFilter);
        
        public bool Equals(FalseFilter other) => other != null;
        
        public override int GetHashCode() => 2049151605;
        
        public override string ToString() => "FALSE";

        public override bool IsMatch(IReadOnlyDictionary<IEvalExpression, IPayload> input) => false;

        public static bool operator ==(FalseFilter expression1, FalseFilter expression2)
        {
            return EqualityComparer<FalseFilter>.Default.Equals(expression1, expression2);
        }

        public static bool operator !=(FalseFilter expression1, FalseFilter expression2)
        {
            return !(expression1 == expression2);
        }
    }
}
