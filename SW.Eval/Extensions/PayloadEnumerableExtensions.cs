
using SW.Eval.Binding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SW.Eval
{
    using PayloadPair = KeyValuePair<PayloadPath, IPayload>;

    public static class PayloadEnumerableExtensions
    {
        
        public static IEnumerable<KeyValuePair<string,IPayload>> ObjectProperties(this IEnumerable<PayloadPair> payload)

            => payload
                .Where(pair => 
                    pair.Key.Length == 1 && 
                    pair.Key.First() is PayloadPath.Property)
                .Select(pair => new KeyValuePair<string, IPayload>(
                    ((PayloadPath.Property)pair.Key.First()).PropertyName,
                    pair.Value));

        public static IEnumerable<IPayload> ArrayItems(this IEnumerable<PayloadPair> payload)

            => payload
                .Where(pair =>
                    pair.Key.Length == 1 &&
                    pair.Key.First() is PayloadPath.Index)
                .Select(pair => pair.Value);
        
        public static IPayload ValueOf(this IPayload payload, string pathString)
        {
            if (pathString == null) throw new ArgumentNullException(nameof(pathString));

            return payload.ValueOf(PayloadPath.Parse(pathString));
        }
    }
}
