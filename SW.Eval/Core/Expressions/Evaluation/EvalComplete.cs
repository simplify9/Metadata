using SW.Eval.Binding;
using System;
using System.Collections.Generic;
using System.Text;

namespace SW.Eval
{
    public class EvalComplete : IEvalState
    {
        public IPayload Result { get; }

        public EvalComplete(IPayload result)
        {
            Result = result ?? throw new ArgumentNullException(nameof(result));
        }

        public IEvalState MergeWith(IEvalState other)
            => other is EvalComplete otherComplete
                ? (IEvalState)new EvalComplete(PayloadArray.FlatCombine(Result, otherComplete.Result))
                : other is EvalInProgress inProgress
                    ? inProgress
                    : throw new NotSupportedException("Merge not supported");
    }
}
