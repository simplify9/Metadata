
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace SW.Describe.Models
{
    
    public class EObject : IPayload, IObject
    {
        
        public IEnumerable<KeyValuePair<string, IPayload>> Properties { get; }
        
        public EObject(IEnumerable<KeyValuePair<string,IPayload>> properties)
        {
            Properties = properties?.ToArray() ?? throw new ArgumentNullException(nameof(properties));
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

            var first = path.First();
            var rawResult = Properties.Where(pair => pair.Key == first.ToString());

            //if (rawResult.Any())
            //{
            //    path = path.Sub(1);
            //    return rawResult.First().Value.TryEvaluate(path, out result);
            //}

            return false;
        }

        public IEnumerator<KeyValuePair<EPath, IPayload>> GetEnumerator()
        {
            yield return new KeyValuePair<EPath, IPayload>(EPath.Root, this);

            foreach (var pair in Properties)
            {
                yield return new KeyValuePair<EPath, IPayload>(EPath.Root.Append(pair.Key), pair.Value);
            }
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        
    }
}
