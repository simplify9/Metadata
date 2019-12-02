using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace SW.Eval.Binding
{
    
    public class PayloadArray : IPayload, ISet
    {
        readonly IEnumerable<KeyValuePair<PayloadPath, IPayload>> pairs;
        
        public PayloadArray(IEnumerable<KeyValuePair<PayloadPath, IPayload>> pairs)
        {
            this.pairs = pairs ?? throw new ArgumentNullException(nameof(pairs));
        }

        public IEnumerable<IPayload> Items => this.ArrayItems();

        public IEnumerator<KeyValuePair<PayloadPath, IPayload>> GetEnumerator() => pairs.GetEnumerator();

        public IPayload ValueOf(PayloadPath path)
        {
            return path.Length < 1
                ? this
                : (pairs
                    .Where(p => path.First().Equals(p.Key.First()))
                    .Select(p => p.Value).FirstOrDefault() ?? NoPayload.Singleton)
                        .ValueOf(path.Sub(1));
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
