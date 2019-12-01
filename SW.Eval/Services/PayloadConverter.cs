using SW.Eval.Binding;
using SW.Eval.Binding.StandardTypes;
using SW.Eval.Binding.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SW.Eval
{
    public class PayloadConverter
    {
        static readonly DelegateCache cache = new DelegateCache();

        readonly IPayloadTypeConverterFactory[] converterFactories;

        IEnumerable<IPayloadTypeConverterFactory> SortMergeFactories(IEnumerable<IPayloadTypeConverterFactory> additional)
        {
            foreach (var e in additional) yield return e;

            yield return new ToPayload();

            yield return new ToString();

            //yield return new ToEnum();

            yield return new ToDateTime();

            yield return new ToStruct();

            yield return new ToDictionary();

            yield return new ToEnumerable();

            yield return new ToClass();
        }

        public PayloadConverter(IEnumerable<IPayloadTypeConverterFactory> factories = null)
        {
            this.converterFactories = SortMergeFactories(factories ??
                Array.Empty<IPayloadTypeConverterFactory>()).ToArray();
        }

        public string TryConvert<T>(IPayload source, out T converted)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));

            converted = default;

            if (source is INull) return null;

            var ctx = new PayloadConversionContext(cache, converterFactories);

            var r = ctx.Convert<T>(source);

            converted = r.Value;

            return r is IPayloadError<T> badPayload
                ? badPayload.Error
                : null;
        }
    }
}
