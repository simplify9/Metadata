using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SW.Eval
{
    public class DataFunc<TQuery> : IEvalExpression, IDataFunc
    {
        public string QueryName { get; }

        public IReadOnlyDictionary<string,IEvalExpression> Parameters { get; }
        
        public DataFunc(string queryName, IReadOnlyDictionary<string, IEvalExpression> parameters)
        {
            QueryName = queryName;
            Parameters = parameters;
        }
        
        public IEnumerable<IEvalExpression> GetChildren() => Parameters.Values;

        public IEvalState ComputeState(EvalContext ctx)
            => ctx.Apply(args => 
                new EvalInProgress(new DataRequest(this, 
                    args.ToDictionary(
                        k => Parameters.Where(p => p.Value.Equals(k.Key)).Select(p => p.Key).First(),
                        v => v.Value))));
        
    }
}
