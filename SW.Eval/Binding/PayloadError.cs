using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace SW.Eval.Binding
{
    

    public class PayloadError : PayloadError<object>
    {
        
        public PayloadError(string error): base(error) { }
        
    }

    public class PayloadError<T>: IPayloadError, IPayload<T>
    {
        public PayloadError(string error)
        {
            Error = error;
        }

        public string Error { get; }

        public T Value => default;

        public IPayload<K> Map<K>(Func<T, K> map) => new PayloadError<K>(Error);

        public IEnumerator<KeyValuePair<PayloadPath, IPayload>> GetEnumerator()
        {
            yield break;
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public IPayload ValueOf(PayloadPath path) => this;
        
    }
}
