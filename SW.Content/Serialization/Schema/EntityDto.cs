using System;
using System.Collections.Generic;
using System.Text;

namespace SW.Content.Serialization.Schema
{
    public class EntityDto : ContentSchemaNodeDto
    {
        public ContentSchemaRuleDto[] Rules { get; set; }

        public string Name { get; set; }
    }
}
