using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SW.Content.Serialization.Schema
{
    public class ListDto : ContentSchemaNodeDto
    {
        public ContentSchemaRuleDto[] Rules { get; set; }

        public ContentSchemaNodeDto Item { get; set; }

        public int? MinItemCount { get; set; }

        public int? MaxItemCount { get; set; }
        
    }
}
