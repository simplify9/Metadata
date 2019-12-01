using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace SW.Eval.Binding.StandardTypes
{
    public class ToString : IPayloadTypeConverterFactory
    {
        static IPayload<string> Convert(PayloadConversionContext ctx, IPayload payload)

            => payload is IPrimitive prim
                ? (IPayload<string>)new PayloadPrimitive<string>(prim.GetValue().ToString())
                : new PayloadError<string>($"Cannot create a string from a non-primitive payload ({payload.GetType()})");

        public PayloadTypeConverter<TTo> GetConverter<TTo>()
        {
            var type = typeof(TTo);

            if (type != typeof(string)) return null;

            var m = typeof(ToString).GetMethod(nameof(Convert),
                BindingFlags.Static |
                BindingFlags.NonPublic);

            return (PayloadTypeConverter<TTo>)
                Delegate.CreateDelegate(typeof(PayloadTypeConverter<TTo>), m);
        }
    }
}
