using SW.Content.Expressions;
using System;
using System.Collections.Generic;
using System.Text;

namespace SW.Content.Filters
{
    public abstract class ContentFilterBase : IContentFilter, IContentExpression
    {
        public abstract ContentFilterType Type { get; }

        public abstract bool IsMatch(IContentNode document);

        public ExpressionIssue TryEvaluate(IContentNode input, out IContentNode result)
        {
            result = new ContentBoolean(IsMatch(input));
            return null;
        }
    }
}
