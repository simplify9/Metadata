using Microsoft.VisualStudio.TestTools.UnitTesting;
using SW.Eval.Core.Expressions.Parsing.Experimental;
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

            return Task.FromResult(
                Enumerable.Zip(rq.Queries, trackables,
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
                        new ExpressionClosure(
                            new ECall("containerByNo")
                                .WithParam("no", new EPath(new EVar("c"), PayloadPath.Root.Append("number"))),
                        "c")))

                 // references: Q: trackableRefs($.parcelNo)
                 .Append("references",
                    new ECall("trackableRefs")
                        .WithParam("no", new EPath(new EVar("$"), PayloadPath.Root.Append("parcelNo"))));
            
            var result = await evalService.GetQueryResults(obj, 
                evalService.CreatePayload(new { parcelNo = "123" }), Serve);

            Assert.AreEqual(expected, result);

            

            //Assert.IsTrue(parsedClosure.IsSuccessful);

        }

        static void AssertClosureMustSucceed<TBody>(string expr)
        {
            var c1 = ExpressionParsers.ParseFunc(expr);
            Assert.IsTrue(c1.IsSuccessful);
            Assert.AreEqual(string.Empty, c1.Remaining);
            Assert.IsInstanceOfType(c1.Value.Body, typeof(TBody));
        }

        static void AssertExpressionMustSucceed<T>(string expr)
        {
            var c1 = ExpressionParsers.ParseExpression(expr);
            Assert.IsTrue(c1.IsSuccessful);
            Assert.AreEqual(string.Empty, c1.Remaining);
            Assert.IsInstanceOfType(c1.Value, typeof(T));
        }

        static void AssertExpressionMustFail(string expr)
        {
            var r = ExpressionParsers.ParseExpression(expr);
            Assert.AreNotEqual(string.Empty, r.Remaining);
        }

        [TestMethod]
        public void Test_Parsers()
        {
            
            AssertClosureMustSucceed<EObject>("$ => { }");
            AssertClosureMustSucceed<EObject>("($) => {}   ");
            AssertClosureMustSucceed<EObject>("   (p1, p2) => {}");

            AssertExpressionMustSucceed<EConstant>("\"\"");
            AssertExpressionMustSucceed<EConstant>("\"yyyy\"");
            AssertExpressionMustSucceed<EConstant>("-54");
            AssertExpressionMustSucceed<EConstant>("-54.0");
            AssertExpressionMustSucceed<EConstant>("false");
            AssertExpressionMustSucceed<EConstant>("true");

            AssertExpressionMustSucceed<EArray>("[]");
            AssertExpressionMustSucceed<EConstant>("false");

            AssertExpressionMustSucceed<EVar>("sss");

            AssertExpressionMustSucceed<EObject>("{  }");
            AssertExpressionMustSucceed<EObject>("{ a:243 }");
            AssertExpressionMustSucceed<EPath>("$.ffff");
            AssertExpressionMustSucceed<ECall>("a(b:$.ffff)");
            AssertExpressionMustSucceed<ECall>("a(b:$.ffff, c:t())");
            AssertExpressionMustSucceed<ECall>("a(b:$.ffff, c:t(r:rr))");
            AssertExpressionMustSucceed<ECall>("a(b:$.ffff, c:(t(g:rr)))");
            AssertExpressionMustSucceed<ECall>("(a(b:$.ffff, c:(t(d:rr))))");

            AssertExpressionMustFail("a(, c:$.ffff)");
            
            AssertExpressionMustSucceed<ECall>("a(b:$.ffff, c:\"44\")");
            AssertExpressionMustFail("a(b:$.ffff, c:\"44\"");

            AssertExpressionMustSucceed<EObject>("{ ...{ a: 5, c:3 }, v:2 }");
            AssertExpressionMustSucceed<EObject>("{ ...b( a: 5, c:3 ), v:2 }");

            AssertExpressionMustSucceed<EAnd>("l && r");
            AssertExpressionMustSucceed<EAnd>("l && r && (a || b)");
            AssertExpressionMustSucceed<EPath>("r.eeet.mag");
            AssertExpressionMustSucceed<EFilter>("r.eeet.map(c => c).filter(c => c)");

            var closureCode = @"
$ => {
    ...parcelByNo(no:$.parcelNo),
    containedIn: trackable(no:$.parcelNo).containedIn.map(c => containerByNo(no: c.number)),
    references: trackableRefs(no: $.parcelNo)
}";

            AssertClosureMustSucceed<EObject>(closureCode);

            var parsedClosure = ExpressionParsers.ParseFunc(closureCode);


        }
    }
}
