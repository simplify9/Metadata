using System;
using System.Collections.Generic;
using System.Text;

namespace SW.Content.Serialization.Schema
{
    public class DateTimeDto : ContentSchemaNodeDto
    {
        public bool HasDate { get; set; }

        public bool HasTime { get; set; }

        public ContentSchemaRuleDto[] Rules { get; set; }
    }
}
