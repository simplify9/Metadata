using SW.Eval.Binding.Internal;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace SW.Eval.Binding.StandardTypes
{
    public class ToEnumerable : IPayloadTypeConverterFactory
    {
        static readonly Type[] listTypes = {
            typeof(ICollection<>),
            typeof(IEnumerable<>),
            typeof(IList<>),
            typeof(IReadOnlyCollection<>),
            typeof(IReadOnlyList<>)
        };

        static IPayload<HashSet<TItem>> CreateHashSet<TItem>(PayloadConversionContext ctx, IPayload payload)
        {
            if (!(payload is ISet set)) return new PayloadError<HashSet<TItem>>($"Payload is not a set. It cannot be converted to type ({typeof(HashSet<TItem>)})");
            var hashSet = new HashSet<TItem>();

            foreach (var item in set.Items)
            {
                var convertedItem = ctx.Convert<TItem>(item);
                if (convertedItem is IPayloadError pError)
                {
                    return new PayloadError<HashSet<TItem>>(pError.Error);
                }
                hashSet.Add(convertedItem.Value);
            }

            return new PayloadPrimitive<HashSet<TItem>>(hashSet);
        }

        static IPayload<List<TItem>> CreateList<TItem>(PayloadConversionContext ctx, IPayload payload)
        {
            if (!(payload is ISet set)) return new PayloadError<List<TItem>>($"Payload is not a set. It cannot be converted to type ({typeof(List<TItem>)})");
            var container = new List<TItem>();

            foreach (var item in set.Items)
            {
                var convertedItem = ctx.Convert<TItem>(item);
                if (convertedItem is IPayloadError pError)
                {
                    return new PayloadError<List<TItem>>(pError.Error);
                }
                container.Add(convertedItem.Value);
            }

            return new PayloadPrimitive<List<TItem>>(container);
        }
        
        static IPayload<TItem[]> CreateArray<TItem>(PayloadConversionContext ctx, IPayload payload)

            => CreateList<TItem>(ctx, payload).Map(list => list.ToArray());
        

        public PayloadTypeConverter<TTo> GetConverter<TTo>()
        {
            var type = typeof(TTo);

            if (!typeof(IEnumerable).IsAssignableFrom(type)) return null;
            
            MethodInfo m = null;
            var flags = BindingFlags.Static | BindingFlags.NonPublic;
            if (type.IsArray)
            {
                if (type.GetArrayRank() > 1) return null;
                var itemType = type.GetElementType();
                m = typeof(ToEnumerable).GetMethod(nameof(CreateArray), flags)
                    .MakeGenericMethod(itemType);
            }
            else if(type.IsGenericType && type.GetGenericArguments().Length == 1) 
            {
                var openType = type.GetGenericTypeDefinition();
                var itemType = openType.GetGenericArguments().First();

                if (openType == typeof(List<>) || listTypes.Contains(openType))
                {
                    m = typeof(ToEnumerable)
                        .GetMethod(nameof(CreateList), flags)
                        .MakeGenericMethod(itemType);
                }
                else if (openType ==typeof(HashSet<>))
                {
                    m = typeof(ToEnumerable)
                        .GetMethod(nameof(CreateHashSet), flags)
                        .MakeGenericMethod(itemType);
                }
            }

            if (m == null) return null;

            return (PayloadTypeConverter<TTo>)Delegate
                .CreateDelegate(typeof(PayloadTypeConverter<TTo>), m);
        }
    }
}
