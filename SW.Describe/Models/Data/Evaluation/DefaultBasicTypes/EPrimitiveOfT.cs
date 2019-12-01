
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SW.Describe.Models
{
    public class EPrimitive<TValue> : IPrimitive, IPayload, IEquatable<EPrimitive<TValue>>
    {
        public TValue Value { get; }

        public EPrimitive(TValue value)
        {
            Value = value;
        }
        
        public override string ToString() => Value.ToString();
        
        public override bool Equals(object obj) => Equals(obj as EPrimitive<TValue>);

        public bool Equals(EPrimitive<TValue> other)
        {
            return other != null && EqualityComparer<TValue>.Default.Equals(Value, other.Value);
        }

        public override int GetHashCode() => Value.GetHashCode();

        public object GetValue() => Value;

        public IEnumerator<KeyValuePair<EPath, IPayload>> GetEnumerator()
        {
            yield return new KeyValuePair<EPath, IPayload>(EPath.Root, this);
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public static bool operator ==(EPrimitive<TValue> base1, EPrimitive<TValue> base2)
        {
            return EqualityComparer<EPrimitive<TValue>>.Default.Equals(base1, base2);
        }

        public static bool operator !=(EPrimitive<TValue> base1, EPrimitive<TValue> base2)
        {
            return !(base1 == base2);
        }
    }
}
