using SW.Content;
using System;
using System.Collections.Generic;
using System.Text;

namespace SW.Content.Search.Commands
{
    public class UpdateIndexCommand : IIndexCommand
    {
        public DocumentSource Source { get; }

        public IContentNode SourceData { get; }

        public UpdateIndexCommand(DocumentSource source, IContentNode sourceData)
        {
            Source = source ?? throw new ArgumentNullException(nameof(source));
            SourceData = sourceData ?? throw new ArgumentNullException(nameof(sourceData));
        }
    }
}
