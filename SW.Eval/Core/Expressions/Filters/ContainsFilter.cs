
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SW.Eval
{
    public class ContainsFilter : EvalFilterBase, IEquatable<ContainsFilter>
    {
        public IEvalExpression List { get; }

        public IEvalExpression Item { get; }
        
        public ContainsFilter(IEvalExpression list, IEvalExpression item)
        {
            List = list ?? throw new ArgumentNullException(nameof(list));
            Item = item ?? throw new ArgumentNullException(nameof(item));
        }

        public override IEnumerable<IEvalExpression> GetChildren()
        {
            yield return List;
            yield return Item;
        }

        public override bool IsMatch(IReadOnlyDictionary<IEvalExpression, IPayload> args)
            => args[List] is ISet set
                ? set.Items.Contains(args[Item])
                : false;

        public override string ToString() => $"{List} CONTAINS {Item}";
        
        public override bool Equals(object obj) => Equals(obj as ContainsFilter);
        
        public bool Equals(ContainsFilter other)
            => other != null &&
                    List.Equals(other.List) &&
                    Item.Equals(other.Item);
        
        public override int GetHashCode()
        {
            var hashCode = -1217978;
            hashCode = hashCode * -1521134295 + EqualityComparer<IEvalExpression>.Default.GetHashCode(List);
            hashCode = hashCode * -1521134295 + EqualityComparer<IEvalExpression>.Default.GetHashCode(Item);
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
