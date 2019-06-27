using SW.Content.Schema;
using System;
using System.Collections.Generic;
using System.Text;

namespace SW.Content.Factories
{
    public class FromClrString : IContentFactory, IContentSchemaNodeFactory
    {
        public IContentNode CreateFrom(object obj)
        {
            if (!(obj is string)) return null;
            return new ContentText((string)obj);
        }

        public IMust CreateSchemaNodeFrom(Type type)
        {
            if (type != typeof(string)) return null;
            var schema = new MustHaveType<ContentText>(new ContentSchemaRule[] { });
            return schema;

        }
    }
}
