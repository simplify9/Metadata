using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;

namespace SW.Content.Search
{
    public class SearchQueryResult
    {
        public JToken[] Matches { get; private set; }

        public int Total { get; private set; }

        public SearchQueryResult(JToken[] matches, int total)
        {
            Matches = matches;
            Total = total;
        }
    }
}
