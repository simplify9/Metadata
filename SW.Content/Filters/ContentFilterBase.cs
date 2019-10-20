using SW.Content.Expressions;
using System;
using System.Collections.Generic;
using System.Text;

namespace SW.Content.Filters
{
    public abstract class ContentFilterBase : IContentFilter, ISpecification<IContentNode> //, IContentExpression
    {
        public abstract bool IsMatch(IContentNode document);
    }
}
