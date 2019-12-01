using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace SW.Eval.Binding
{
    

    public class PayloadError : IPayloadError, IPayload
    {
        
        public string Error { get; }
        

        public PayloadError(string error) { Error = error; }
        
        public IEnumerator<KeyValuePair<PayloadPath, IPayload>> GetEnumerator()
        {
            yield break;
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        
    }

    public class PayloadError<T>: PayloadError, IPayload<T>
    {
        public PayloadError(string error) : base(error)
        {

        }

        public T Value => default;

        public IPayload<K> Map<K>(Func<T, K> map) => new PayloadError<K>(Error);
    }
}
