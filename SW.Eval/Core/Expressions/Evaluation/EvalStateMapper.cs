using System;
using System.Collections.Generic;
using System.Text;

namespace SW.Eval
{
    public delegate IEvalState EvalStateMapper(IReadOnlyDictionary<IEvalExpression,IPayload> args);
}
