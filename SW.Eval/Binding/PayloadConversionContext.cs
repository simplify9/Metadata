using SW.Eval.Binding.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace SW.Eval.Binding
{
    public class PayloadConversionContext
    {
        readonly DelegateCache cache;
        readonly IEnumerable<IPayloadTypeConverterFactory> factories;

        public PayloadConversionContext(DelegateCache cache, IEnumerable<IPayloadTypeConverterFactory> factories)
        {
            this.cache = cache;
            this.factories = factories;
        }

        static IPayload<T> NoConverter<T>(PayloadConversionContext ctx, IPayload payload) 
            => new PayloadError<T>($"No converter found for type ({typeof(T)})");

        IPayload<TTo> CastPayload<TFrom,TTo>(PayloadTypeConverter<TFrom> converter, IPayload payload)
            where TFrom : TTo
        {
            var source = converter(this, payload);
            return source.Map(v => (TTo)v);
        }
        
        PayloadTypeConverter<T> FindConverter<T>() 
            => factories
                .Select(f => f.GetConverter<T>()).Where(r => r != null)
                .FirstOrDefault() ?? NoConverter<T>;

        Delegate FindCompatibleConverter(Type actualType)
        {
            return (Delegate)GetType()
                .GetMethod(nameof(FindConverter), BindingFlags.Instance | BindingFlags.NonPublic)
                .MakeGenericMethod(actualType)
                .Invoke(this, Array.Empty<object>());
        }

        public IPayload ConvertUntyped(IPayload payload, Type typeTo)
        {
            return typeof(PayloadConversionContext)
                .GetMethod(nameof(Convert))
                .MakeGenericMethod(typeTo)
                .Invoke(this, new object[] { payload, typeTo }) as IPayload;
        }

        public IPayload<T> Convert<T>(IPayload payload, Type exactType = null)
        {
            if (payload == null) throw new ArgumentNullException(nameof(payload));
            exactType = exactType ?? typeof(T);

            if (payload is IPayloadError pError) return new PayloadError<T>(pError.Errors);
            if (exactType == typeof(object))
            {
                if (payload is IPrimitive prim) return Convert<T>(payload, prim.GetValue().GetType());
                else if (payload is ISet) return Convert<T>(payload, typeof(object[]));
                else if (payload is IObject) return Convert<T>(payload, typeof(IReadOnlyDictionary<string,object>)); 
            }
            
            var converter = cache.GetOrCreate(exactType, FindCompatibleConverter);

            return typeof(T).Equals(exactType)

                ? ((PayloadTypeConverter<T>)converter)(this, payload)

                : (IPayload<T>)GetType()
                    .GetMethod(nameof(CastPayload), BindingFlags.Instance | BindingFlags.NonPublic)
                    .MakeGenericMethod(exactType, typeof(T))
                    .Invoke(this, new object[] { converter, payload });
        }
    }
}
