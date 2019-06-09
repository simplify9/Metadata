using System;
using System.Collections.Generic;
using System.Text;

namespace SW.Content.Expressions
{
    public class ScopeRootExpression : IContentExpression
    {
        public ExpressionIssue TryEvaluate(LexicalScope scope, out IContentNode result)
        {
            return scope.TryEvaluate(ContentPath.Root(), out result)
                ? null 
                : new ExpressionIssue("No lexical scope provided");
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
