using Microsoft.VisualStudio.TestTools.UnitTesting;
using SW.Eval.Parser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SW.Eval.UnitTests
{
    [TestClass]
    public class EvalRuntimeTests
    {
        static readonly EvalRuntime evalRuntime = new EvalRuntime();

        static readonly EvalPayloadService payloadService = new EvalPayloadService();

        static Task<DataResponse[]> ServeParcelByNo(DataRequest rq)
        {
            
            var parcels = rq.Queries.Select(q => payloadService.CreatePayload(new
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
            var trackables = rq.Queries.Select(q => payloadService.CreatePayload(new
            {
                trackableNo = "123",
                containedIn = new[]
                {
                    new { number = "456" },
                    new { number = "789" },
                    new { number = "83508" },
                }
            }));

            return Task.FromResult(
                Enumerable.Zip(rq.Queries, trackables,
                    (q, rs) =>
                        new DataResponse(q.Id, rs)).ToArray());
        }

        static Task<DataResponse[]> ServeContainerByNo(DataRequest rq)
        {
            var responses = rq.Queries.Select(q => payloadService.CreatePayload(new
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
            var responses = rq.Queries.Select(q => payloadService.CreatePayload(new[]
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
            var expected = payloadService.CreatePayload(new
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

            // { --- }
            var obj = new EObject() 

                // ...Q:parcelByNo($.parcelNo),
                .FlatAppend(
                    new ECall("parcelByNo") 
                        .WithParam("no", new EPath(new EVar("$"), PayloadPath.Root.Append("parcelNo"))))

                // containedIn: Q:trackable($.parcelNo).containedIn.map(c => Q:containerByNo(c.number)),
                .Append("containedIn", 
                    new EMap(
                        new EPath(
                            new ECall("trackableByNo") 
                                .WithParam("no", new EPath(new EVar("$"), PayloadPath.Root.Append("parcelNo"))), 
                            PayloadPath.Root.Append("containedIn")), 
                        new DataFunc(
                            new ECall("containerByNo")
                                .WithParam("no", new EPath(new EVar("c"), PayloadPath.Root.Append("number"))),
                        "c")))

                 // references: Q: trackableRefs($.parcelNo)
                 .Append("references",
                    new ECall("trackableRefs")
                        .WithParam("no", new EPath(new EVar("$"), PayloadPath.Root.Append("parcelNo"))));
            
            var result = await evalRuntime.GetResults(obj, 
                payloadService.CreatePayload(new { parcelNo = "123" }), Serve);

            Assert.AreEqual(expected, result);

            
        }

        

        
    }
}
