using SW.Content;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SW.Content.Search
{
    public interface IIndexRepo
    {
        
        Task SaveTokens(DocumentToken[] events);

        Task DeleteDocuments(DocumentSource[] sources);
    }
}
