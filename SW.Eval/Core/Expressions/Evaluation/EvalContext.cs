using SW.Eval.Binding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SW.Eval
{
    public class EvalContext
    {
        readonly IEvalExpression rootExpr;

        public LexicalScope ScopeVars { get; }

        public EvalContext(IEvalExpression rootExpr, LexicalScope scopeVars)
        {
            this.rootExpr = rootExpr ?? throw new ArgumentNullException(nameof(rootExpr));
            ScopeVars = scopeVars ?? throw new ArgumentNullException(nameof(scopeVars));
        }

        public EvalContext WithExpression(IEvalExpression subExpr) 
            => new EvalContext(subExpr, ScopeVars);
        
        public EvalContext WithVariable(string name, IPayload value)
            => new EvalContext(rootExpr, ScopeVars.SetVariable(name, value));
        

        public IEvalState Apply(EvalStateMapper mapper)
        {
            var children = rootExpr.GetChildren();

            var outputMap = children.Select(ch => 
                new KeyValuePair<IEvalExpression,IEvalState>(ch, 
                    ch.ComputeState(WithExpression(ch))));

            var runnables = outputMap.Select(m => m.Value)
                .OfType<EvalInProgress>()
                .SelectMany(eip => eip.ReadyToRun);

            // error stub
            var errs = outputMap.Select(m => m.Value)
                .OfType<EvalComplete>()
                .Select(ec => ec.Result)
                .OfType<IPayloadError>();
                
            if (errs.Any()) return new EvalComplete(new PayloadError(errs));

            return runnables.Any()
                ? new EvalInProgress(runnables)
                : mapper(outputMap.ToDictionary(k => k.Key, e => ((EvalComplete)e.Value).Result));
        }
        
    }
}
