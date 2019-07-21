using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SW.Content.Search
{
    public interface ISearchQueryHandler
    {
        Task<SearchQueryResult> Handle(SearchQuery query);
    }
}
