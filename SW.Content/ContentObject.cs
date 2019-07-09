using SW.Content.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace SW.Content
{
    public class ContentObject : IContentNode, IRawValueWrapper, IEnumerable<ContentPathValue>
    {
        readonly IContentNodeFactory _contentFactory;

        IEnumerable<KeyValuePair<string, object>> DataSource { get; }

        readonly object _rawValue;

        object IRawValueWrapper.RawValue => _rawValue;

        public IEnumerable<string> Keys => DataSource.Select(pair => pair.Key);

        public ContentObject(IEnumerable<KeyValuePair<string,object>> dataSource, object rawData,  IContentNodeFactory contentFactory)
        {
            DataSource = dataSource ?? throw new ArgumentNullException(nameof(dataSource));
            _rawValue = rawData;
            _contentFactory = contentFactory ?? throw new ArgumentNullException(nameof(contentFactory));
            
        }

        //public ContentObject(IEnumerable<KeyValuePair<string, object>> keyValues, IContentNodeFactory contentFactory)
        //{
        //    if (keyValues == null) throw new ArgumentNullException(nameof(keyValues));
        //    _contentFactory = contentFactory ?? throw new ArgumentNullException(nameof(contentFactory));
        //    _keyValues = keyValues.ToDictionary(i => i.Key, i => i.Value);

        //}

        public ComparisonResult CompareWith(IContentNode other)
        {
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

            var first = path.Nodes.First();
            var rawResult = DataSource.Where(pair => pair.Key == first);

            if (rawResult.Any())
            {
                path = path.Sub(1);

                return _contentFactory
                    .CreateFrom(rawResult.First().Value)
                    .TryEvaluate(path, out result);
            }

            return false;
        }

        public IEnumerator<ContentPathValue> GetEnumerator()
        {
            foreach (var pair in DataSource)
            {
                yield return new ContentPathValue(ContentPath.Root.Append(pair.Key), 
                    _contentFactory.CreateFrom(pair.Value));
            }
            
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
    }
}
