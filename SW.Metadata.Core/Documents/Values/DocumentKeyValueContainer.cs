using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace SW.Metadata.Core
{
    public class DocumentKeyValueContainer : IDocumentValue
    {
        readonly Dictionary<string, object> _keyValues;

        public DocumentKeyValueContainer(IEnumerable<KeyValuePair<string, object>> keyValues)
        {
            if (keyValues == null) throw new ArgumentNullException(nameof(keyValues));

            _keyValues = keyValues.ToDictionary(i => i.Key, i => i.Value);
        }

        public IEnumerable<IDocumentValue> AsEnumerable(Func<object, IDocumentValue> wrapIn)
        {
            return _keyValues.Values.Select(wrapIn);
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
            
            if (!path.Nodes.Any())
            {
                result = this;
                return true;
            }
            
            var keyExists = _keyValues.TryGetValue(path.Nodes.First(), out object rawResult);
            if (keyExists)
            {
                path = path.Sub(1);
                return wrapIn(rawResult).TryEvaluate(path, wrapIn, out result);
            }

            result = null;
            return false;
        }
    }
}
