using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace SW.Metadata.Core
{
    public class ContentObject : IContentNode
    {
        readonly Dictionary<string, object> _keyValues;
        readonly ContentReaderMap _registry;

        public ContentObject(IEnumerable<KeyValuePair<string, object>> keyValues, ContentReaderMap r)
        {
            if (keyValues == null) throw new ArgumentNullException(nameof(keyValues));

            _keyValues = keyValues.ToDictionary(i => i.Key, i => i.Value);
            _registry = r;
        }
        
        public ComparisonResult CompareWith(IContentNode other)
        {
            return ComparisonResult.NotEqualTo;
        }
        
        public bool TryEvaluate(ContentPath path, out IContentNode result)
        {
            if (path == null) throw new ArgumentNullException(nameof(path));
            
            result = null;

            if (!path.Nodes.Any())
            {
                result = this;
                return true;
            }
            
            var keyExists = _keyValues.TryGetValue(path.Nodes.First(), out object rawResult);
            if (keyExists)
            {
                path = path.Sub(1);
                return _registry
                    .CreateValue(rawResult)
                    .TryEvaluate(path, out result);
            }

            return false;
        }
        
    }
}
