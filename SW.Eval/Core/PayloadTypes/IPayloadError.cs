using System;
using System.Collections.Generic;
using System.Text;

namespace SW.Eval
{
    public interface IPayloadError : INoPayload
    { 
        string[] Errors { get; }
    }

    public interface IPayloadError<T> : IPayloadError, IPayload<T>
    {

    }
    
        
    
}
