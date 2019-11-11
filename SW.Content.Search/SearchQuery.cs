using SW.Content;
using SW.Content.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SW.Content.Search
{
    public class SearchQuery
    {
        public enum Op
        {
            Equals,
            NotEquals,
            GreaterThan,
            GreaterThanOrEquals,
            LessThan,
            LessThanOrEquals,
            AnyOf
        };

        public class Line
        {
            public ContentPath Field { get; private set; }

            public Op Comparison { get; private set; }

            public IContentNode Value { get; private set; }

            public Line(ContentPath path, Op comparison, IContentNode value)
            {
                Field = path;
                Comparison = comparison;
                Value = value;
            }
        }

        public DocumentType DocumentType { get; private set; }
        
        public Line[] QueryLines { get; private set; }

        public ContentPath SortByField { get; private set; }

        public bool SortByDescending { get; private set; }

        public int Offset { get; private set; }

        public int Limit { get; private set; }

        SearchQuery() { }

        public SearchQuery(DocumentType docType, 
            IEnumerable<Line> lines, 
            ContentPath sortField, bool descending, int offset, int limit)
        {
            if (lines == null) throw new ArgumentNullException(nameof(lines));
            
            DocumentType = docType ?? throw new ArgumentNullException(nameof(docType));
            QueryLines = lines.ToArray();
            SortByField = sortField ?? throw new ArgumentNullException(nameof(sortField));
            SortByDescending = descending;
            Offset = offset;
            Limit = limit;
        }
    }
}
