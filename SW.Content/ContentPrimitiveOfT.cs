
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SW.Content
{
    public class ContentPrimitive<TValue> : IContentPrimitive, IContentNode, IEquatable<ContentPrimitive<TValue>>
    {
        protected readonly TValue _value;

        public TValue Value => _value;

        public ContentPrimitive(TValue value)
        {
            _value = value;
        }
        


        public virtual ComparisonResult CompareWith(IContentNode other)
        {
            if (other == null)
            {
                throw new ArgumentNullException(nameof(other));
            }
            
            IComparable comparable = (IComparable)_value;

            return other is ContentPrimitive<TValue> otherCast
                ? (ComparisonResult)comparable.CompareTo(otherCast._value)
                : ComparisonResult.NotEqualTo;
        }

        public virtual string CreateMatchKey()
        {
            return _value.ToString();
        }

        public virtual bool TryEvaluate(ContentPath path, out IContentNode result)
        {
            if (path == null) throw new ArgumentNullException(nameof(path));
            
            if (!path.Any())
            {
                result = this;
                return true;
            }

            result = null;
            return false;
        }

        public override string ToString()
        {
            return _value.ToString();
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as ContentPrimitive<TValue>);
        }

        public bool Equals(ContentPrimitive<TValue> other)
        {
            return other != null &&
                   EqualityComparer<TValue>.Default.Equals(_value, other._value);
        }

        public override int GetHashCode()
        {
            return -1939223833 + EqualityComparer<TValue>.Default.GetHashCode(_value);
        }
        
        public static bool operator ==(ContentPrimitive<TValue> base1, ContentPrimitive<TValue> base2)
        {
            return EqualityComparer<ContentPrimitive<TValue>>.Default.Equals(base1, base2);
        }

        public static bool operator !=(ContentPrimitive<TValue> base1, ContentPrimitive<TValue> base2)
        {
            return !(base1 == base2);
        }
    }
}
