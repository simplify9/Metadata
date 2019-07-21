using System;
using System.Collections.Generic;
using System.Text;

namespace SW.Content.Search.Commands
{
    public interface IIndexCommand
    {
        DocumentSource Source
        {
            get;
        }
    }
}
