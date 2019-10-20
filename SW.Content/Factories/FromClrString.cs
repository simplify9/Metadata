using SW.Content.Schema;
using SW.Content.Utils;
using System;
using System.Collections.Generic;
using System.Text;

namespace SW.Content.Factories
{
    public class FromClrString : IContentNodeFactory, IContentSchemaNodeFactory
    {
        public IContentNode CreateFrom(object obj)
        {
            if (!(obj is string)) return null;
            return new ContentText((string)obj);
        }

        public ITypeDef CreateSchemaNodeFrom(Type type)
        {
            if (type != typeof(string)) return null;
            var schema = new TypeDef<ContentText>();
            return schema;

        }
    }
}
