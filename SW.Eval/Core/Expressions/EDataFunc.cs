using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SW.Eval
{
    public class EDataFunc : IEvalExpression
    {
        public string FuncName { get; }

        public EvalArg[] Arguments { get; }
        
        public EDataFunc(string funcName, params EvalArg[] parameters)
        {
            FuncName = funcName;
            Arguments = parameters;
        }

        public EDataFunc(string funcName, IEnumerable<EvalArg> parameters)
        {
            if (parameters == null) throw new ArgumentNullException(nameof(parameters));
            FuncName = funcName ?? throw new ArgumentNullException(nameof(funcName));

            Arguments = parameters.ToArray();
        }
        
        public EDataFunc WithParam(string paramName, IEvalExpression paramExpr)
        {
            return new EDataFunc(FuncName, Arguments.Concat(new[] { new EvalArg(paramName, paramExpr) }));
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
