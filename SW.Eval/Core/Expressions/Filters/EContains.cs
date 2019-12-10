
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SW.Eval
{
    public class EContains : EvalFilterBase, IEquatable<EContains>
    {
        public IEvalExpression List { get; }

        public IEvalExpression Item { get; }
        
        public EContains(IEvalExpression list, IEvalExpression item)
        {
            List = list ?? throw new ArgumentNullException(nameof(list));
            Item = item ?? throw new ArgumentNullException(nameof(item));
        }

        public override IEnumerable<EvalArg> GetArgs()
        {
            yield return new EvalArg("list", List,
                p => p is INull || p is INoPayload || p is ISet,
                    "Contains source must be an array");

            yield return new EvalArg("item", Item);
        }

        public override bool IsMatch(IPayload[] args)
            => args[0] is ISet set
                ? set.Items.Contains(args[1])
                : false;

        public override string ToString() => $"{List} CONTAINS {Item}";
        
        public override bool Equals(object obj) => Equals(obj as EContains);
        
        public bool Equals(EContains other)
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
        
        public static bool operator ==(EContains expression1, EContains expression2)
        {
            return EqualityComparer<EContains>.Default.Equals(expression1, expression2);
        }

        public static bool operator !=(EContains expression1, EContains expression2)
        {
            return !(expression1 == expression2);
        }
    }
}
