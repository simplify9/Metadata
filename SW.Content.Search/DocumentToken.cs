using SW.Content;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace SW.Content.Search
{
    public class DocumentToken
    {
        

        public DateTime Timestamp { get; }

        public DocumentSource Source { get; }

        public object SourceData { get;  }

        public DocumentSourcePath SourcePath { get; }
        
        public IContentNode Raw { get; }

        public IContentNode Normalized { get; }

        public CultureInfo Locale { get; }

        public DocumentToken(
            DocumentSource source,
            object sourceData,
            DocumentSourcePath sourcePath, 
            IContentNode raw, 
            IContentNode normalized,
            CultureInfo locale = null)
        {
            Source = source ?? throw new ArgumentNullException(nameof(source));
            SourcePath = sourcePath ?? throw new ArgumentNullException(nameof(sourcePath));
            SourceData = sourceData ?? throw new ArgumentNullException(nameof(sourceData));
            Raw = raw ?? throw new ArgumentNullException(nameof(raw));
            Normalized = normalized ?? throw new ArgumentNullException(nameof(normalized));
            Locale = locale;
        }
    }
}
