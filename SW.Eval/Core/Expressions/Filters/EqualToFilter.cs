
using System;
using System.Collections.Generic;
using System.Text;

namespace SW.Eval
{
    public class EqualToFilter : EvalFilterBase, IEquatable<EqualToFilter>
    {
        public IEvalExpression Left { get; private set; }

        public IEvalExpression Right { get; private set; }
        
        public EqualToFilter(IEvalExpression left, IEvalExpression right)
        {
            Left = left ?? throw new ArgumentNullException(nameof(left));
            Right = right ?? throw new ArgumentNullException(nameof(right));
        }

        public override bool IsMatch(IReadOnlyDictionary<IEvalExpression, IPayload> args)
            => args[Left].Equals(args[Right]);

        public override IEnumerable<IEvalExpression> GetChildren()
        {
            yield return Left;
            yield return Right;
        }

        public override string ToString() => $"{Left} EQUALS {Right}";
        
        public override bool Equals(object obj) => Equals(obj as EqualToFilter);
        
        public bool Equals(EqualToFilter other)
            => other != null &&
                    //Type == other.Type &&
                    Left.Equals(other.Left) &&
                    Right.Equals(other.Right);
        
        public override int GetHashCode()
        {
            var hashCode = 368036125;
            hashCode = hashCode * -1521134295 + EqualityComparer<IEvalExpression>.Default.GetHashCode(Left);
            hashCode = hashCode * -1521134295 + EqualityComparer<IEvalExpression>.Default.GetHashCode(Right);
            hashCode = hashCode * -1521134295;
            return hashCode;
        }

        

        public static bool operator ==(EqualToFilter expression1, EqualToFilter expression2)
        {
            return EqualityComparer<EqualToFilter>.Default.Equals(expression1, expression2);
        }

        public static bool operator !=(EqualToFilter expression1, EqualToFilter expression2)
        {
            return !(expression1 == expression2);
        }
    }
}
