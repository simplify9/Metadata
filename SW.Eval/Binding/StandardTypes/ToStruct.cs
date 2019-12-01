using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace SW.Eval.Binding.StandardTypes
{
    public class ToStruct : IPayloadTypeConverterFactory
    {
        static IPayload<TStruct?> ConvertNullable<TStruct>(PayloadConversionContext ctx, IPayload payload)
            where TStruct : struct

            => Convert<TStruct>(ctx, payload).Map(v => (TStruct?)v);

        static IPayload<TStruct> Convert<TStruct>(PayloadConversionContext ctx, IPayload payload)
            where TStruct : struct

            => payload is IPayload<TStruct> prim
                ? prim
                : new PayloadError<TStruct>($"Cannot create a struct of type {typeof(TStruct)} from payload ({payload.GetType()})");


        public PayloadTypeConverter<TTo> GetConverter<TTo>()
        {
            var type = typeof(TTo);

            if (!type.IsValueType) return null;
            
            MethodInfo m;
            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
            {
                var nullableType = type.GetGenericArguments().First();
                m = typeof(ToStruct)
                    .GetMethod(nameof(ConvertNullable), BindingFlags.Static | BindingFlags.NonPublic)
                    .MakeGenericMethod(nullableType);
            }
            else
            {
                m = typeof(ToStruct)
                    .GetMethod(nameof(Convert), BindingFlags.Static | BindingFlags.NonPublic)
                    .MakeGenericMethod(type);
            }

            return (PayloadTypeConverter<TTo>)
                Delegate.CreateDelegate(typeof(PayloadTypeConverter<TTo>), m);
        }
    }
}
