using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;

namespace SW.Content.Search
{
    public class SearchQueryResult<T>
    {
        public T[] Matches { get; private set; }

        public int Total { get; private set; }

        public SearchQueryResult(T[] matches, int total)
        {
            Matches = matches;
            Total = total;
        }
    }
}
