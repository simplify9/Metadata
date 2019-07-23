using Microsoft.VisualStudio.TestTools.UnitTesting;
using SW.Content.Search.EF;
using System;
using System.Collections.Generic;
using System.Text;

namespace SW.Content.UnitTests
{ 
    [TestClass]
    public class JsonUtilTests
    {
        [TestMethod]
        public void Test_Serializer()
        {
            var m = new[] { "John", "Scotty" };

            Assert.AreEqual(m.Length, JsonUtil.Deserialize<string[]>(JsonUtil.Serialize(m)).Length);
        }
    }
}
