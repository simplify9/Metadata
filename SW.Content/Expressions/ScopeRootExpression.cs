using System;
using System.Collections.Generic;
using System.Text;

namespace SW.Content.Expressions
{
    public class ScopeRootExpression : IContentExpression
    {
        public ExpressionIssue TryEvaluate(IContentNode input, out IContentNode result)
        {
            result = input;
            return null;
        }

        public override string ToString()
        {
            return "$";
        }

        public override bool Equals(object obj)
        {
            return obj is ScopeRootExpression;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
