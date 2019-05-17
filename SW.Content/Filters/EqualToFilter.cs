using System;
using System.Collections.Generic;
using System.Text;

namespace SW.Content.Filters
{
    public class EqualToFilter : IContentFilter, IEquatable<EqualToFilter>
    {
        public ContentPath Path { get; private set; }

        public IContentNode Value { get; private set; }

        public ContentFilterType Type => ContentFilterType.EqualTo;

        public EqualToFilter(ContentPath path, IContentNode value)
        {
            Path = path ?? throw new ArgumentNullException(nameof(path));
            Value = value ?? throw new ArgumentNullException(nameof(value));
        }

        public bool IsMatch(IContentNode value)
        {
            if (!value.TryEvaluate(Path, out IContentNode left))
            {
                return false;
            }

            return left.CompareWith(Value) == ComparisonResult.EqualTo;
        }

        public override string ToString()
        {
            return $"{Path} EQUALS {Value}";
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as EqualToFilter);
        }

        public bool Equals(EqualToFilter other)
        {
            return other != null &&
                    Type == other.Type &&
                    Path.Equals(other.Path) &&
                    Value.Equals(other.Value);
        }

        public override int GetHashCode()
        {
            var hashCode = 368036125;
            hashCode = hashCode * -1521134295 + EqualityComparer<ContentPath>.Default.GetHashCode(Path);
            hashCode = hashCode * -1521134295 + EqualityComparer<IContentNode>.Default.GetHashCode(Value);
            hashCode = hashCode * -1521134295 + Type.GetHashCode();
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
