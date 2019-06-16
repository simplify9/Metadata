using System;
using System.Collections.Generic;
using System.Text;

namespace SW.Content.Serialization.Schema
{
    public class ContentPropertyDto
    {
        public string Key { get; set; }

        public bool IsRequired { get; set; }

        public ContentSchemaNodeDto Value { get; set; }
        
    }
}
