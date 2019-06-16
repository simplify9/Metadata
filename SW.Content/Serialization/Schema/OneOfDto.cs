using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SW.Content.Serialization.Schema
{
    public class OneOfDto : ContentSchemaNodeDto
    {
        public ContentSchemaNodeDto[] Options { get; set; }
        
        
    }
}
