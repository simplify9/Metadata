using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SW.Content.Search
{
    public interface ISearchQueryHandler
    {
        Task<SearchQueryResult<T>> Handle<T>(SearchQuery query);
    }
}
