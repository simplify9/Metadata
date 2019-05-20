using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace SW.Content.Factories
{
    public class FromClrPoco : IContentFactory
    {
        readonly IContentFactory _memberFactory;

        public FromClrPoco(IContentFactory memberFactory)
        {
            _memberFactory = memberFactory;
        }

        public IContentNode CreateFrom( object obj)
        {
            var t = obj.GetType();
            var keyValuePairs = t.GetProperties(BindingFlags.Instance | BindingFlags.Public)
                .Select(p =>
                    new KeyValuePair<string, object>(p.Name, p.GetValue(obj)));

            return new ContentObject(keyValuePairs, _memberFactory);
        }
    }
}
