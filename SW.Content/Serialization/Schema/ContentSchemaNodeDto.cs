using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SW.Content.Serialization.Schema
{
    public abstract class ContentSchemaNodeDto
    {
        public string NodeType => Map.NodesByName.Where(p => p.Value.Equals(GetType()))
            .Select(p => p.Key)
            .FirstOrDefault();
    }
}
