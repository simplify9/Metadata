using SW.Eval.Binding.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace SW.Eval.Binding
{
    

    public class PayloadReaderContext
    {
        static IPayload Bad<T>(PayloadReaderContext ctx, T payload)
        {
            return new PayloadError($"No reader found capable of reading data from type {typeof(T)}");
        }

        readonly DelegateCache cache;

        PayloadTypeReader<T> FindCompatibleReader<T>()
        {
            return Factories
                .Select(f => f.GetReader<T>()).Where(r => r != null)
                .FirstOrDefault() ?? Bad;
        }

        Delegate AdaptCast<T>(Type actualType)
        {
            return (Delegate)GetType()
                .GetMethod(nameof(FindCompatibleReader), BindingFlags.Instance | BindingFlags.NonPublic)
                .MakeGenericMethod(actualType)
                .Invoke(this, Array.Empty<object>());
        }

        PayloadReaderContext(object source, PayloadReaderContext parent, DelegateCache cache, IEnumerable<IPayloadTypeReaderFactory> factories)
        {
            Source = source;
            Parent = parent;
            Factories = factories;
            this.cache = cache;
        }

        public static PayloadReaderContext CreateRoot(DelegateCache cache, IEnumerable<IPayloadTypeReaderFactory> factories)
        {
            if (cache == null) throw new ArgumentNullException(nameof(cache));
            if (factories == null) throw new ArgumentNullException(nameof(factories));

            return new PayloadReaderContext(null, null, cache, factories);
        }
        
        public object Source { get; }

        public PayloadReaderContext Parent { get; }
        
        public IEnumerable<IPayloadTypeReaderFactory> Factories { get; }
        

        /// <summary>
        /// Resolves / caches a reader capable of reading given object as a payload 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="payload"></param>
        /// <returns></returns>
        public IPayload Read<T>(T payload)
        {
            if (payload == null) return PayloadNull.Singleton;
            var exactType = Nullable.GetUnderlyingType(typeof(T)) != null
                ? typeof(T)
                : payload.GetType();

            if (SeenBefore(payload)) return NoPayload.Singleton;

            

            var reader = cache.GetOrCreate(exactType, AdaptCast<T>);



            return typeof(T).Equals(exactType)
                ? ((PayloadTypeReader<T>)reader)(this, payload)
                : (IPayload)reader.DynamicInvoke(this, payload);
        }

        /// <summary>
        /// Increases indent one level down the source object tree and saves the parent object 
        /// for cycle detection
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public PayloadReaderContext CreateSub(object source)
        {
            return new PayloadReaderContext(source, this, cache, Factories);
        }

        /// <summary>
        /// a flag consulted to prevent infinite cycles
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public bool SeenBefore(object model) => Source == null
            ? false
            : Source.Equals(model)
                ? true
                : (Parent?.SeenBefore(model) ?? false);
        
    }
}
