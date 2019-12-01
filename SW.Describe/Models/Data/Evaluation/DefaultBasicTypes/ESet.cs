
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SW.Describe.Models
{
   
    public class ESet : IPayload, IEnumerable<IPayload>
    {
        

        IPayload[] Items { get; }

        public ESet(IEnumerable<IPayload> items)
        {
            Items = items?.ToArray() ?? throw new ArgumentNullException(nameof(items));
        }
        
        public ComparisonResult CompareWith(IPayload other)
        {
            if (other == null) throw new ArgumentNullException(nameof(other));
            
            return ComparisonResult.NotEqualTo;
        }
        
        public bool TryEvaluate(EPath path, out IPayload result)
        {
            if (path == null) throw new ArgumentNullException(nameof(path));

            result = null;
            if (path == EPath.Root)
            {
                result = this;
                return true;
            }

            return false;
        }

        public IEnumerator<IPayload> GetEnumerator()
        {
            foreach (var item in Items)
            {
                yield return item;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        IEnumerator<KeyValuePair<EPath, IPayload>> IEnumerable<KeyValuePair<EPath, IPayload>>.GetEnumerator()
        {
            throw new NotImplementedException();
        }
    }
}
