using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace SW.Eval.Binding.StandardTypes
{
    public class FromString : IPayloadTypeReaderFactory
    {
        public PayloadTypeReader<T> GetReader<T>()
        {
            var type = typeof(T);

            if (type != typeof(string)) return null;

            var m = typeof(FromString).GetMethod(nameof(Read), BindingFlags.Static | BindingFlags.NonPublic);

            return (PayloadTypeReader<T>)Delegate.CreateDelegate(typeof(PayloadTypeReader<T>), m);
        }

        static IPayload Read(PayloadReaderContext ctx, string o) => new PayloadPrimitive<string>(o); 
        
    }
}
