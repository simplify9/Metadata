using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SW.Metadata.Core.Mapping
{
    public class FromClrNumberTypeProvider : IContentReader
    {
        static readonly Type[] _numberTypes = 
            {
                typeof(int),
                typeof(float),
                typeof(double),
                typeof(long),
                typeof(decimal),
                typeof(Enum)
            };
        
        public IContentNode CreateFrom(ContentReaderMap r, object obj)
        {
            var t = obj.GetType();
            t = t.IsNullableType() ? Nullable.GetUnderlyingType(t) : t;
            if (!_numberTypes.Any(n => n.IsAssignableFrom(t))) return null;
            return new ContentNumber((decimal)obj);
            
        }
    }
}
