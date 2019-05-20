using System;
using System.Collections.Generic;
using System.Text;

namespace SW.Content.Schema
{
    public class ContentProperty
    {
        public string Key { get; private set; }

        public bool IsRequired { get; private set; }

        public IMust Value { get; private set; }

        public ContentProperty(string key, IMust value, bool isRequired)
        {
            Key = key;
            Value = value;
            IsRequired = isRequired;
        }

    }
}
