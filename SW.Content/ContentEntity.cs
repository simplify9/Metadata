﻿using System;
using System.Collections.Generic;
using System.Text;

namespace SW.Content
{
    public class ContentEntity : ContentPrimitive<string>
    {
        public string EntityType { get; }

        public ContentEntity(string entityType, string value) : base(value)
        {
            EntityType = entityType;
        }

        public override string ToString()
        {
            return $"{EntityType}(\"{_value}\")";
        }
    }
}
