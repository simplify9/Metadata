using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;

namespace SW.Eval.Binding.Internal
{
    public class DelegateCache
    {
        readonly ConcurrentDictionary<Type, Delegate> data = new ConcurrentDictionary<Type, Delegate>();
        
        public Delegate GetOrCreate<TDel>(Type type, Func<Type,TDel> createWith) where TDel:Delegate
        {

            var reader = data.GetOrAdd(type, (t) => createWith(t));
            
            return reader;
        }
    }

    //public class ReaderCache
    //{
    //    readonly ConcurrentDictionary<Type, Delegate> data = new ConcurrentDictionary<Type, Delegate>();

    //    public PayloadTypeReader<T> GetOrCreate<T>(Type type, Func<PayloadTypeReader<T>> createWith)
    //    {

    //        var reader = data.GetOrAdd(type, (t) => createWith());
    //        if (typeof(T) == typeof(object))
    //        {
    //            return (ctx, v) => (IPayload)reader.CAs.DynamicInvoke(ctx, v);
    //        }

    //        return (PayloadTypeReader<T>)reader;
    //    }
    //}
}
