using System;
using System.Collections.Generic;
using System.Text;

namespace SW.Eval
{
    public class ExpressionClosure
    {
        public string[] Parameters { get; }

        public IEvalExpression Body { get; }

        public ExpressionClosure(IEvalExpression expr, params string[] parameters)
        {
            Body = expr ?? throw new ArgumentNullException(nameof(expr));
            Parameters = parameters ?? throw new ArgumentNullException(nameof(parameters));
        }

        public override string ToString()
        {
            return $"{(Parameters.Length == 1? Parameters[0]: $"({string.Join(", ", Parameters)})")} => {Body}";
        }
    }
}
