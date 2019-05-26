using System;
using System.Collections.Generic;
using System.Text;

namespace SW.Content.Expressions
{
    public interface IContentExpression
    {
        bool TryEvaluate(LexicalScope scope, out IContentNode result);
    }
}
