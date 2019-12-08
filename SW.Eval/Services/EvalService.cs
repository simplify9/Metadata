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
    public class EvalService
    {
        
        static readonly DelegateCache readerCache = new DelegateCache();

        static readonly DelegateCache convertCache = new DelegateCache();

        readonly IPayloadTypeReaderFactory[] readerFactories;

        readonly IPayloadTypeConverterFactory[] converterFactories;

        IEnumerable<IPayloadTypeConverterFactory> SortMergeConverters(IEnumerable<IPayloadTypeConverterFactory> additional)
        {
            foreach (var e in additional) yield return e;
            yield return new ToPayload();
            yield return new ToString();
            yield return new ToDateTime();
            yield return new ToStruct();
            yield return new ToDictionary();
            yield return new ToEnumerable();
            yield return new ToClass();
        }

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

        public EvalService(
            IEnumerable<IPayloadTypeReaderFactory> readerFactories = null,
            IEnumerable<IPayloadTypeConverterFactory> converterFactories = null)
        {
            this.readerFactories = SortMergeReaders(readerFactories ??
                Array.Empty<IPayloadTypeReaderFactory>()).ToArray();

            this.converterFactories = SortMergeConverters(converterFactories ??
                Array.Empty<IPayloadTypeConverterFactory>()).ToArray();
        }

        public IPayload Read<T>(T source)
        {
            if (source == null) return PayloadNull.Singleton;
            var ctx = PayloadReaderContext.CreateRoot(readerCache, readerFactories);
            return ctx.Read(source);
        }

        public string TryConvert<T>(IPayload source, out T converted)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));

            converted = default;
            if (source is INull) return null;
            var ctx = new PayloadConversionContext(convertCache, converterFactories);
            var r = ctx.Convert<T>(source);
            converted = r.Value;
            return r is IPayloadError<T> badPayload
                ? string.Join(",\n\r", badPayload.Errors)
                : null;
        }
    }
}
