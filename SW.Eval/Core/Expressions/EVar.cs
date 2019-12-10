using System;
using System.Collections.Generic;
using System.Text;

namespace SW.Eval
{
    public class EVar : IEvalExpression
    {
        

        public string VarName { get; private set; }

        public EVar(string varName)
        {
            VarName = varName ?? throw new ArgumentNullException(nameof(varName));
        }
        
        public override string ToString() => VarName;
        
        public override bool Equals(object obj) => 
            obj is EVar scopeVar && 
                VarName == scopeVar.VarName;
        
        public override int GetHashCode() => VarName.GetHashCode();

        public IEnumerable<EvalArg> GetArgs() => Array.Empty<EvalArg>();

        public EvalStateMapper GetMapper() => 
            (ctx, args) => 
                new EvalComplete(ctx.ScopeVars.GetValue(VarName));
        
    }
}
