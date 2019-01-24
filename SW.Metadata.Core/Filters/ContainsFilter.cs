using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SW.Metadata.Core
{
    public class ContainsFilter : IDocumentFilter, IEquatable<ContainsFilter>
    {
        public DocumentPath ListPath { get; private set; }

        public IDocumentValue ItemValue { get; private set; }

        public DocumentFilterType Type => DocumentFilterType.Contains;

        public ContainsFilter(DocumentPath listPath, IDocumentValue itemValue)
        {
            ListPath = listPath ?? throw new ArgumentNullException(nameof(listPath));
            ItemValue = itemValue ?? throw new ArgumentNullException(nameof(itemValue));
        }
        
        public bool IsMatch(Document document)
        {
            if (document == null) throw new ArgumentNullException(nameof(document));
            
            if (!document.TryEvaluate(ListPath, out IDocumentValue left))
            {
                return false;
            }
            
            return document
                .AsEnumerable(left)
                .Any(i => i.CompareWith(ItemValue) == ComparisonResult.EqualTo);
        }

        public override string ToString()
        {
            return $"{ListPath} CONTAINS {ItemValue}";
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as ContainsFilter);
        }

        public bool Equals(ContainsFilter other)
        {
            return other != null &&
                    Type == other.Type &&
                    ListPath.Equals(other.ListPath) &&
                    ItemValue.Equals(other.ItemValue);
        }

        public override int GetHashCode()
        {
            var hashCode = -1217978;
            hashCode = hashCode * -1521134295 + EqualityComparer<DocumentPath>.Default.GetHashCode(ListPath);
            hashCode = hashCode * -1521134295 + EqualityComparer<IDocumentValue>.Default.GetHashCode(ItemValue);
            hashCode = hashCode * -1521134295 + Type.GetHashCode();
            return hashCode;
        }

        public static bool operator ==(ContainsFilter expression1, ContainsFilter expression2)
        {
            return EqualityComparer<ContainsFilter>.Default.Equals(expression1, expression2);
        }

        public static bool operator !=(ContainsFilter expression1, ContainsFilter expression2)
        {
            return !(expression1 == expression2);
        }
    }
}
