using System;
using System.Collections.Generic;
using System.Text;

namespace SW.Eval
{
    public interface IEvalState
    {
        IEvalState MergeWith(IEvalState other);
    }
}
