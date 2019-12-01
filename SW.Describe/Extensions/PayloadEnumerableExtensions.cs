using SW.Describe.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SW.Describe
{
    using PayloadPair = KeyValuePair<EPath, IPayload>;

    public static class PayloadEnumerableExtensions
    {
        public static IPayload Self(this IEnumerable<PayloadPair> payload)

            => payload
                .Where(pair => pair.Key.Equals(EPath.Root))
                .Select(pair => pair.Value)
                .FirstOrDefault() ?? EBadEval.Singleton;

        public static IEnumerable<PayloadPair> Children(this IEnumerable<PayloadPair> payload)
                
            => payload
                .Where(pair => pair.Key.Length == 1);

        public static IEnumerable<PayloadPair> ObjectProperties(this IEnumerable<PayloadPair> payload)

            => payload
                .Where(pair => 
                    pair.Key.Length == 1 && 
                    pair.Key.First() is EPath.Property);

        public static IEnumerable<PayloadPair> SetItems(this IEnumerable<PayloadPair> payload)

            => payload
                .Where(pair =>
                    pair.Key.Length == 1 &&
                    pair.Key.First() is EPath.Index);

        public static IEnumerable<PayloadPair> Flatten(this IEnumerable<PayloadPair> payload)

            => payload.Self()
                .Concat(
                    payload.Children()
                        .SelectMany(pair =>      
                            pair.Value
                                .Flatten()
                                .Select(pSub =>
                                    new PayloadPair(
                                        EPath.Root.Append(pair.Key).Append(pSub.Key), 
                                        pSub.Value))));

        public static IPayload Read(this IEnumerable<PayloadPair> payload, EPath path)

            => path.Length < 1
                ? payload.Self()
                : payload.Children()
                    .Where(pair => pair.Key.First() == path.First())
                    .Read(path.Sub(1));
    }
}
