using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SW.Describe
{
    public class PayloadSnapshot : IPayload
    {
        readonly KeyValuePair<EPath, IPayload>[] data;

        public PayloadSnapshot(IEnumerable<KeyValuePair<EPath, IPayload>> data)
        {
            if (data == null) throw new ArgumentNullException(nameof(data));
            
            this.data = data.ToArray();
        }

        public IEnumerator<KeyValuePair<EPath, IPayload>> GetEnumerator()
        {
            foreach (var item in data) yield return item;
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        
    }
}
