using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace SW.Eval.Binding.StandardTypes
{
    public class ToPayload : IPayloadTypeConverterFactory
    {
        static IPayload<IPayload> Convert(PayloadConversionContext ctx, IPayload payload)

            => new PayloadPrimitive<IPayload>(payload);
        
        public PayloadTypeConverter<TTo> GetConverter<TTo>()
        {
            var type = typeof(TTo);

            if (typeof(IPayload) != type) return null;

            var m = typeof(ToPayload).GetMethod(nameof(Convert), 
                BindingFlags.Static | 
                BindingFlags.NonPublic);

            return (PayloadTypeConverter<TTo>)
                Delegate.CreateDelegate(typeof(PayloadTypeConverter<TTo>), m);
        }
    }
}
