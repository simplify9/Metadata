using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SW.Describe.Models
{
    
    public class ENull : IPayload, IPrimitive, INull
    {
        static readonly string KEY = "_#%$";

        public static readonly ENull Singleton = new ENull();

        ENull() { }

        public override bool Equals(object obj) => (obj != null && obj is ENull);

        public override string ToString() => "NULL";
        
        public override int GetHashCode() => KEY.GetHashCode();

        public object GetValue() => null;

        public IEnumerator<KeyValuePair<EPath, IPayload>> GetEnumerator()
        {
            yield return new KeyValuePair<EPath, IPayload>(EPath.Root, this);
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
