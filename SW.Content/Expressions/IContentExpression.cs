using System;
using System.Collections.Generic;
using System.Text;

namespace SW.Content.Expressions
{
    public interface IContentExpression
    {
        ExpressionIssue TryEvaluate(LexicalScope scope, out IContentNode result);
    }
}
