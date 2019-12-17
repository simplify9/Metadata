using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SW.Eval
{
    public class ECall : IEvalExpression
    {
        public string FuncName { get; }

        public EvalArg[] Arguments { get; }
        
        public ECall(string funcName, params EvalArg[] parameters)
        {
            FuncName = funcName;
            Arguments = parameters;
        }

        public ECall(string funcName, IEnumerable<EvalArg> parameters)
        {
            if (parameters == null) throw new ArgumentNullException(nameof(parameters));
            FuncName = funcName ?? throw new ArgumentNullException(nameof(funcName));

            Arguments = parameters.ToArray();
        }

        public override string ToString()
        {
            return $"{FuncName}({string.Join(", ", Arguments.Select(a => $"{a.Name}: {a.Expression}"))})";
        }

        public ECall WithParam(string paramName, IEvalExpression paramExpr)
        {
            return new ECall(FuncName, Arguments.Concat(new[] { new EvalArg(paramName, paramExpr) }));
        }

        public IEnumerable<EvalArg> GetArgs() => Arguments;

        public EvalStateMapper GetMapper() =>
            (ctx, args) =>
            {
                var requestId = ctx.Id;

                return ctx.MaterializedPayloads.TryGetValue(requestId, out IPayload dataResults)

                    ? (IEvalState)new EvalComplete(dataResults)

                    : new EvalInProgress(new DataRequest(FuncName, requestId, 
                        new EvalQueryArgs(Enumerable.Zip(Arguments, args, 
                            (def, value) => 
                                new KeyValuePair<string,IPayload>(def.Name, value)))));
            };
        
    }
}
