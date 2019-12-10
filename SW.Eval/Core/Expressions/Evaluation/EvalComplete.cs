using SW.Eval.Binding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SW.Eval
{
    public class EvalComplete : IEvalState
    {
        public IPayload[] Results { get; }

        public EvalComplete(IEnumerable<IPayload> results)
        {
            if (results == null) throw new ArgumentNullException(nameof(results));
        
            Results = results.ToArray();
        }

        public EvalComplete(params IPayload[] results)
        {
            Results = results;
        }

        public IEvalState MergeWith(IEvalState other)
            => other is EvalComplete otherComplete
                ? (IEvalState)new EvalComplete(Results.Concat(otherComplete.Results))
                : other is EvalInProgress inProgress
                    ? inProgress
                    : throw new NotSupportedException("Merge not supported");

        public IEvalState Apply(EvalContext ctx, EvalStateMapper func)
        {
            if (func == null) throw new ArgumentNullException(nameof(func));

            var errors = Results.OfType<IPayloadError>().ToArray();
            return errors.Length > 0
                ? new EvalComplete(new PayloadError(errors))
                : func(ctx, Results);
        }
    }
}
