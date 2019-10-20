using SW.Content.Filters;
using System;
using System.Collections.Generic;
using System.Text;

namespace SW.Content.Expressions
{
    public class FilterExpression : IContentExpression
    {
        public IContentFilter Filter { get; }

        public FilterExpression(IContentFilter filter)
        {
            Filter = filter ?? throw new ArgumentNullException(nameof(filter));
        }

        public ExpressionIssue TryEvaluate(IContentNode node, out IContentNode result)
        {
            if (node == null) throw new ArgumentNullException(nameof(node));
            result = new ContentBoolean(Filter.IsMatch(node));
            return null;
        }


    }
}
