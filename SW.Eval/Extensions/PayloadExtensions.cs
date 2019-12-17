
using SW.Eval.Binding;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace SW.Eval
{
    using PayloadPair = KeyValuePair<PayloadPath, IPayload>;

    public static class PayloadExtensions
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

        public static PayloadPair MakePair(this IPayload p, PayloadPath path)
        {
            return new PayloadPair(path, p);
        }

        

        public static string GetComparableSequence(this IPrimitive p)
        {
            if (p == null) throw new ArgumentNullException(nameof(p));

            if (p is IPayload<string> pStr) return pStr.Value;

            if (p is IPayload<DateTime> pDate) return pDate.Value.ToString("o");

            var v = p.GetValue();
            if (typeof(IFormattable).IsAssignableFrom(v.GetType()))
            {
                var m = ((IFormattable)v).ToString("n6", CultureInfo.InvariantCulture);
                return $"{m.IndexOf('.').ToString("00")}{m}";
            }

            return v.ToString();
        }
    }
}
