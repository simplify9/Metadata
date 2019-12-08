
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SW.Eval
{
    

    public interface IEvalExpression
    {
        IEnumerable<IEvalExpression> GetChildren();

        IEvalState ComputeState(EvalContext ctx);
    }
}
