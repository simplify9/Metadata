
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SW.Eval.Binding
{
    public class PayloadPrimitive<TValue> : IPayload<TValue>, IPrimitive, IEquatable<PayloadPrimitive<TValue>>
    {
        public TValue Value { get; }

        public PayloadPrimitive(TValue value)
        {
            if (value == null) throw new ArgumentNullException(nameof(value));
            Value = value;
        }
        
        public override string ToString() => Value.ToString();
        
        public override bool Equals(object obj) => Equals(obj as PayloadPrimitive<TValue>);

        public bool Equals(PayloadPrimitive<TValue> other)
        {
            return other != null && EqualityComparer<TValue>.Default.Equals(Value, other.Value);
        }

        public override int GetHashCode() => Value.GetHashCode();

        public object GetValue() => Value;

        public IEnumerator<KeyValuePair<PayloadPath, IPayload>> GetEnumerator()
        {
            yield return new KeyValuePair<PayloadPath, IPayload>(PayloadPath.Root, this);
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public IPayload<K> Map<K>(Func<TValue, K> map) => new PayloadPrimitive<K>(map(Value));

        public IPayload ValueOf(PayloadPath path)
        {
            return path.Length < 1 ? this : (IPayload)NoPayload<TValue>.Singleton;
        }

        public static bool operator ==(PayloadPrimitive<TValue> base1, PayloadPrimitive<TValue> base2)
        {
            return EqualityComparer<PayloadPrimitive<TValue>>.Default.Equals(base1, base2);
        }

        public static bool operator !=(PayloadPrimitive<TValue> base1, PayloadPrimitive<TValue> base2)
        {
            return !(base1 == base2);
        }
    }
}
