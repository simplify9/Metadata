using System;
using System.Collections.Generic;
using System.Text;

namespace SW.Describe.Models
{
    public class ScopeRootExpression : IContentExpression
    {
        static readonly ScopeRootExpression _singleton = new ScopeRootExpression();

        ScopeRootExpression()
        {

        }

        public static ScopeRootExpression Create() => _singleton;

        public ExpressionIssue TryEvaluate(IPayload input, out IPayload result)
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
