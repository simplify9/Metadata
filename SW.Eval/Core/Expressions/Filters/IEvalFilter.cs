using System;
using System.Collections.Generic;
using System.Text;

namespace SW.Eval
{
    public interface IEvalFilter
    {
        bool IsMatch(IPayload[] input);
    }
}
