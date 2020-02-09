using SW.Content;
using SW.Content.Search.Commands;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SW.Content.Search
{
    public interface IIndexRepo
    {
        
       // Task SaveTokens(DocumentToken[] events);

        Task UpdateDocuments(Document[] commands);

        Task DeleteDocuments(DocumentSource[] sources);
    }
}
