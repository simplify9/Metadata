using System;
using System.Collections.Generic;
using System.Text;

namespace SW.Eval
{
    public interface IObject
    {
        IEnumerable<KeyValuePair<string,IPayload>> Properties { get; }
    }
}
