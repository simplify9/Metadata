using System;
using System.Collections.Generic;
using System.Text;

namespace SW.Eval
{
    public class TrueFilter : EvalFilterBase, IEquatable<TrueFilter>
    {
        
        public static TrueFilter Singleton { get; } = new TrueFilter();

        TrueFilter() { }
        
        public override bool Equals(object obj) => Equals(obj as TrueFilter);
        
        public bool Equals(TrueFilter other) => other != null;
        
        public override int GetHashCode() => 2049151605;
        
        public static bool operator ==(TrueFilter expression1, TrueFilter expression2)
        {
            return EqualityComparer<TrueFilter>.Default.Equals(expression1, expression2);
        }

        public static bool operator !=(TrueFilter expression1, TrueFilter expression2)
        {
            return !(expression1 == expression2);
        }

        public override string ToString() => "TRUE";
        
        public override bool IsMatch(IReadOnlyDictionary<IEvalExpression, IPayload> input) => true;
    }
}
