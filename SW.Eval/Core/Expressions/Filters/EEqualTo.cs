
using System;
using System.Collections.Generic;
using System.Text;

namespace SW.Eval
{
    public class EEqualTo : EvalFilterBase, IEquatable<EEqualTo>
    {
        public IEvalExpression Left { get; private set; }

        public IEvalExpression Right { get; private set; }
        
        public EEqualTo(IEvalExpression left, IEvalExpression right)
        {
            Left = left ?? throw new ArgumentNullException(nameof(left));
            Right = right ?? throw new ArgumentNullException(nameof(right));
        }

        public override bool IsMatch(IPayload[] args) => args[0].Equals(args[1]);

        public override IEnumerable<EvalArg> GetArgs()
        {
            yield return new EvalArg("left", Left);
            yield return new EvalArg("right", Right);
        }

        public override string ToString() => $"{Left} EQUALS {Right}";
        
        public override bool Equals(object obj) => Equals(obj as EEqualTo);
        
        public bool Equals(EEqualTo other)
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

        

        public static bool operator ==(EEqualTo expression1, EEqualTo expression2)
        {
            return EqualityComparer<EEqualTo>.Default.Equals(expression1, expression2);
        }

        public static bool operator !=(EEqualTo expression1, EEqualTo expression2)
        {
            return !(expression1 == expression2);
        }
    }
}
