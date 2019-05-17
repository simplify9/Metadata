using System;
using System.Collections.Generic;
using System.Text;

namespace SW.Metadata.Core.Schemas
{
    public class KeyValueContainerSpecification : IValueSpecification
    {
        public class KeyValue
        {
            public string Key { get; private set; }

            public bool IsRequired { get; private set; }

            public IValueSpecification ValueSpecification { get; private set; }
        }

        public IEnumerable<KeyValue> Properties { get; private set; }
        
        public bool IsMatch(IContentNode value)
        {
            if (value == null) throw new ArgumentNullException(nameof(value));
            
            if (value is ContentObject keyValueContainer)
            {

            }

            return false;
        }
    }
}
