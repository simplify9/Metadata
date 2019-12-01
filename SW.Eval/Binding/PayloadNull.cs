using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SW.Eval.Binding
{
    
    public class PayloadNull : IPayload, INull
    {
        static readonly string KEY = "_#%$";

        public static PayloadNull Singleton = new PayloadNull();
        
        PayloadNull()
        {

        }

        public override bool Equals(object obj) => (obj != null && obj is INull);

        public override string ToString() => "NULL";
        
        public override int GetHashCode() => KEY.GetHashCode();

        public object GetValue() => null;

        public IEnumerator<KeyValuePair<PayloadPath, IPayload>> GetEnumerator()
        {
            yield return new KeyValuePair<PayloadPath, IPayload>(PayloadPath.Root, this);
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        
    }
}
