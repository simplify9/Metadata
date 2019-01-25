using System;
using System.Collections.Generic;
using System.Text;

namespace SW.Metadata.Core
{
    public class EqualToFilter : IDocumentFilter, IEquatable<EqualToFilter>
    {
        public DocumentPath Path { get; private set; }

        public IDocumentValue Value { get; private set; }

        public DocumentFilterType Type => DocumentFilterType.EqualTo;

        public EqualToFilter(DocumentPath path, IDocumentValue value)
        {
            Path = path ?? throw new ArgumentNullException(nameof(path));
            Value = value ?? throw new ArgumentNullException(nameof(value));
        }

        public bool IsMatch(DocumentContentReader reader)
        {
            if (!reader.TryEvaluate(Path, out IDocumentValue left))
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
            hashCode = hashCode * -1521134295 + EqualityComparer<DocumentPath>.Default.GetHashCode(Path);
            hashCode = hashCode * -1521134295 + EqualityComparer<IDocumentValue>.Default.GetHashCode(Value);
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
