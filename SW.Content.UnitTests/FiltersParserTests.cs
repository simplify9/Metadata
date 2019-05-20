using Microsoft.VisualStudio.TestTools.UnitTesting;
using SW.Content.Filters;
using SW.Content.Filters.Parser;
using System;
using System.Collections.Generic;
using System.Text;

namespace SW.Content.UnitTests
{
    [TestClass]
    public class FiltersParserTests
    {
        readonly ContentFilterParser _parser = new ContentFilterParser(new Tokenizer());

        [TestMethod]
        public void Test_Parser()
        {
            var left = new AndFilter(
                new EqualToFilter(
                    ContentPath.Parse("Name"),
                    new ContentText("Ahmad contains")),
                new ContainsFilter(
                    ContentPath.Parse("Friends"),
                    new ContentText("Sameer")));
            var issues = _parser.TryParse("Name EQUALS \"Ahmad contains\" AND Friends CONTAINS \"Sameer\"", out IContentFilter actual);
            Assert.AreEqual(left, actual);


            var right = new AndFilter(
                new EqualToFilter(
                    ContentPath.Parse("Name"),
                    new ContentText("Suzi 34")),
                new ContainsFilter(
                    ContentPath.Parse("Friends"),
                    new ContentText("Sameer")));

            issues = _parser.TryParse(
                "(Name EQUALS \"Ahmad contains\" AND Friends CONTAINS \"Sameer\")" +
                " OR " +
                "(Name EQUALS \"Suzi 34\" AND Friends CONTAINS \"Sameer\")",
                out actual);

            var combined = new OrFilter(left, right);

            Assert.AreEqual(combined, actual);
        }
    }
}
