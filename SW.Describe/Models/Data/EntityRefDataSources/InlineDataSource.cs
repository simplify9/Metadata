using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SW.Describe.Models
{
    
    public class InlineDataSource : IEntityRefDataSource
    {
        InlineDataSource()
        {

        }

        public string[] Values { get; private set; }

        public InlineDataSource(IEnumerable<string> values)
        {
            if (values == null) throw new ArgumentNullException(nameof(values));
            
            Values = values.ToArray();
        }
    }
}
