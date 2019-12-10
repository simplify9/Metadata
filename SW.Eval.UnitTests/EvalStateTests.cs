using Microsoft.VisualStudio.TestTools.UnitTesting;
using SW.Eval.Binding;
using System;
using System.Collections.Generic;
using System.Text;

namespace SW.Eval.UnitTests
{
    [TestClass]
    public class EvalStateTests
    {
        [TestMethod]
        public void Test_EvalStates()
        {
            IEvalState s1 = new EvalComplete(new PayloadPrimitive<string>("Hello"));
            IEvalState s2 = new EvalInProgress(new DataRequest("test", "~/0", 
                new EvalQueryArgs(
                    new Dictionary<string, IPayload> { { "a", new PayloadPrimitive<int>(5) } })));
            IEvalState s3 = new EvalComplete(new PayloadPrimitive<string>("World"));
            IEvalState s4 = new EvalInProgress(new DataRequest("test2", "~/1",
                new EvalQueryArgs(
                    new Dictionary<string, IPayload> { { "a", new PayloadPrimitive<int>(5) } })));
            IEvalState s5 = new EvalComplete(
                new PayloadPrimitive<string>("Hello1"),
                new PayloadPrimitive<string>("Hello2"));
            IEvalState s6 = new EvalInProgress(
                new[]
                {
                    new DataRequest("fasdfa", "24444", 
                        new EvalQueryArgs(
                            new Dictionary<string, IPayload> { { "a", new PayloadPrimitive<int>(5) } })),
                    new DataRequest("fasdfa", "24444",
                        new EvalQueryArgs(
                            new Dictionary<string, IPayload> { { "a", new PayloadPrimitive<int>(5) } })),
                });

            var combined = s1.MergeWith(s2);
            Assert.IsInstanceOfType(combined, typeof(EvalInProgress));
            Assert.AreEqual(1, (combined as EvalInProgress).ReadyToRun.Length);

            combined = s1.MergeWith(s3);
            Assert.IsInstanceOfType(combined, typeof(EvalComplete));
            Assert.AreEqual(2, (combined as EvalComplete).Results.Length);

            combined = s1.MergeWith(s2).MergeWith(s3).MergeWith(s4);
            Assert.IsInstanceOfType(combined, typeof(EvalInProgress));
            Assert.AreEqual(2, (combined as EvalInProgress).ReadyToRun.Length);

            combined = s1.MergeWith(s3).MergeWith(s5);
            Assert.IsInstanceOfType(combined, typeof(EvalComplete));
            Assert.AreEqual(4, (combined as EvalComplete).Results.Length);

            combined = s1.MergeWith(s2).MergeWith(s3).MergeWith(s4).MergeWith(s5).MergeWith(s6);
            Assert.IsInstanceOfType(combined, typeof(EvalInProgress));
            Assert.AreEqual(4, (combined as EvalInProgress).ReadyToRun.Length);
        }
    }
}
