
using System;
using System.Collections.Generic;
using System.Text;

namespace SW.Content.Search.Commands
{
    public class DropIndexCommand : IIndexCommand
    {
        public DocumentSource Source { get; }

        public DropIndexCommand(DocumentSource source)
        {
            Source = source ?? throw new ArgumentNullException(nameof(source));
        }
    }
}
