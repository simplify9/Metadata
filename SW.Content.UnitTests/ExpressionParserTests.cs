using Microsoft.VisualStudio.TestTools.UnitTesting;
using SW.Content.Expressions;
using SW.Content.Serialization;
using System;
using System.Collections.Generic;
using System.Text;

namespace SW.Content.UnitTests
{
    [TestClass]
    public class ExpressionParserTests
    {
        void Test(string text, IContentExpression expected)
        {
            var parser = new ContentExpressionParser(new Tokenizer());

            var issues = parser.TryParse(text, out IContentExpression actual);

            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void Test_Parser()
        {
            var subObject = new CreateObjectExpression(new List<CreateObjectExpression.Element>
            {
                new CreateObjectExpression.Attribute
                {
                    Name = "subName",
                    Value = new PathExpression(new ScopeRootExpression(), ContentPath.Parse("$.name"))
                },

                new CreateObjectExpression.Attribute
                {
                    Name = "subAge",
                    Value = new ConstantExpression(new ContentNumber(45))
                }
            });

            Test("{ name: $.name, age: 45, ...{ subName: $.name, subAge: 45 } }", new CreateObjectExpression(new List<CreateObjectExpression.Element>
            {
                new CreateObjectExpression.Attribute
                {
                    Name = "name",
                    Value = new PathExpression(new ScopeRootExpression(), ContentPath.Parse("$.name"))
                },

                new CreateObjectExpression.Attribute
                {
                    Name = "age",
                    Value = new ConstantExpression(new ContentNumber(45))
                },

                new CreateObjectExpression.Object
                {
                    SubObject = subObject
                }
            }));

            Test("{}", new CreateObjectExpression(new CreateObjectExpression.Element[] { }));

            Test("[]", new CreateListExpression(new CreateListExpression.Element[] { }));

            Test("\"Yaser\"", new ConstantExpression(ContentFactory.Default.CreateFrom("Yaser")));

            Test("[45]", new CreateListExpression(new CreateListExpression.Element[]
            {
                new CreateListExpression.ListItem
                {
                    Value = new ConstantExpression(ContentFactory.Default.CreateFrom(45))
                }
            }));
        }
    }
}
