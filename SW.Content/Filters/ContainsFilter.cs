using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SW.Content.Filters
{
    public class ContainsFilter : IContentFilter, IEquatable<ContainsFilter>
    {
        public ContentPath ListPath { get; private set; }

        public IContentNode ItemValue { get; private set; }

        public ContentFilterType Type => ContentFilterType.Contains;

        public ContainsFilter(ContentPath listPath, IContentNode itemValue)
        {
            ListPath = listPath ?? throw new ArgumentNullException(nameof(listPath));
            ItemValue = itemValue ?? throw new ArgumentNullException(nameof(itemValue));
        }
        
        public bool IsMatch(IContentNode document)
        {
            if (document == null) throw new ArgumentNullException(nameof(document));
            
            if (!document.TryEvaluate(ListPath, out IContentNode left))
            {
                return false;
            }
            
            if (document is ContentList list)
            {
                return list.Items
                    .Any(i => i.CompareWith(ItemValue) == ComparisonResult.EqualTo);
            }

            return false;
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
            hashCode = hashCode * -1521134295 + EqualityComparer<ContentPath>.Default.GetHashCode(ListPath);
            hashCode = hashCode * -1521134295 + EqualityComparer<IContentNode>.Default.GetHashCode(ItemValue);
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
