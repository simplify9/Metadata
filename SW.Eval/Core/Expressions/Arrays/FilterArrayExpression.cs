using System;
using System.Collections.Generic;
using System.Text;

namespace SW.Eval
{
    public class FilterArrayExpression : IEvalExpression
    {
        public IEvalState ComputeState(EvalContext ctx)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<IEvalExpression> GetChildren()
        {
            throw new NotImplementedException();
        }
    }
}
