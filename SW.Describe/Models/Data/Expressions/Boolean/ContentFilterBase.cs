
using System;
using System.Collections.Generic;
using System.Text;

namespace SW.Describe.Models
{
    public abstract class ContentFilterBase : IContentFilter, IContentExpression
    {
        public abstract ContentFilterType Type { get; }

        public abstract bool IsMatch(IPayload document);

        public ExpressionIssue TryEvaluate(IPayload input, out IPayload result)
        {
            result = new EBool(IsMatch(input));
            return null;
        }
    }
}
