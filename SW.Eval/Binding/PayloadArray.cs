using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace SW.Eval.Binding
{
    
    public class PayloadArray : IPayload, ISet
    {
        public static readonly PayloadArray Empty = 
            new PayloadArray(Array.Empty<KeyValuePair<PayloadPath, IPayload>>());

        readonly IEnumerable<KeyValuePair<PayloadPath, IPayload>> pairs;

        public static IPayload Combine(IEnumerable<IPayload> items)
        {
            var errors = items.Select(p => p).OfType<IPayloadError>();
            if (errors.Any()) return new PayloadError(errors);
 
            return new PayloadArray(items.Select((i, idx) => 
                new KeyValuePair<PayloadPath, IPayload>(PayloadPath.Root.Append(idx), i)));
        }

        public static IPayload FlatCombine(params IPayload[] items) 
            => Combine(items.SelectMany(i => i is ISet set? set.Items : new[] { i }));
        
        PayloadArray(IEnumerable<KeyValuePair<PayloadPath, IPayload>> pairs)
        {
            this.pairs = pairs ?? throw new ArgumentNullException(nameof(pairs));
        }

        public IEnumerable<IPayload> Items => this.ArrayItems();

        public IEnumerator<KeyValuePair<PayloadPath, IPayload>> GetEnumerator() => pairs.GetEnumerator();

        public IPayload ValueOf(PayloadPath path)
        {
            return path.Length < 1
                ? this
                : (pairs
                    .Where(p => path.First().Equals(p.Key.First()))
                    .Select(p => p.Value).FirstOrDefault() ?? NoPayload.Singleton)
                        .ValueOf(path.Sub(1));
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
