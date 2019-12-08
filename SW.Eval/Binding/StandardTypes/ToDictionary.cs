using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Text;

namespace SW.Eval.Binding.StandardTypes
{
    public class ToDictionary : IPayloadTypeConverterFactory
    {
        static readonly Type[] dictionaryTypes =
        {
            typeof(IDictionary<,>),
            typeof(Dictionary<,>),
            typeof(IReadOnlyDictionary<,>)
        };

        static IPayload<ReadOnlyDictionary<string,TItem>> CreateReadOnlyDictionary<TItem>(PayloadConversionContext ctx, IPayload payload)
        {
            return CreateDictionary<TItem>(ctx, payload).Map(fromDict => new ReadOnlyDictionary<string, TItem>(fromDict));
        }

        static IPayload<Dictionary<string, TItem>> CreateDictionary<TItem>(PayloadConversionContext ctx, IPayload payload)
        {
            if (!(payload is IObject pObject))
            {
                return new PayloadError<Dictionary<string, TItem>>($"Cannot create dictionary from paylod of type ({payload.GetType()})");
            }

            var d = new Dictionary<string, TItem>();
            foreach (var pair in pObject.Properties)
            {
                var pairValue = ctx.Convert<TItem>(pair.Value);
                if (pairValue is IPayloadError pError)
                {
                    return new PayloadError<Dictionary<string, TItem>>(pError.Errors);
                }
                d.Add(pair.Key, pairValue.Value);
            }

            return new PayloadPrimitive<Dictionary<string, TItem>>(d);
        }

        public PayloadTypeConverter<TTo> GetConverter<TTo>()
        {
            var type = typeof(TTo);

            if (!type.IsGenericType) return null;

            var openType = type.GetGenericTypeDefinition();
            var typeArgs = type.GetGenericArguments();
            if (typeArgs.Length != 2) return null;
            if (typeArgs.First() != typeof(string)) return null;

            var itemType = typeArgs.Last();

            MethodInfo m = null;

            var flags = BindingFlags.Static | BindingFlags.NonPublic;
            if (openType == typeof(ReadOnlyDictionary<,>))
            {
                m = typeof(ToDictionary)
                    .GetMethod(nameof(CreateReadOnlyDictionary), flags)
                    .MakeGenericMethod(itemType);
            }
            else if (dictionaryTypes.Contains(openType))
            {
                m = typeof(ToDictionary)
                    .GetMethod(nameof(CreateDictionary), flags)
                    .MakeGenericMethod(itemType);
            }

            if (m == null) return null;

            return (PayloadTypeConverter<TTo>)Delegate
                .CreateDelegate(typeof(PayloadTypeConverter<TTo>), m);
        }
    }
}
