using System;
using System.Collections.Generic;
using System.Text;

namespace SW.Content.Serialization.Schema
{
    public class NumberDto : ContentSchemaNodeDto
    {
        public ContentSchemaRuleDto[] Rules { get; set; }
    }
}
