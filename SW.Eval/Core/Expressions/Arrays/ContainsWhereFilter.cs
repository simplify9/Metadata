
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SW.Eval
{
    public class ContainsWhereFilter : EvalFilterBase
    {
        
        public IEvalExpression List { get; private set; }

        public IEvalFilter ItemFilter { get; private set; }

        public ContainsWhereFilter(IEvalExpression list, IEvalFilter item)
        {
            List = list ?? throw new ArgumentNullException(nameof(list));
            ItemFilter = item ?? throw new ArgumentNullException(nameof(item));
        }

        public override IEnumerable<IEvalExpression> GetChildren()
        {
            yield return List;
        }

        public override string ToString() => $"{List} CONTAINS ({ItemFilter})";

        public override bool Equals(object obj)
            => obj != null && obj is ContainsWhereFilter other
                ? List.Equals(other.List) && ItemFilter.Equals(other.ItemFilter)
                : false;

        public override int GetHashCode() => List.GetHashCode() + ItemFilter.GetHashCode();

        public override bool IsMatch(IReadOnlyDictionary<IEvalExpression, IPayload> input)
        {
            // TODO Contains Where

            throw new NotImplementedException();
        }
    }
}
