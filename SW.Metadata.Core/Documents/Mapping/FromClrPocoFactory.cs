using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace SW.Metadata.Core.Mapping
{
    public class FromClrPocoFactory : IDocumentValueFactory
    {
        public IDocumentValue CreateFrom(object obj)
        {
            var t = obj.GetType();
            var keyValuePairs = t.GetProperties(BindingFlags.Instance | BindingFlags.Public)
                .Select(p =>
                    new KeyValuePair<string, object>(p.Name, p.GetValue(obj)));

            return new DocumentKeyValueContainer(keyValuePairs);
        }
    }
}
