using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;

namespace SW.Eval.UnitTests
{
    [TestClass]
    public class PayloadPathTests
    {
        [TestMethod]
        public void Test_PathToString()
        {
            Assert.AreEqual("$.a.b", PayloadPath.Root.Append("a").Append("b").ToString());

            Assert.AreEqual("$", PayloadPath.Root.ToString());

            var cPath = PayloadPath.Root.Append("a");
            Assert.AreEqual("$.a", cPath.ToString());

            cPath = PayloadPath.Root.Append("a");
            Assert.IsTrue(cPath.Equals(PayloadPath.Root.Append("a")));

            Assert.IsTrue(PayloadPath.Root.Equals(cPath.Sub(1)));
            Assert.IsFalse(PayloadPath.Root.Equals(cPath));
        }

        [TestMethod]
        public void Test_PathParsing()
        {
            Assert.IsFalse(PayloadPath.TryParse("", out PayloadPath _));
            Assert.IsFalse(PayloadPath.TryParse(".", out _));
            Assert.IsFalse(PayloadPath.TryParse(".ff", out _));
            Assert.IsFalse(PayloadPath.TryParse("3ff.", out _));

            var p1 = PayloadPath.Parse("$.a.FAERF_Ar");
            var p2 = PayloadPath.Root.Append("a").Append("FAERF_Ar");

            Assert.IsTrue(p1.Equals(p2));

            Assert.IsTrue(PayloadPath.Parse("$").Equals(PayloadPath.Root));


            Assert.IsTrue(p1.Equals(PayloadPath.Parse(p1.ToString())));
            Assert.IsTrue(PayloadPath.Root.Equals(PayloadPath.Parse(PayloadPath.Root.ToString())));
        }

    }
}
