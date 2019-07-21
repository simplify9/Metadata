using System;
using System.Collections.Generic;
using System.Text;

namespace SW.Content.Search.EF
{
    public class DbDocToken
    {
        public long Id { get; set; }

        public DbDoc Document { get; set; }
        
        public DbDocSourcePath Path { get; set; }

        public int Offset { get; set; }

        public string ValueAsString { get; set; }

        public decimal? ValueAsNumber { get; set; }

        public bool? ValueAsBoolean { get; set; }

        public DateTime? ValueAsDateTime { get; set; }

        public string ValueAsAny { get; set; }

        public DateTime CreatedOn { get; set; }

        public DateTime LastUpdatedOn { get; set; }
    }
}
