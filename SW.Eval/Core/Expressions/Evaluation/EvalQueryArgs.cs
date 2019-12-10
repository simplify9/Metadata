using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SW.Eval
{
    public class EvalQueryArgs : IEnumerable<KeyValuePair<string, IPayload>>
    {
        readonly KeyValuePair<string, IPayload>[] data;

        public EvalQueryArgs(IEnumerable<KeyValuePair<string, IPayload>> data)
        {
            this.data = data?.ToArray() ?? throw new ArgumentNullException(nameof(data));
        }

        public IEnumerator<KeyValuePair<string, IPayload>> GetEnumerator()
        {
            return (data as IEnumerable<KeyValuePair<string, IPayload>>).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
