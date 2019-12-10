
using SW.Eval.Binding;
using System;
using System.Collections.Generic;
using System.Text;

namespace SW.Eval
{
    public abstract class EvalFilterBase : IEvalFilter, IEvalExpression
    {
        
        public abstract bool IsMatch(IPayload[] input);
        
        public virtual IEnumerable<EvalArg> GetArgs()
        {
            yield break;
        }

        public EvalStateMapper GetMapper() => 
            (ctx, args) => 
                new EvalComplete(new PayloadPrimitive<bool>(IsMatch(args)));
        
    }
}
