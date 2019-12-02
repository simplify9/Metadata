using System;
using System.Collections.Generic;
using System.Text;

namespace SW.Eval
{
    public interface IPayload : IEnumerable<KeyValuePair<PayloadPath, IPayload>>
    {
        IPayload ValueOf(PayloadPath path);
    }
}
