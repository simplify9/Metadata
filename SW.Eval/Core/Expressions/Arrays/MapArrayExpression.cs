using SW.Eval.Binding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SW.Eval
{
    public class MapArrayExpression : IEvalExpression
    {
        public IEvalExpression SourceArray { get; }

        public IEvalExpression MapperExpr { get; }

        public string ItemVarName { get; }

        public string IndexVarName { get; }

        public MapArrayExpression(IEvalExpression sourceArray, 
            IEvalExpression mapperExpr,
            string itemVarName, 
            string indexVarName = null)
        {
            SourceArray = sourceArray ?? throw new ArgumentNullException(nameof(sourceArray));
            MapperExpr = mapperExpr ?? throw new ArgumentNullException(nameof(mapperExpr));
            ItemVarName = itemVarName ?? throw new ArgumentNullException(nameof(itemVarName));
            IndexVarName = indexVarName;
        }

        public IEvalState ComputeState(EvalContext ctx)
            => ctx.Apply(args =>
            {
                var source = args.Values.First();
                if (source is ISet set)
                {
                    var evals = set.Items.Select((i,idx) =>
                    {
                        var ctxInner = ctx.WithExpression(MapperExpr).WithVariable(ItemVarName, i);
                        if (IndexVarName != null)
                        {
                            ctxInner = ctxInner.WithVariable(IndexVarName, new PayloadPrimitive<int>(idx));
                        }

                        return MapperExpr.ComputeState(ctxInner);
                    });

                    return evals.Aggregate((e1, e2) => e1.MergeWith(e2));
                }
                else if (source is INull || source is INoPayload)
                {
                    return new EvalComplete(PayloadArray.Empty);
                }

                return new EvalComplete(
                    new PayloadError($"Map expects an array. Payload of type {source.GetType()}"));
            });

        public IEnumerable<IEvalExpression> GetChildren()
        {
            yield return SourceArray; 
        }
    }
}
