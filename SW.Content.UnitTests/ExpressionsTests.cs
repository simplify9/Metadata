using Microsoft.VisualStudio.TestTools.UnitTesting;
using SW.Content.Expressions;
using System;
using System.Collections.Generic;
using System.Text;

namespace SW.Content.UnitTests
{
    [TestClass]
    public class ExpressionsTests
    {
        public class R
        {
            public string name { get; set; }

            public int age { get; set; }
        }

        [TestMethod]
        public void Test_Expressions()
        {
            var scope = new LexicalScope(new ContentObject(new Dictionary<string, object>
            {
                { "name", "John" }
            }, ContentFactory.Default));

            var e = new CreateObjectExpression(new Dictionary<string, IContentExpression>
            {
                { "name", new ScopeVariableExpression(ContentPath.Parse("name")) },
                { "age", new ConstantExpression(new ContentNumber(45)) }
            });

            var issue = e.TryEvaluate(scope, out IContentNode result);

            Assert.IsNull(issue);

            var jToken = result.ToJson();

            var poco = jToken.ToObject<R>();

            Assert.AreEqual("John", poco.name);
            Assert.AreEqual(45, poco.age);
        }
    }
}
