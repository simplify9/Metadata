using Microsoft.VisualStudio.TestTools.UnitTesting;
using SW.Eval.Parser;
using System;
using System.Collections.Generic;
using System.Text;

namespace SW.Eval.UnitTests
{
    [TestClass]
    public class EvalParserTests
    {
        static readonly EvalParser parser = new EvalParser();

        void AssertClosureMustSucceed<TBody>(string expr)
        {
            var c1 = parser.ParseFunc(expr);
            Assert.IsTrue(c1.IsSuccessful);
            Assert.AreEqual(string.Empty, c1.Remaining);
            Assert.IsInstanceOfType(c1.Value.Body, typeof(TBody));
        }

        void AssertExpressionMustSucceed<T>(string expr)
        {
            var c1 = parser.ParseExpression(expr);
            Assert.IsTrue(c1.IsSuccessful);
            Assert.AreEqual(string.Empty, c1.Remaining);
            Assert.IsInstanceOfType(c1.Value, typeof(T));
        }

        void AssertExpressionMustFail(string expr)
        {
            var r = parser.ParseExpression(expr);
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

        }
    }
}
