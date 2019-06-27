using SW.Content.Schema;
using SW.Content.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SW.Content.Factories
{
    public class FromClrNumberType : IContentFactory, IContentSchemaNodeFactory
    {
        static readonly Type[] _numberTypes =
            {
                typeof(int),
                typeof(float),
                typeof(double),
                typeof(long),
                typeof(decimal)
            };

        public IContentNode CreateFrom(object obj)
        {
            var t = obj.GetType();
            t = t.IsNullableType() ? Nullable.GetUnderlyingType(t) : t;
            if (!_numberTypes.Any(n => n.IsAssignableFrom(t))) return null;
            return new ContentNumber((decimal)Convert.ChangeType(obj, typeof(decimal)));
        }

        public IMust CreateSchemaNodeFrom(Type t)
        {
            var nullable = t.IsNullableType();
            t = nullable ? Nullable.GetUnderlyingType(t) : t;
            if (!_numberTypes.Any(n => n.IsAssignableFrom(t))) return null;
            var schema = new MustHaveType<ContentNumber>(new ContentSchemaRule[] { });
            return !nullable
                ? (IMust)schema
                : new MustBeOneOf(new IMust[]
                {
                    schema,
                    new MustBeNull()
                });
        }
    }
}
