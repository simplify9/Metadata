using SW.Content.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SW.Content
{
    public class ContentList : IContentNode, IEnumerable<IContentNode>, IRawValueWrapper
    {
        readonly IContentNodeFactory _contentFactory;
        readonly object _rawValue;

        object IRawValueWrapper.RawValue => _rawValue;

        IEnumerable DataSource { get; }

        public ContentList(IEnumerable dataSource, object rawValue, IContentNodeFactory contentFactory)
        {
            DataSource = dataSource ?? throw new ArgumentNullException(nameof(rawValue));
            _rawValue = rawValue ?? throw new ArgumentNullException(nameof(rawValue));
            _contentFactory = contentFactory ?? throw new ArgumentNullException(nameof(contentFactory));
        }
        
        public ComparisonResult CompareWith(IContentNode other)
        {
            if (other == null) throw new ArgumentNullException(nameof(other));
            
            return ComparisonResult.NotEqualTo;
        }
        
        public bool TryEvaluate(ContentPath path, out IContentNode result)
        {
            if (path == null) throw new ArgumentNullException(nameof(path));

            result = null;
            if (path == ContentPath.Root)
            {
                result = this;
                return true;
            }

            return false;
        }

        public IEnumerator<IContentNode> GetEnumerator()
        {
            foreach (var item in DataSource)
            {
                yield return _contentFactory.CreateFrom(item);
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
    }
}
