using System;
using System.Collections.Generic;
using System.Text;

namespace SW.Describe.Models
{
    public abstract class BinaryFilterBase : ContentFilterBase, IEquatable<BinaryFilterBase>
    {
        public IContentFilter Left { get; private set; }

        public IContentFilter Right { get; private set; }

        public override ContentFilterType Type { get; }

        protected BinaryFilterBase(ContentFilterType type, IContentFilter left, IContentFilter right)
        {
            Left = left ?? throw new ArgumentNullException(nameof(left));
            Right = right ?? throw new ArgumentNullException(nameof(right));
            Type = type;
        }

        //public abstract bool IsMatch(IContentNode document);

        public override bool Equals(object obj)
        {
            return Equals(obj as BinaryFilterBase);
        }

        public bool Equals(BinaryFilterBase other)
        {
            return other != null &&
                   Left.Equals(other.Left) &&
                   Right.Equals(other.Right) &&
                   Type == other.Type;
        }

        public override int GetHashCode()
        {
            var hashCode = -412577974;
            hashCode = hashCode * -1521134295 + EqualityComparer<IContentFilter>.Default.GetHashCode(Left);
            hashCode = hashCode * -1521134295 + EqualityComparer<IContentFilter>.Default.GetHashCode(Right);
            hashCode = hashCode * -1521134295 + Type.GetHashCode();
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
