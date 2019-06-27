using System;
using System.Collections.Generic;
using System.Text;

namespace SW.Content
{
    public class ContentEntity : ContentPrimitive<string>
    {
        public string EntityName { get; }

        public ContentEntity(string entityType, string value) : base(value)
        {
            EntityName = entityType;
        }

        public override string ToString()
        {
            return $"{EntityName}(\"{_value}\")";
        }
    }
}
