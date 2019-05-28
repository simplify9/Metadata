using System;
using System.Collections.Generic;
using System.Text;

namespace SW.Content.Expressions
{
    public class ScopeVariableExpression : IContentExpression
    {
        public ContentPath Path { get; }

        public ScopeVariableExpression(ContentPath path)
        {
            Path = path ?? throw new ArgumentNullException(nameof(path));
        }

        public ExpressionIssue TryEvaluate(LexicalScope scope, out IContentNode result)
        {
            return scope.TryEvaluate(Path, out result)
                ? null
                : new ExpressionIssue($"The expression '{Path}' was not in scope");
        }
    }
}
