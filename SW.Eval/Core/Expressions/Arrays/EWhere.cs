using SW.Eval.Binding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SW.Eval
{
    public class EWhere : IEvalExpression
    {
        public IEvalExpression SourceArray { get; }

        public IEvalExpression MapperExpr { get; }

        public string ItemVarName { get; }

        public string IndexVarName { get; }

        public EWhere(IEvalExpression sourceArray,
            IEvalExpression mapperExpr,
            string itemVarName,
            string indexVarName = null)
        {
            SourceArray = sourceArray ?? throw new ArgumentNullException(nameof(sourceArray));
            MapperExpr = mapperExpr ?? throw new ArgumentNullException(nameof(mapperExpr));
            ItemVarName = itemVarName ?? throw new ArgumentNullException(nameof(itemVarName));
            IndexVarName = indexVarName;
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
                    return MapperExpr.MapRun(ctx, set.Items, ItemVarName, IndexVarName)
                        .Apply(ctx, (_, outputs) => 
                            new EvalComplete(
                                PayloadArray.Combine(
                                    Enumerable.Zip(set.Items,
                                            outputs.OfType<IPayload<bool>>(), 
                                            (item, isMatch) => (item, isMatch))
                                        .Where(t => t.isMatch.Value)
                                        .Select(t => t.item))));
                }

                return new EvalComplete(PayloadArray.Empty);
            };
    }
}
