using System;
using System.Collections.Generic;
using System.Text;

namespace SW.Content.Expressions
{
    public class ConstantExpression : IContentExpression
    {
        public IContentNode Constant { get; }

        public ConstantExpression(IContentNode constant)
        {
            Constant = constant ?? throw new ArgumentNullException(nameof(constant));
        }

        public ExpressionIssue TryEvaluate(LexicalScope scope, out IContentNode result)
        {
            result = Constant;

            return null;
        }
    }
}
