using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SW.Content.Serialization.Schema
{
    public class ObjectDto : ContentSchemaNodeDto
    {
        public ContentSchemaRuleDto[] Rules { get; set; }

        public ContentPropertyDto[] Properties { get; set; }
        
        
    }
}
