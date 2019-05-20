using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SW.Content
{
    public class ContentList : IContentNode
    {
        readonly IEnumerable<object> _values;
        readonly IContentFactory _contentFactory;

        public ContentList(IEnumerable<object> values, IContentFactory contentFactory)
        {
            _values = values ?? throw new ArgumentNullException(nameof(values));
            _contentFactory = contentFactory ?? throw new ArgumentNullException(nameof(contentFactory));
        }

        public IEnumerable<IContentNode> Items => 
            _values.Select(_contentFactory.CreateFrom);

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
