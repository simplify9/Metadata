
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
        static readonly IPayload Empty = NoPayload.Singleton; // new PayloadSnapshot(Array.Empty<PayloadPair>());

        public static IPayload Self(this IEnumerable<PayloadPair> payload)

            => payload
                .Where(pair => pair.Key.Equals(PayloadPath.Root))
                .Select(pair => pair.Value)
                .FirstOrDefault() ?? Empty;

        public static IEnumerable<PayloadPair> Children(this IEnumerable<PayloadPair> payload)
                
            => payload
                .Where(pair => pair.Key.Length == 1);

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

        public static IPayload ValueOf(this IEnumerable<PayloadPair> payload, PayloadPath path)
        {
            if (path == null) throw new ArgumentNullException(nameof(path));
            
            return path.Length < 1
                ? payload.Self()
                : payload.Children()
                    .Where(pair => pair.Key.First().Equals( path.First()))
                    .Select(pair => new KeyValuePair<PayloadPath,IPayload>(
                        pair.Key.Sub(1),
                        pair.Value))
                    .ValueOf(path.Sub(1));
        }

        public static IPayload ValueOf(this IEnumerable<PayloadPair> payload, string pathString)
        {
            if (pathString == null) throw new ArgumentNullException(nameof(pathString));

            return payload.ValueOf(PayloadPath.Parse(pathString));
        }
    }
}
