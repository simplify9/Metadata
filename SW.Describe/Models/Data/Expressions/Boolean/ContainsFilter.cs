
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SW.Describe.Models
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
        
        public override bool IsMatch(IPayload document)
        {
            if (document == null) throw new ArgumentNullException(nameof(document));

            var leftIssue = List.TryEvaluate(document, out IPayload left);
            if (leftIssue != null) return false;

            var rightIssue = Item.TryEvaluate(document, out IPayload right);
            if (rightIssue != null) return false;

            //if (left is ESet list)
            //{
            //    return list.Any(i => i.CompareWith(right) == ComparisonResult.EqualTo);
            //}

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
                    //Type == other.Type &&
                    List.Equals(other.List) &&
                    Item.Equals(other.Item);
        }

        public override int GetHashCode()
        {
            var hashCode = -1217978;
            hashCode = hashCode * -1521134295 + EqualityComparer<IContentExpression>.Default.GetHashCode(List);
            hashCode = hashCode * -1521134295 + EqualityComparer<IContentExpression>.Default.GetHashCode(Item);
            hashCode = hashCode * -1521134295;
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
