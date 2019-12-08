using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SW.Eval.Binding
{
    public class PayloadObject : IPayload, IObject
    {
        readonly IEnumerable<KeyValuePair<PayloadPath, IPayload>> pairs;
        
        public static IPayload Combine(IEnumerable<KeyValuePair<PayloadPath, IPayload>> pairs)
        {
            var errors = pairs.Select(p => p.Value).OfType<IPayloadError>();
            if (errors.Any()) return new PayloadError(errors);
            return new PayloadObject(pairs);
        }

        public IEnumerable<KeyValuePair<string, IPayload>> Properties => this.ObjectProperties();

        PayloadObject(IEnumerable<KeyValuePair<PayloadPath, IPayload>> pairs)
        {
            this.pairs = pairs ?? throw new ArgumentNullException(nameof(pairs));
        }

        public IEnumerator<KeyValuePair<PayloadPath, IPayload>> GetEnumerator() => pairs.GetEnumerator();
        
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public IPayload ValueOf(PayloadPath path)
        {
            return path.Length < 1
                ? this
                : (pairs
                    .Where(p => path.First().Equals(p.Key.First()))
                    .Select(p => p.Value).FirstOrDefault() ?? NoPayload.Singleton)
                        .ValueOf(path.Sub(1));
        }
    }
}
