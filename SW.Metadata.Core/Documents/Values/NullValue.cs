using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SW.Metadata.Core
{
    public class NullValue : IDocumentValue
    {
        static readonly string KEY = "_#%$";

        public NullValue()
        {

        }

        public IEnumerable<IDocumentValue> AsEnumerable(Func<object, IDocumentValue> wrapIn)
        {
            yield break;
        }

        public ComparisonResult CompareWith(IDocumentValue other)
        {
            if (other == null) throw new ArgumentNullException(nameof(other));
            
            return other is NullValue
                ? ComparisonResult.EqualTo
                : ComparisonResult.NotEqualTo;
        }

        public override bool Equals(object obj)
        {
            return obj != null && obj is NullValue;
        }

        public string CreateMatchKey()
        {
            return KEY;
        }

        public override string ToString()
        {
            return KEY;
        }

        public bool TryEvaluate(DocumentPath path, Func<object, IDocumentValue> wrapIn, out IDocumentValue result)
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

        public override int GetHashCode()
        {
            return KEY.GetHashCode();
        }
    }
}
