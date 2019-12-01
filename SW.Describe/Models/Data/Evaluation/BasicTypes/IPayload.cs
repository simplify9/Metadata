using System;
using System.Collections.Generic;
using System.Text;

namespace SW.Describe
{
    public interface IPayload : IEnumerable<KeyValuePair<EPath, IPayload>>
    {
        
    }
}
