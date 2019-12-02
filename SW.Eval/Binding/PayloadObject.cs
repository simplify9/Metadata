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
        
        public IEnumerable<KeyValuePair<string, IPayload>> Properties => this.ObjectProperties();

        public PayloadObject(IEnumerable<KeyValuePair<PayloadPath, IPayload>> pairs)
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
