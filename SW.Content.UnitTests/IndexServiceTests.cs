using Microsoft.VisualStudio.TestTools.UnitTesting;
using SW.Content.Search;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SW.Content.UnitTests
{
    [TestClass]
    public class IndexServiceTests
    {
        class MockRepo : IIndexRepo
        {
            public DocumentToken[] Tokens { get; set; }

            public Task DeleteDocuments(DocumentSource[] sources)
            {
                throw new NotImplementedException();
            }

            public Task SaveTokens(DocumentToken[] events)
            {
                Tokens = events;
                return Task.CompletedTask;
            }
        }

        [TestMethod]
        public async Task Test_IndexService()
        {
            var r = new MockRepo();
            var indexService = new IndexService(r);
            var emp = new Employee
            {
                Phones = new[] {"123", "456", "789" }
            };

            var cmd = indexService.CreateUpdateCommand(5, emp);

            await indexService.Handle(cmd);

            Assert.AreEqual(3, r.Tokens.Count(t => t.SourcePath.Path.ToString() == "$.Phones.[]"));
            
        }


    }
}
