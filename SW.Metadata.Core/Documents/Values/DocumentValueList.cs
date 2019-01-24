using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SW.Metadata.Core
{
    public class DocumentValueList : IDocumentValue
    {
        readonly IEnumerable<object> _values;

        public DocumentValueList(IEnumerable<object> values)
        {
            _values = values ?? throw new ArgumentNullException(nameof(values));
        }

        public IEnumerable<IDocumentValue> AsEnumerable(Func<object, IDocumentValue> wrapIn)
        {
            return _values.Select(wrapIn);
        }

        public ComparisonResult CompareWith(IDocumentValue other)
        {
            return ComparisonResult.NotEqualTo;
        }

        public string CreateMatchKey()
        {
            return "n/a";
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
    }
}
