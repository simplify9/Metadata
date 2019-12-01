using System;
using System.Collections;
using System.Collections.Generic;
using SW.Eval.Binding.Internal;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Reflection;

namespace SW.Eval.Binding.StandardTypes
{


    public class FromEnumerable : IPayloadTypeReaderFactory
    {
        
        public PayloadTypeReader<T> GetReader<T>()
        {
            var type = typeof(T);

            if (!typeof(IEnumerable).IsAssignableFrom(type)) return null;

            var argType = type.GetEnumerableTypeArgument();
            
            if (argType == null) return null;

            var m = typeof(FromEnumerable)
                .GetMethod(nameof(Read), BindingFlags.Static | BindingFlags.NonPublic)
                .MakeGenericMethod(argType);

            return (PayloadTypeReader<T>)Delegate.CreateDelegate(typeof(PayloadTypeReader<T>), m);
        }
        
        static IPayload Read<TItem>(PayloadReaderContext ctx, IEnumerable<TItem> source)
        {
            return new PayloadArray(source.Select(
                (v, i) =>
                    new KeyValuePair<PayloadPath, IPayload>(PayloadPath.Root.Append(i),
                        ctx.CreateSub(source).Read(v))));
        }
    }
}
