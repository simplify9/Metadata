using SW.Eval.Binding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SW.Eval
{
    public class EMap : IEvalExpression
    {
        public DataFunc Closure { get; }

        public IEvalExpression SourceArray { get; }

        public IEvalExpression MapperExpr => Closure.Body;

        public string ItemVarName => Closure.Parameters[0];

        public string IndexVarName => Closure.Parameters.Length < 2 ? null : Closure.Parameters[1];

        public EMap(IEvalExpression sourceArray, DataFunc closure)
        {
            Closure = closure ?? throw new ArgumentNullException(nameof(closure));
            
            SourceArray = sourceArray ?? throw new ArgumentNullException(nameof(sourceArray));

            if (closure.Parameters.Length < 1)
            {
                throw new ArgumentException("Closure must have at least one parameter for item in array", nameof(closure));
            }
        }

        public override string ToString()
        {
            return $"{SourceArray}.map({Closure})";
        }

        public IEnumerable<EvalArg> GetArgs()
        {
            yield return new EvalArg("src", SourceArray, 
                p => p is INull || p is INoPayload || p is ISet,
                    "Map source must be an array"); 
        }

        public EvalStateMapper GetMapper() =>
            (ctx, args) =>
            {
                var source = args.First();

                if (source is ISet set)
                {
                    return MapperExpr.GetStateAggregate(ctx, set.Items, ItemVarName, IndexVarName)
                        .Apply(ctx, (_, outputs) => 
                            new EvalComplete(PayloadArray.Combine(outputs)));
                }

                return new EvalComplete(PayloadArray.Empty);
            };
    }
}
