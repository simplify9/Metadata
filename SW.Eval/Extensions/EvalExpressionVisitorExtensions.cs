using System;
using System.Collections.Generic;
using System.Text;

namespace SW.Eval
{
    public static class EvalExpressionVisitorExtensions
    {
        public static IEnumerable<IEvalExpression> Visit(this IEvalExpression expr)
        {
            yield return expr;
            foreach (var sub in expr.GetChildren())
            {
                foreach (var e in Visit(sub)) yield return e;
            }
        }
        
    }
}
