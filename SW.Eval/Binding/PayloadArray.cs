using System;
using System.Collections;
using System.Collections.Generic;
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
        

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
