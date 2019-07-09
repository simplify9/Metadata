using System;
using System.Collections.Generic;
using System.Text;

namespace SW.Content
{
    public class ContentPathValue
    {
        public ContentPath Path { get; private set; }

        public IContentNode Value { get; private set; }

        ContentPathValue() { }

        public ContentPathValue(ContentPath path, IContentNode value)
        {
            Path = path ?? throw new ArgumentNullException(nameof(path));
            Value = value ?? throw new ArgumentNullException(nameof(value));
        }

        public override string ToString()
        {
            return $"[{Path}: {Value}]";
        }
    }
}
