using System;
using System.Collections.Generic;
using System.Text;

namespace SW.Eval
{
    public abstract class BinaryFilterBase : EvalFilterBase, IEquatable<BinaryFilterBase>
    {
        public IEvalFilter Left { get; }

        public IEvalFilter Right { get; }

        protected abstract bool IsMatch(bool left, bool right);

        public override bool IsMatch(IPayload[] args)
        {
            var leftValue = args[0];
            var rightValue = args[1];
            if (!(leftValue is IPayload<bool> leftBool)) return false;
            if (!(rightValue is IPayload<bool> rightBool)) return false;
            return IsMatch(leftBool.Value, rightBool.Value);
        }

        protected BinaryFilterBase(IEvalFilter left, IEvalFilter right)
        {
            Left = left ?? throw new ArgumentNullException(nameof(left));
            Right = right ?? throw new ArgumentNullException(nameof(right));
        }

        public override IEnumerable<EvalArg> GetArgs()
        {
            yield return new EvalArg("left", (IEvalExpression)Left);
            yield return new EvalArg("right", (IEvalExpression)Right);
        }
        
        public override bool Equals(object obj) => Equals(obj as BinaryFilterBase);
        
        public bool Equals(BinaryFilterBase other)
            => other != null &&
                   Left.Equals(other.Left) &&
                   Right.Equals(other.Right) &&
                   GetType() == other.GetType();
        

        public override int GetHashCode()
        {
            var hashCode = -412577974;
            hashCode = hashCode * -1521134295 + EqualityComparer<IEvalFilter>.Default.GetHashCode(Left);
            hashCode = hashCode * -1521134295 + EqualityComparer<IEvalFilter>.Default.GetHashCode(Right);
            return hashCode;
        }

        public static bool operator ==(BinaryFilterBase expression1, BinaryFilterBase expression2)
        {
            return EqualityComparer<BinaryFilterBase>.Default.Equals(expression1, expression2);
        }

        public static bool operator !=(BinaryFilterBase expression1, BinaryFilterBase expression2)
        {
            return !(expression1 == expression2);
        }
    }
}
