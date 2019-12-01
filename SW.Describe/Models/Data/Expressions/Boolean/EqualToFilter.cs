
using System;
using System.Collections.Generic;
using System.Text;

namespace SW.Describe.Models
{
    public class EqualToFilter : ContentFilterBase, IEquatable<EqualToFilter>
    {
        public IContentExpression Left { get; private set; }

        public IContentExpression Right { get; private set; }

        public override ContentFilterType Type => ContentFilterType.EqualTo;

        public EqualToFilter(IContentExpression left, IContentExpression right)
        {
            Left = left ?? throw new ArgumentNullException(nameof(left));
            Right = right ?? throw new ArgumentNullException(nameof(right));
        }

        public override bool IsMatch(IPayload value)
        {
            var leftIssue = Left.TryEvaluate(value, out IPayload left);
            if (leftIssue != null) return false;

            var rightIssue = Right.TryEvaluate(value, out IPayload right);
            if (rightIssue != null) return false;


            return false;// left.CompareWith(right) == ComparisonResult.EqualTo;
        }

        public override string ToString()
        {
            return $"{Left} EQUALS {Right}";
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as EqualToFilter);
        }

        public bool Equals(EqualToFilter other)
        {
            return other != null &&
                    //Type == other.Type &&
                    Left.Equals(other.Left) &&
                    Right.Equals(other.Right);
        }

        public override int GetHashCode()
        {
            var hashCode = 368036125;
            hashCode = hashCode * -1521134295 + EqualityComparer<IContentExpression>.Default.GetHashCode(Left);
            hashCode = hashCode * -1521134295 + EqualityComparer<IContentExpression>.Default.GetHashCode(Right);
            hashCode = hashCode * -1521134295;// + Type.GetHashCode();
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
