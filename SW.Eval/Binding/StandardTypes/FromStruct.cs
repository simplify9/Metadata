using SW.Eval.Binding.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace SW.Eval.Binding.StandardTypes
{
    public class FromStruct : IPayloadTypeReaderFactory
    {
        protected static KeyValuePair<PayloadPath, IPayload> Lift<TPrimitive>(TPrimitive p)

            => new KeyValuePair<PayloadPath, IPayload>(PayloadPath.Root, new PayloadPrimitive<TPrimitive>(p));

        static IPayload Read<TStruct>(PayloadReaderContext ctx, TStruct o)
            where TStruct : struct
        {
            return new PayloadPrimitive<TStruct>(o);
        }

        static IPayload ReadNullable<TStruct>(PayloadReaderContext ctx, TStruct? o)
            where TStruct : struct
        {
            return new PayloadPrimitive<TStruct>(o.Value);
        }
        
        public PayloadTypeReader<T> GetReader<T>()
        {
            var type = typeof(T);

            if (!type.IsValueType) return null;

            MethodInfo m;
            
            if (type.IsNullableType())
            {
                var nullableType = type.GetGenericArguments().First();
                m = typeof(FromStruct)
                    .GetMethod(nameof(ReadNullable), BindingFlags.Static | BindingFlags.NonPublic)
                    .MakeGenericMethod(nullableType);
            }
            else
            {
                m = typeof(FromStruct)
                    .GetMethod(nameof(Read), BindingFlags.Static | BindingFlags.NonPublic)
                    .MakeGenericMethod(type);
            }

            return (PayloadTypeReader<T>)Delegate.CreateDelegate(typeof(PayloadTypeReader<T>), m);
        }
    }
}
