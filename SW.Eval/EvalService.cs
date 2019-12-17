using SW.Eval.Binding;
using SW.Eval.Binding.Internal;
using SW.Eval.Binding.StandardTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SW.Eval
{
    public class EvalService
    {
        static readonly DelegateCache readerCache = new DelegateCache();
        static readonly DelegateCache convertCache = new DelegateCache();
        readonly IPayloadTypeReaderFactory[] readerFactories;
        readonly IPayloadTypeConverterFactory[] converterFactories;

        static EvalDataTaskFactory Decorate(EvalDataTaskFactory taskFactory)
            => async (r) =>
            {
                try
                {
                    var responses = await taskFactory(r);
                    return r.Queries.Select(q =>
                        responses.FirstOrDefault(rs => rs.RequestId == q.Id) ??
                            new DataResponse(q.Id, NoPayload.Singleton)).ToArray();
                }
                catch (Exception ex)
                {
                    var errorPayload = new PayloadError(ex);
                    return r.Queries.Select(q => new DataResponse(q.Id, errorPayload)).ToArray();
                }
            };
        

        static IEnumerable<IPayloadTypeConverterFactory> SortMergeConverters(IEnumerable<IPayloadTypeConverterFactory> additional)
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

        static IEnumerable<IPayloadTypeReaderFactory> SortMergeReaders(IEnumerable<IPayloadTypeReaderFactory> additionalReaders)
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

        public IPayload CreatePayload<T>(T source)
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

        public async Task<IPayload> GetQueryResults(IEvalExpression query, IPayload input, EvalDataTaskFactory taskFactory)
        {
            // create eval context
            var ctx = new EvalContext(new LexicalScope("$", input));

            IEvalState result = null;

            // loop while expression has non-materialized data requests
            while ((result = query.Run(ctx)) is EvalInProgress inProgress)
            {
                // group requests by func name
                var batchRequests = inProgress.ReadyToRun
                    .GroupBy(r => r.DataFuncName)
                    .Select(batch =>
                        new DataRequest(batch.Key,
                            batch.SelectMany(item => item.Queries)));

                // run all
                var responses = await Task.WhenAll(
                    batchRequests.Select(r => 
                        Decorate(taskFactory)(r)));

                // reflect response to context
                foreach (var response in responses.SelectMany(rSet => rSet))
                {
                    ctx = ctx.Materialize(response);
                }
            }

            // unwrap final expression result
            return (result as EvalComplete).Results.First();
        }
    }
}
