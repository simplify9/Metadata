using Microsoft.VisualStudio.TestTools.UnitTesting;
using SW.Eval.Binding;
using System;
using System.Collections.Generic;
using System.Text;

namespace SW.Eval.UnitTests
{
    [TestClass]
    public class EvalContextTests
    {
        [TestMethod]
        public void Test_LexicalScope()
        {
            var s = new LexicalScope("$", new PayloadPrimitive<int>(7));
            Assert.IsInstanceOfType(s.GetValue("ggg"), typeof(INoPayload));
            Assert.AreEqual(s.GetValue("$"), new PayloadPrimitive<int>(7));

            s = s.SetVariable("ggg", new PayloadPrimitive<int>(9));
            Assert.AreEqual(new PayloadPrimitive<int>(9), s.GetValue("ggg"));
            Assert.AreEqual(new PayloadPrimitive<int>(7), s.GetValue("$"));
            Assert.IsInstanceOfType(s.GetValue("fff"), typeof(INoPayload));
        }

        [TestMethod]
        public void Test_EvalContext()
        {
            var s = new LexicalScope("$", new PayloadPrimitive<int>(7));
            var ctx = new EvalContext(s);
            Assert.IsNull(ctx.Parent);
            Assert.AreEqual("~", ctx.Id);
            Assert.AreEqual("~/sub", ctx.CreateSub("sub").Id);
            Assert.AreEqual("~/sub/5", ctx.CreateSub("sub").CreateSub("5").Id);
            
        }
    }
}
