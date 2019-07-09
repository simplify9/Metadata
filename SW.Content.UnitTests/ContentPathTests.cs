using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;

namespace SW.Content.UnitTests
{
    [TestClass]
    public class ContentPathTests
    {
        [TestMethod]
        public void Test_ContentPathToString()
        {
            Assert.AreEqual("$.a.b", ContentPath.Root.Append("a").Append("b").ToString());

            Assert.AreEqual("$", ContentPath.Root.ToString());

            var cPath = ContentPath.Root.Append("a");
            Assert.AreEqual("$.a", cPath.ToString());

            cPath = ContentPath.Root.Append("a");
            Assert.IsTrue(cPath.Equals(ContentPath.Root.Append("a")));

            Assert.IsTrue(ContentPath.Root.Equals(cPath.Sub(1)));
            Assert.IsFalse(ContentPath.Root.Equals(cPath));
        }

        [TestMethod]
        public void Test_ContentPathParsing()
        {
            Assert.IsFalse(ContentPath.TryParse("", out ContentPath _));
            Assert.IsFalse(ContentPath.TryParse(".", out _));
            Assert.IsFalse(ContentPath.TryParse(".ff", out _));
            Assert.IsFalse(ContentPath.TryParse("3ff.", out _));

            var p1 = ContentPath.Parse("$.a.FAERF_Ar");
            var p2 = ContentPath.Root.Append("a").Append("FAERF_Ar");

            Assert.IsTrue(p1.Equals(p2));

            Assert.IsTrue(ContentPath.Parse("$").Equals(ContentPath.Root));


            Assert.IsTrue(p1.Equals(ContentPath.Parse(p1.ToString())));
            Assert.IsTrue(ContentPath.Root.Equals(ContentPath.Parse(ContentPath.Root.ToString())));
        }

    }
}
