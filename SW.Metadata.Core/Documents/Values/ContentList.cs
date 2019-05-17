using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SW.Metadata.Core
{
    public class ContentList : IContentNode
    {
        readonly IEnumerable<object> _values;
        readonly ContentReaderMap _registry;

        public ContentList(IEnumerable<object> values, ContentReaderMap reg)
        {
            _values = values ?? throw new ArgumentNullException(nameof(values));
            _registry = reg;
        }

        public IEnumerable<IContentNode> Items => _values.Select(_registry.CreateValue);

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

            return false;
        }
    }
}
