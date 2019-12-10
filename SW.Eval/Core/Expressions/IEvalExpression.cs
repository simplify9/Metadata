
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SW.Eval
{
    

    public interface IEvalExpression
    {
        IEnumerable<EvalArg> GetArgs();

        EvalStateMapper GetMapper();
    }
}
