using System;
using System.Collections.Generic;
using System.Text;

namespace SW.Metadata.Core
{
    public abstract class BinaryFilterBase : IDocumentFilter, IEquatable<BinaryFilterBase>
    {
        public IDocumentFilter Left { get; private set; }

        public IDocumentFilter Right { get; private set; }
        
        public DocumentFilterType Type { get; }

        protected BinaryFilterBase(DocumentFilterType type, IDocumentFilter left, IDocumentFilter right)
        {
            Left = left ?? throw new ArgumentNullException(nameof(left));
            Right = right ?? throw new ArgumentNullException(nameof(right));
            Type = type;
        }

        public abstract bool IsMatch(DocumentContentReader document);

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
            hashCode = hashCode * -1521134295 + EqualityComparer<IDocumentFilter>.Default.GetHashCode(Left);
            hashCode = hashCode * -1521134295 + EqualityComparer<IDocumentFilter>.Default.GetHashCode(Right);
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
