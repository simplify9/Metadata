using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Linq;

namespace SW.Eval.Binding.StandardTypes
{
    public class ToClass : IPayloadTypeConverterFactory
    {
        
        public PayloadTypeConverter<TTo> GetConverter<TTo>()
        {
            var type = typeof(TTo);

            var flags = BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance;
            var ctor = type.GetConstructor(flags, null, new Type[0], null);
            var writableProps = type
                .GetProperties(BindingFlags.Instance | BindingFlags.Public)
                .Where(p => p.CanWrite);

            return (ctx, payload) =>
            {
                if (!(payload is IObject pObject)) return new PayloadError<TTo>($"Payload is not an object. Cannot convert to {type}");
                
                var o = ctor.Invoke(null);
                foreach (var prop in writableProps)
                {
                    var propValuePayload = payload.ValueOf(PayloadPath.Root.Append(prop.Name));
                    var converted = ctx.ConvertUntyped(propValuePayload, prop.PropertyType);
                    if (converted is IPrimitive prim) prop.SetValue(o, prim.GetValue());
                    else if (converted is INull) prop.SetValue(o, null);
                    else if (converted is IPayloadError pError) return new PayloadError<TTo>(pError.Errors);
                }

                return new PayloadPrimitive<TTo>((TTo)o);
            };
        }
    }
}
