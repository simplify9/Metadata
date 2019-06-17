using SW.Content.Expressions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SW.Content.Filters
{
    public class ContainsFilter : ContentFilterBase, IEquatable<ContainsFilter>
    {
        public IContentExpression List { get; private set; }

        public IContentExpression Item { get; private set; }

        public override ContentFilterType Type => ContentFilterType.Contains;
        

        public ContainsFilter(IContentExpression list, IContentExpression item)
        {
            List = list ?? throw new ArgumentNullException(nameof(list));
            Item = item ?? throw new ArgumentNullException(nameof(item));
        }
        
        public override bool IsMatch(IContentNode document)
        {
            if (document == null) throw new ArgumentNullException(nameof(document));

            var leftIssue = List.TryEvaluate(document, out IContentNode left);
            if (leftIssue != null) return false;

            var rightIssue = Item.TryEvaluate(document, out IContentNode right);
            if (rightIssue != null) return false;

            if (left is ContentList list)
            {
                return list.Items
                    .Any(i => i.CompareWith(right) == ComparisonResult.EqualTo);
            }

            return false;
        }

        public override string ToString()
        {
            return $"{List} CONTAINS {Item}";
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as ContainsFilter);
        }

        public bool Equals(ContainsFilter other)
        {
            return other != null &&
                    Type == other.Type &&
                    List.Equals(other.List) &&
                    Item.Equals(other.Item);
        }

        public override int GetHashCode()
        {
            var hashCode = -1217978;
            hashCode = hashCode * -1521134295 + EqualityComparer<IContentExpression>.Default.GetHashCode(List);
            hashCode = hashCode * -1521134295 + EqualityComparer<IContentExpression>.Default.GetHashCode(Item);
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
