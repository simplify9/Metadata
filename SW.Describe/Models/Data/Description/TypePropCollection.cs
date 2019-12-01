using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace SW.Describe.Models
{
    public class TypePropCollection : IEnumerable<KeyValuePair<string, object>>
    {
        //readonly IReadOnlyDictionary<string, object> data;

            
        public TypePropCollection(IEnumerable<KeyValuePair<string, object>> values)
        {

        }

        public IEnumerator<KeyValuePair<string, object>> GetEnumerator()
        {
            throw new NotImplementedException();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException();
        }
    }
}
