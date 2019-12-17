using System;
using System.Collections.Generic;
using System.Text;

namespace SW.Eval
{
    public class EConcat : IEvalExpression
    {
        public IEnumerable<EvalArg> GetArgs()
        {
            throw new NotImplementedException();
        }

        public EvalStateMapper GetMapper()
        {
            throw new NotImplementedException();
        }
    }
}
