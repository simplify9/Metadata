using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SW.Eval
{
    public class PayloadSnapshot : IPayload
    {
        readonly KeyValuePair<PayloadPath, IPayload>[] data;

        public PayloadSnapshot(IEnumerable<KeyValuePair<PayloadPath, IPayload>> data)
        {
            if (data == null) throw new ArgumentNullException(nameof(data));
            
            this.data = data.ToArray();
        }

        public IEnumerator<KeyValuePair<PayloadPath, IPayload>> GetEnumerator()
        {
            return (data as IEnumerable<KeyValuePair<PayloadPath,IPayload>>).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        
    }
}
