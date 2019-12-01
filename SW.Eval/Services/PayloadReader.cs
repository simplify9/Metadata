using SW.Eval.Binding;
using SW.Eval.Binding.Internal;
using SW.Eval.Binding.StandardTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace SW.Eval
{
    public class PayloadReader
    {
        static readonly MethodInfo MethodReadFrom = typeof(PayloadReaderContext)
            .GetMethods().First(m =>
                m.IsGenericMethod &&
                m.Name == nameof(PayloadReaderContext.Read));

        static readonly DelegateCache readerCache = new DelegateCache();

        //static readonly DelegateCache ctxMethodCache = new DelegateCache();

        readonly IPayloadTypeReaderFactory[] readerFactories;
        
        IEnumerable<IPayloadTypeReaderFactory> SortMergeReaders(IEnumerable<IPayloadTypeReaderFactory> additionalReaders)
        {
            foreach (var e in additionalReaders) yield return e;

            yield return new FromPayload();

            yield return new FromString();

            yield return new FromEnum();

            yield return new FromStruct();

            yield return new FromDictionary();

            yield return new FromEnumerable();

            yield return new FromClass();
        }

        public PayloadReader(IEnumerable<IPayloadTypeReaderFactory> readerFactories = null)
        {
            this.readerFactories = SortMergeReaders(readerFactories ?? 
                Array.Empty<IPayloadTypeReaderFactory>()).ToArray();
        }
        
        public IPayload Read<T>(T source)
        {
            if (source == null) return PayloadNull.Singleton;
            
            var ctx = PayloadReaderContext.CreateRoot(readerCache, readerFactories);

            return ctx.Read(source);
        }
    }
}
