using System;
using System.Collections.Generic;
using System.Text;

namespace SW.Eval
{
    public class EvalArg
    {
        public string Name { get; }

        public IEvalExpression Expression { get; }

        public Func<IPayload,bool> Validate { get; }

        public string Error { get; }

        public EvalArg(string name, IEvalExpression expr)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            Expression = expr ?? throw new ArgumentNullException(nameof(expr));
            Validate = (IPayload p) => true;
        }

        public EvalArg(string name, IEvalExpression expr, Func<IPayload,bool> validator, string error)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            Expression = expr ?? throw new ArgumentNullException(nameof(expr));
            Validate = validator ?? throw new ArgumentNullException(nameof(validator));
            Error = error ?? throw new ArgumentNullException(nameof(error));
        }
    }
}
