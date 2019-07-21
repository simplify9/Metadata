using System;
using System.Collections.Generic;
using System.Text;

namespace SW.Content.Search.EF
{
    public class DbDocSourcePath
    {
        public int Id { get; set; }

        public string DocumentType { get; set; }

        public string PathString { get; set; }
        
        public DateTime CreatedOn { get; set; }
    }
}
