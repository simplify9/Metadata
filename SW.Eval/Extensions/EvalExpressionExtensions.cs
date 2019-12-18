using SW.Eval.Binding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SW.Eval
{
    public static class EvalExpressionExtensions
    {
        public static IEnumerable<IEvalExpression> Visit(this IEvalExpression expr)
        {
            yield return expr;
            foreach (var sub in expr.GetArgs())
            {
                foreach (var e in Visit(sub.Expression)) yield return e;
            }
        }

        

        static EvalStateMapper ApplyValidation(IEnumerable<EvalArg> argDefs, EvalStateMapper decoratee)
        {
            return (ctx, args) =>
            {
                var errors = Enumerable.Zip(argDefs, args, (def, value) => (def, value))
                    .Where(t => !t.def.Validate(t.value))
                    .Select(t => new PayloadError(t.def.Error));

                return errors.Any()
                    ? new EvalComplete(errors)
                    : decoratee(ctx, args);
            };
        }

        public static IEvalState GetState(this IEvalExpression expr, EvalContext ctx)
        {
            if (expr == null) throw new ArgumentNullException(nameof(expr));
            if (ctx == null) throw new ArgumentNullException(nameof(ctx));

            var mapper = expr.GetMapper();
            var args = expr.GetArgs().ToArray();
            
            return args.Length < 1
                // no arguments
                ? mapper(ctx, Array.Empty<IPayload>())
                // run arguments then expression
                : args.Select(arg => GetState(arg.Expression, ctx.CreateSub(arg.Name)))
                    .Aggregate((e1, e2) => e1.MergeWith(e2))
                    .Apply(ctx, ApplyValidation(args, mapper));
        }

        public static IEvalState GetStateAggregate(this IEvalExpression expr, 
            EvalContext ctx, 
            IEnumerable<IPayload> payloads,
            string itemVarName,
            string indexVarName)
        {
            
            // evaluate iterations over array
            var evals = payloads.Select((i, idx) =>
            {
                // create sub-context and push iteration map item variable
                var ctxInner = ctx.CreateSub(idx.ToString()).WithVariable(itemVarName, i);

                // push iteration index if used
                ctxInner = indexVarName != null
                    ? ctxInner.WithVariable(indexVarName, new PayloadPrimitive<int>(idx))
                    : ctxInner;

                // evaluate
                return expr.GetState(ctxInner);
            });

            // convert array of payloads to 'PayloadArray' as expected
            return evals.Any()
                ? evals.Aggregate((e1, e2) => e1.MergeWith(e2))
                : new EvalComplete(Array.Empty<IPayload>());
        }
        
    }
}
