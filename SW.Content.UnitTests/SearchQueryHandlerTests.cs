using Microsoft.VisualStudio.TestTools.UnitTesting;
using SW.Content.Search;
using SW.Content.Search.EF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SW.Content.UnitTests
{
    [TestClass]
    public class SearchQueryHandlerTests
    {
        [TestMethod]
        public void Test_QueryHandlerAnyOf()
        {
            var list = ContentFactory.Default.CreateFrom(new[] { "123", "456" });

            var a = (list as ContentList).ToArray();
            Assert.IsInstanceOfType(a[0], typeof(ContentText));

            Assert.IsInstanceOfType(a[1], typeof(ContentText));

            var line = new SearchQuery.Line(ContentPath.Parse("$.Number.Value"), SearchQuery.Op.AnyOf, list);

            var expr = QueryConversionHelper.BuildCriteriaExpr(line);

        }
    }
}
