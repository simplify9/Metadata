using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SW.Eval.Binding
{
    

    public class PayloadError : PayloadError<object>
    {
        public PayloadError(Exception ex) : base(ex)
        {

        }

        public PayloadError(string error): base(error) { }

        public PayloadError(IEnumerable<string> errors) : base(errors)
        {
            
        }

        public PayloadError(IEnumerable<IPayloadError> errors) : base(errors)
        {
            
        }
    }

    public class PayloadError<T>: IPayloadError, IPayload<T>
    {
        public PayloadError(string error)
        {
            Errors = new[] { error };
        }

        public PayloadError(Exception ex) : this(ex.ToString())
        {
            
        }

        public PayloadError(IEnumerable<string> errors)
        {
            Errors = errors.ToArray();
        }

        public PayloadError(IEnumerable<IPayloadError> errors)
        {
            Errors = errors.SelectMany(err => err.Errors).ToArray();
        }

        public string[] Errors { get; }

        public T Value => default;

        public IPayload<K> Map<K>(Func<T, K> map) => new PayloadError<K>(Errors);

        public IEnumerator<KeyValuePair<PayloadPath, IPayload>> GetEnumerator()
        {
            yield break;
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public IPayload ValueOf(PayloadPath path) => this;

        public override bool Equals(object obj)
        {
            return obj is IPayloadError error && Errors.SequenceEqual(error.Errors);
        }

        public override int GetHashCode()
        {
            return 1564055702 + EqualityComparer<string[]>.Default.GetHashCode(Errors);
        }
    }
}
