using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;

namespace SW.Eval.Binding.StandardTypes
{
    public class ToDateTime : IPayloadTypeConverterFactory
    {
        
        static bool TryParse(string s, out DateTime dateTime)
        {
            return DateTime.TryParse(s, null, DateTimeStyles.RoundtripKind, out dateTime);
        }

        static IPayload<DateTime?> ConvertNullable(PayloadConversionContext ctx, IPayload payload)
            => Convert(ctx, payload).Map(v => (DateTime?)v);

        static IPayload<DateTime> Convert(PayloadConversionContext ctx, IPayload payload)
            => payload is IPayload<DateTime> prim
                ? prim
                : payload is IPayload<string> str && TryParse(str.Value, out DateTime parsed)
                    ? (IPayload<DateTime>)new PayloadPrimitive<DateTime>(parsed)
                    : new PayloadError<DateTime>($"Cannot create a struct of type {typeof(DateTime)} from payload ({payload.GetType()})");

        public PayloadTypeConverter<TTo> GetConverter<TTo>()
        {
            var type = typeof(TTo);

            if (type != typeof(DateTime) && type != typeof(DateTime?)) return null;

            
            MethodInfo m;
            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
            {
                var nullableType = type.GetGenericArguments().First();
                m = typeof(ToDateTime)
                    .GetMethod(nameof(ConvertNullable), BindingFlags.Static | BindingFlags.NonPublic);
            }
            else
            {
                m = typeof(ToDateTime)
                    .GetMethod(nameof(Convert), BindingFlags.Static | BindingFlags.NonPublic);
            }

            return (PayloadTypeConverter<TTo>)
                Delegate.CreateDelegate(typeof(PayloadTypeConverter<TTo>), m);
        }
    }
}
