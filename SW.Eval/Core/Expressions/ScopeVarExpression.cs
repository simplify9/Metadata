using System;
using System.Collections.Generic;
using System.Text;

namespace SW.Eval
{
    public class ScopeVarExpression : IEvalExpression
    {
        public string VarName { get; private set; }

        public ScopeVarExpression(string varName)
        {
            VarName = varName ?? throw new ArgumentNullException(nameof(varName));
        }
        
        public override string ToString() => VarName;
        
        public override bool Equals(object obj) => 
            obj is ScopeVarExpression scopeVar && 
                VarName == scopeVar.VarName;
        
        public override int GetHashCode() => VarName.GetHashCode();

        public IEnumerable<IEvalExpression> GetChildren() => Array.Empty<IEvalExpression>();

        public IEvalState ComputeState(EvalContext ctx) 
            => ctx.Apply(_ => 
                new EvalComplete(ctx.ScopeVars.GetValue(VarName)));
    }
}
