using Microsoft.VisualStudio.TestTools.UnitTesting;
using SW.Content.Search;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SW.Content.UnitTests
{
    [TestClass]
    public class TokenHelperTests
    {
        [TestMethod]
        public void GetTokensInitial()
        {
            var emp = new Employee
            {
                Phones = new[] { "899", "635", "7877" },
                Name = "yaser",
                EndDate = DateTime.UtcNow,
                Id = 2,
                ContractType = EmploymentType.Permenant

            };
            var stringPathes = new string[] { "$.Phones", "$.Name", "$.EndDate", "$.Id", "$.ContractType" };

            var tokens = TokenHelper.GetTokens(new DocumentSource(new DocumentType(emp.GetType()), ContentFactory.Default.CreateFrom($"{emp.ContractType.ToString()}:{emp.Id}")), emp);

            Assert.AreEqual(7, tokens.Count());
            foreach(var t in tokens)
            {
                Assert.IsTrue(stringPathes.Any(p => p == t.SourcePath.Path.ToString()));
            }
        }

        [TestMethod]
        public void ArrayTest()
        {
            var emp = new Employee
            {
                Phones = new[] { "899", "635", "7877" },
               

            };
            var stringPathes = new string[] { "$.Phones","$.Id"};

            var tokens = TokenHelper.GetTokens(new DocumentSource(new DocumentType(emp.GetType()), ContentFactory.Default.CreateFrom($"{emp.ContractType.ToString()}:{emp.Id}")), emp);

            Assert.AreEqual(4, tokens.Count());
            foreach (var t in tokens)
            {
                Assert.IsTrue(stringPathes.Any(p => p == t.SourcePath.Path.ToString()));
            }
            var phones = tokens.Where(t => t.SourcePath.Path.ToString() == "$.Phones");
            Assert.AreEqual(3, phones.Count());

        }

        [TestMethod]
        public void LongProperties()
        {
            var emp = new Employee
            {
               Name= @"1Zjh5HK5tcNGucysZjyUBEceqjq8rw0cyBXhiU swDbrtAGdBTeRBFS6XS0tqNIF9Gnm9ikhslHy3Ii MzGc3eKscWlHaOtxJM7LXb9WWj2hLqBOPmMu5CuAk rQIrzbrz7sp7o0gvFdGT8yyfnFL0PI2AXT1QdlYXg hCBlQgfttFMaOJkj7iArUrOh4lgbmq32BeaQ9j5MI"


            };
            var stringPathes = new string[] { "$.Name","$.Id"};

            var tokens = TokenHelper.GetTokens(new DocumentSource(new DocumentType(emp.GetType()), ContentFactory.Default.CreateFrom($"{emp.ContractType.ToString()}:{emp.Id}")), emp);

            Assert.AreEqual(6, tokens.Count());
            foreach (var t in tokens)
            {
                Assert.IsTrue(stringPathes.Any(p => p == t.SourcePath.Path.ToString()));
            }
            var name = tokens.Where(t => t.SourcePath.Path.ToString() == "$.Name");
            Assert.AreEqual(5, name.Count());

        }
    }
}
