using SW.Content.Schema;
using System;
using System.Collections.Generic;
using System.Text;

namespace SW.Content.Expressions
{
    public interface IContentExpression
    {
        

        ExpressionIssue TryEvaluate(IContentNode node, out IContentNode result);
    }
}
