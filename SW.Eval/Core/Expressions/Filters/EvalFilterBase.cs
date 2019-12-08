
using SW.Eval.Binding;
using System;
using System.Collections.Generic;
using System.Text;

namespace SW.Eval
{
    public abstract class EvalFilterBase : IEvalFilter, IEvalExpression
    {
        
        public abstract bool IsMatch(IReadOnlyDictionary<IEvalExpression,IPayload> input);
        
        public virtual IEnumerable<IEvalExpression> GetChildren()
        {
            yield break;
        }

        public IEvalState ComputeState(EvalContext ctx)
            => ctx.Apply(args => 
                new EvalComplete(new PayloadPrimitive<bool>(IsMatch(args))));
    }
}
