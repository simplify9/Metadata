using Microsoft.VisualStudio.TestTools.UnitTesting;
using SW.Content.Expressions;
using SW.Content.Filters;
using SW.Content.Serialization;
using System;
using System.Collections.Generic;
using System.Text;

namespace SW.Content.UnitTests
{
    [TestClass]
    public class FiltersParserTests
    {
        readonly ContentExpressionParser _parser = new ContentExpressionParser(new Tokenizer());

        [TestMethod]
        public void Test_Parser()
        {
            var left = new AndFilter(
                new EqualToFilter(
                    new PathExpression(new ScopeRootExpression(), ContentPath.Parse("$.Name")),
                    new ConstantExpression(new ContentText("Ahmad contains"))),
                new ContainsFilter(
                    new PathExpression(new ScopeRootExpression(), ContentPath.Parse("$.Friends")),
                    new ConstantExpression(new ContentText("Sameer"))));
            var issues = _parser.TryParse("$.Name EQUALS \"Ahmad contains\" AND $.Friends CONTAINS \"Sameer\"", out IContentExpression actual);
            
            Assert.AreEqual(left, actual);


            var right = new AndFilter(
                new EqualToFilter(
                    new PathExpression(new ScopeRootExpression(), ContentPath.Parse("$.Name")),
                    new ConstantExpression(new ContentText("Suzi 34"))),
                new ContainsFilter(
                    new PathExpression(new ScopeRootExpression(), ContentPath.Parse("$.Friends")),
                    new ConstantExpression(new ContentText("Sameer"))));

            issues = _parser.TryParse(
                "($.Name EQUALS \"Ahmad contains\" AND $.Friends CONTAINS \"Sameer\")" +
                " OR " +
                "($.Name EQUALS \"Suzi 34\" AND $.Friends CONTAINS \"Sameer\")",
                out actual);

            var combined = new OrFilter(left, right);

            Assert.AreEqual(combined, actual);
        }
    }
}
