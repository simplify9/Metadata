using System;
using System.Collections.Generic;
using System.Text;

namespace SW.Content.Filters
{
    public class OfTypeFilter : IContentFilter
    {
        public ContentFilterType Type => ContentFilterType.OfType;

        public ContentType ContentType { get; }

        public OfTypeFilter(ContentType contentType)
        {
            ContentType = contentType;
        }

        public bool IsMatch(IContentNode data)
        {
            if (data == null) throw new ArgumentNullException(nameof(data));

            return data.ContentType() == ContentType;
        }
    }
}
