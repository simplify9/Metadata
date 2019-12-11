using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SW.Eval.UnitTests
{
    [TestClass]
    public class EvalExpressionTests
    {
        static readonly EvalService evalService = new EvalService();

        static Task<DataResponse[]> ServeParcelByNo(DataRequest rq)
        {
            
            var parcels = rq.Queries.Select(q => evalService.CreatePayload(new
            {
                no = "123",
                shipper = "Yaser Awajan",
                consignee = "Samer Awajan"
            }));

            return Task.FromResult(Enumerable
                .Zip(rq.Queries, parcels, 
                    (q, rs) => 
                        new DataResponse(q.Id, rs)).ToArray());
        }

        static Task<DataResponse[]> ServeTrackableByNo(DataRequest rq)
        {
            var trackables = rq.Queries.Select(q => evalService.CreatePayload(new
            {
                trackableNo = "123",
                containedIn = new[]
                {
                    new { number = "456" },
                    new { number = "789" },
                    new { number = "83508" },
                }
            }));

            return Task.FromResult(Enumerable
                .Zip(rq.Queries, trackables,
                    (q, rs) =>
                        new DataResponse(q.Id, rs)).ToArray());
        }

        static Task<DataResponse[]> ServeContainerByNo(DataRequest rq)
        {
            var responses = rq.Queries.Select(q => evalService.CreatePayload(new
            {
                no = "123",
                type = "bag",
                isClosed = true
            }));

            return Task.FromResult(Enumerable
                .Zip(rq.Queries, responses,
                    (q, rs) =>
                        new DataResponse(q.Id, rs)).ToArray());
        }

        static Task<DataResponse[]> ServeTrackableRefs(DataRequest rq)
        {
            var responses = rq.Queries.Select(q => evalService.CreatePayload(new[]
            {
                "gaergaerG234234", "ag34gareg", "aggg3333", "1232"
            }));

            return Task.FromResult(Enumerable
                .Zip(rq.Queries, responses,
                    (q, rs) =>
                        new DataResponse(q.Id, rs)).ToArray());
        }

        static Task<DataResponse[]> Serve(DataRequest rq)
        {
            var queryCatalog = new Dictionary<string, EvalDataTaskFactory>
            {
                { "parcelByNo", ServeParcelByNo },
                { "trackableByNo", ServeTrackableByNo },
                { "containerByNo", ServeContainerByNo },
                { "trackableRefs", ServeTrackableRefs },
            };

            if (!queryCatalog.TryGetValue(rq.DataFuncName, out EvalDataTaskFactory f))
            {
                throw new NotSupportedException($"Could not resolve handler for data func '{rq.DataFuncName}'");
            }

            return f(rq);
        }
        


        [TestMethod]
        public async Task Test_Expressions()
        {
            var expected = evalService.CreatePayload(new
            {
                no = "123",
                shipper = "Yaser Awajan",
                consignee = "Samer Awajan",
                containedIn = new[]
                {
                    new {
                        no = "123",
                        type = "bag",
                        isClosed = true
                    },
                    new {
                        no = "123",
                        type = "bag",
                        isClosed = true
                    },
                    new {
                        no = "123",
                        type = "bag",
                        isClosed = true
                    }
                },
                references = new[] { "gaergaerG234234", "ag34gareg", "aggg3333", "1232" }
            });

            var obj = new EObject() // { --- }

                // ...Q:parcelByNo($.parcelNo),
                .FlatAppend(
                    new EDataFunc("parcelByNo") 
                        .WithParam("no", new EPath(new EVar("$"), PayloadPath.Root.Append("parcelNo"))))

                // containedIn: Q:trackable($.parcelNo).containedIn.map(c => Q:containerByNo(c.number)),
                .Append("containedIn", 
                    new EMap(
                        new EPath(
                            new EDataFunc("trackableByNo") 
                                .WithParam("no", new EPath(new EVar("$"), PayloadPath.Root.Append("parcelNo"))), 
                            PayloadPath.Root.Append("containedIn")), 
                        new EDataFunc("containerByNo")
                            .WithParam("no", new EPath(new EVar("c"), PayloadPath.Root.Append("number"))),
                        "c"))

                 // references: Q: trackableRefs($.parcelNo)
                 .Append("references",
                    new EDataFunc("trackableRefs")
                        .WithParam("no", new EPath(new EVar("$"), PayloadPath.Root.Append("parcelNo"))));
            
            var result = await evalService.GetQueryResults(obj, evalService.CreatePayload(new { parcelNo = "123" }), Serve);

            Assert.AreEqual(expected, result);
        }
    }
}
