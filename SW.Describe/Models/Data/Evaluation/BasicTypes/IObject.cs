using System;
using System.Collections.Generic;
using System.Text;

namespace SW.Describe
{
    public interface IObject
    {
        IEnumerable<KeyValuePair<string,IPayload>> Properties { get; }
    }
}
