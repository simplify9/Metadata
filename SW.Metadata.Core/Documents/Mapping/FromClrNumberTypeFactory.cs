using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SW.Metadata.Core.Mapping
{
    public class FromClrNumberTypeFactory : IDocumentValueFactory
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
        
        public IDocumentValue CreateFrom(object obj)
        {
            var t = obj.GetType();
            t = t.IsNullableType() ? Nullable.GetUnderlyingType(t) : t;
            if (!_numberTypes.Any(n => n.IsAssignableFrom(t))) return null;
            return new NumericValue((decimal)obj);
            
        }
    }
}
