using System;
using System.Collections.Generic;
using System.Text;

namespace SW.Content.Search.EF
{
    public class DocSortValue
    {
        public DbDoc Doc { get; set; }

        public DbDocToken OrderBy { get; set; }
    }
}
