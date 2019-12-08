using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Reflection;
using SW.Eval.Binding.Internal;
using System.Collections;

namespace SW.Eval.Binding.StandardTypes
{
    public class FromDictionary : IPayloadTypeReaderFactory
    {
        static IPayload Read<T>(PayloadReaderContext ctx, IEnumerable<KeyValuePair<string, T>> source)
            
            => PayloadObject.Combine(source.Select(pair => 
                new KeyValuePair<PayloadPath, IPayload>(
                    PayloadPath.Root.Append(pair.Key),
                    ctx.CreateSub(source).Read(pair.Value))));

        public PayloadTypeReader<T> GetReader<T>()
        {
            var type = typeof(T);
            if (!typeof(IEnumerable).IsAssignableFrom(type)) return null;

            var argType = type.GetEnumerableTypeArgument();

            if (argType == null) return null;

            if (!argType.IsGenericType || 
                argType.GetGenericTypeDefinition() != typeof(KeyValuePair<,>))
            {
                return null;
            }

            var typeArgs = argType.GetGenericArguments();
            if (typeArgs.First() != typeof(string)) return null;
            
            var m = typeof(FromDictionary)
                .GetMethod(nameof(Read), BindingFlags.Static | BindingFlags.NonPublic)
                .MakeGenericMethod(typeArgs.Last());

            return (PayloadTypeReader<T>)Delegate.CreateDelegate(typeof(PayloadTypeReader<T>), m);
        }
    }
}
