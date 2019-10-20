using SW.Content.Expressions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SW.Content.Filters
{
    public class ContainsWhereFilter : ContentFilterBase
    {
        //public override ContentFilterType Type => ContentFilterType.ContainsWhere;
        
        public IContentExpression List { get; private set; }

        public IContentFilter ItemFilter { get; private set; }

        public ContainsWhereFilter(IContentExpression list, IContentFilter item)
        {
            List = list ?? throw new ArgumentNullException(nameof(list));
            ItemFilter = item ?? throw new ArgumentNullException(nameof(item));
        }

        public override bool IsMatch(IContentNode document)
        {
            if (document == null) throw new ArgumentNullException(nameof(document));

            var leftIssue = List.TryEvaluate(document, out IContentNode left);
            if (leftIssue != null) return false;
            
            if (left is ContentList list)
            {
                // evaluate filter against each child
                return list.Any(i => ItemFilter.IsMatch(i));
            }
            
            return false;
        }

        public override string ToString()
        {
            return $"{List} CONTAINS ({ItemFilter})";
        }
    }
}
