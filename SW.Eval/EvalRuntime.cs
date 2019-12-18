using SW.Eval.Binding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SW.Eval
{
    public class EvalRuntime
    {
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

        public async Task<IPayload> GetResults(IEvalExpression query, IPayload input, EvalDataTaskFactory taskFactory)
        {
            // create eval context
            var ctx = new EvalContext(new LexicalScope("$", input));

            IEvalState result = null;

            // loop while expression has non-materialized data requests
            while ((result = query.GetState(ctx)) is EvalInProgress inProgress)
            {
                // group requests by func name
                var batchRequests = inProgress.ReadyToRun
                    .GroupBy(r => r.DataFuncName)
                    .Select(batch => new DataRequest(batch.Key, batch.SelectMany(item => item.Queries)));

                // run all
                var responses = await Task.WhenAll(batchRequests.Select(r => Decorate(taskFactory)(r)));

                // reflect response to context
                ctx = responses.SelectMany(rSet => rSet).Aggregate(ctx, (ctx_, rs) => ctx_.Materialize(rs));
            }

            // unwrap final expression result
            return (result as EvalComplete).Results.First();
        }
    }
}
