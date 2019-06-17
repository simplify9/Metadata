using SW.Content.Filters;
using System;
using System.Collections.Generic;
using System.Text;

namespace SW.Content.Schema
{
    public class ContentSchemaRule
    {
        public string Name { get; private set; }

        public IContentFilter Filter { get; private set; }

        public ContentSchemaRule(string name, IContentFilter filter)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            Filter = filter ?? throw new ArgumentNullException(nameof(filter));
        }
    }
}
