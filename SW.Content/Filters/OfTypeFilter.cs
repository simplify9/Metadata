using SW.Content.Expressions;
using System;
using System.Collections.Generic;
using System.Text;

namespace SW.Content.Filters
{
    public class OfTypeFilter : ContentFilterBase
    {
        public override ContentFilterType Type => ContentFilterType.OfType;

        public IContentExpression Value { get; }

        public ContentType ContentType { get; }

        public OfTypeFilter(IContentExpression value, ContentType contentType)
        {
            Value = value ?? throw new ArgumentNullException(nameof(value));
            ContentType = contentType;
        }

        public override bool IsMatch(IContentNode data)
        {
            if (data == null) throw new ArgumentNullException(nameof(data));

            var leftIssue = Value.TryEvaluate(data, out IContentNode left);
            if (leftIssue != null) return false;
            
            return left.ContentType() == ContentType;
        }
    }
}
