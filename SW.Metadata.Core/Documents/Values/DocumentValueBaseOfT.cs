using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SW.Metadata.Core
{
    public class DocumentValueBase<TValue> : IDocumentValue, IEquatable<DocumentValueBase<TValue>>
    {
        protected readonly TValue _value;

        public TValue Value => _value;

        public DocumentValueBase(TValue value)
        {
            _value = value;
        }

        public virtual IEnumerable<IDocumentValue> AsEnumerable(Func<object, IDocumentValue> wrapIn)
        {
            yield return this;
        }

        public virtual ComparisonResult CompareWith(IDocumentValue other)
        {
            if (other == null)
            {
                throw new ArgumentNullException(nameof(other));
            }
            
            IComparable comparable = (IComparable)_value;

            return other is DocumentValueBase<TValue> otherCast
                ? (ComparisonResult)comparable.CompareTo(otherCast._value)
                : ComparisonResult.NotEqualTo;
        }

        public virtual string CreateMatchKey()
        {
            return _value.ToString();
        }

        public virtual bool TryEvaluate(DocumentPath path, Func<object, IDocumentValue> wrapIn, out IDocumentValue result)
        {
            if (path == null) throw new ArgumentNullException(nameof(path));
            if (wrapIn == null) throw new ArgumentNullException(nameof(wrapIn));

            if (!path.Nodes.Any())
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
            return Equals(obj as DocumentValueBase<TValue>);
        }

        public bool Equals(DocumentValueBase<TValue> other)
        {
            return other != null &&
                   EqualityComparer<TValue>.Default.Equals(_value, other._value);
        }

        public override int GetHashCode()
        {
            return -1939223833 + EqualityComparer<TValue>.Default.GetHashCode(_value);
        }

        public static bool operator ==(DocumentValueBase<TValue> base1, DocumentValueBase<TValue> base2)
        {
            return EqualityComparer<DocumentValueBase<TValue>>.Default.Equals(base1, base2);
        }

        public static bool operator !=(DocumentValueBase<TValue> base1, DocumentValueBase<TValue> base2)
        {
            return !(base1 == base2);
        }
    }
}
