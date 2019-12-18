using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SW.Eval.Binding
{
    
    public class PayloadNull : IPayload, INull
    {
        

        public static readonly PayloadNull Singleton = new PayloadNull();
        
        PayloadNull()
        {

        }
        
        public override string ToString() => "<<NULL>>";
        
        public object GetValue() => null;

        public IEnumerator<KeyValuePair<PayloadPath, IPayload>> GetEnumerator()
        {
            yield return new KeyValuePair<PayloadPath, IPayload>(PayloadPath.Root, this);
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public IPayload ValueOf(PayloadPath path) => 
            path.Equals(PayloadPath.Root)? (IPayload)this : NoPayload.Singleton;

        public override bool Equals(object obj)
        {
            return obj is INull;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
