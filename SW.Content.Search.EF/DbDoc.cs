using System;
using System.Collections.Generic;
using System.Text;

namespace SW.Content.Search.EF
{
    public class DbDoc
    {
        public long Id { get; set; }

        public long? SourceIdNumber { get; set; }

        public Guid? SourceIdGuid { get; set; }

        public string SourceIdString { get; set; }

        public string SourceType { get; set; }

        public string BodyEncoding { get; set; }

        public string BodyData { get; set; }

        public ICollection<DbDocToken> Tokens { get; set; }

        public DateTime CreatedOn { get; set; }

        public DateTime LastIndexOn { get; set; }
    }
}
