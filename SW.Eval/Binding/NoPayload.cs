using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace SW.Eval.Binding
{
    public class NoPayload : NoPayload<object>
    {
        public NoPayload()
        {
            
        }
        
    }

    public class NoPayload<T> : INoPayload, IPayload<T>
    {
        public static NoPayload<T> Singleton { get; } = new NoPayload<T>();

        public T Value => default;

        protected NoPayload()
        {

        }

        public IEnumerator<KeyValuePair<PayloadPath, IPayload>> GetEnumerator()
        {
            yield break;
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public IPayload<K> Map<K>(Func<T, K> func)
        {
            return NoPayload<K>.Singleton;
        }

        public IPayload<T1> Map<T1>(Func<object, T1> func)
        {
            return NoPayload<T1>.Singleton;
        }
    }
}
