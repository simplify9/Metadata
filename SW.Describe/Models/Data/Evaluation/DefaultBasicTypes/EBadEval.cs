using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace SW.Describe.Models
{
    public class EBadEval : IPayload, IBadEval
    {
        EBadEval() { }

        public static EBadEval Singleton { get; } = new EBadEval();

        public IEnumerator<KeyValuePair<EPath, IPayload>> GetEnumerator()
        {
            yield break;
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
