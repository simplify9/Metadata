using System;
using System.Collections.Generic;
using System.Text;

namespace SW.Eval
{ 
    public interface INoPayload
    {
        
    }

    public interface INoPayload<T> : INoPayload, IPayload<T>
    {
        
    }
}
