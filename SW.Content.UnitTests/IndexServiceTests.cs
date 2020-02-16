using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SW.Content.Search;
using SW.Content.Search.EF;
using SW.Content.UnitTests.models;
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
        readonly ILogger<IndexDbRepo> _logger;

        public IndexServiceTests()
        {
            _logger = (new LoggerFactory()).CreateLogger<IndexDbRepo>();

        }
        class MockRepo : IIndexRepo
        {

            public Document[] Tokens { get; set; }

            public Task DeleteDocuments(DocumentSource[] sources)
            {
                throw new NotImplementedException();
            }

            public Task UpdateDocuments(Document[] events)
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
            

           //Assert.AreEqual(3, r.Tokens.Count(t => t.SourcePath.Path.ToString() == "$.Phones"));
            
        }

        [TestMethod]
        public async Task Test_IndexServiceNeverDuplicatesPathMasterRecords()
        {
            var _connection = new SqliteConnection("DataSource=:memory:");
            _connection.Open();
            var options = new DbContextOptionsBuilder<DbCtxt>()
                .UseSqlite(_connection)
                .EnableSensitiveDataLogging(true)
                .Options;
            var dbc = new DbCtxt(options);
            await dbc.Database.EnsureCreatedAsync();

            var r = new IndexDbRepo(dbc,_logger);
            var indexService = new IndexService(r);
         
            var container = new ContainerDto
            {
                ContainerNumber = new ContainerNumberDto()
                {
                    Number = "bcdefghijklmnopqrstuvwxyabcdefghijklmnopqrstuvwxz :123456789012345678901234567890 12345678901234567890 12345678901234567890123456789012bcdefghijklmnopqrst uvwxyabcdefghijklmnopqrstuvwxz:12345678901234 567890123456789012345678 901234567890123456 78901234567890123456789012"
                },
                AssignedTo = new ContainerDto.CarrierDto { Code = "aby", Name = "name", phones = new string[] { "bcdefghijklmnopqrstuvwxyabcdefghijklmnopqrstuvwxz:1234567890123456789012345678901234567890123456789012345678901234567890123456789012bcdefghijklmnopqrstuvwxyabcdefghijklmnopqrstuvwxz:1234567890123456789012345678901234567890123456789012345678901234567890123456789012" } }
            };
            var cmd = indexService.CreateUpdateCommand("0001", container);
            await indexService.Handle(cmd);

            var attchs = new List<AttachmentDto>();
            attchs.Add(new AttachmentDto
            {
                Type = "invoice",
                DownloadUrl = "hdhjdhjhsa"
            });
            container.Attachments = attchs.ToArray();
            var cmd2 = indexService.CreateUpdateCommand("0001", container);
            await indexService.Handle(cmd2);

            attchs.Add(new AttachmentDto
            {
                Type = "invoice3",
                DownloadUrl = "hdhjdhjhsa3"
            });
            container.Attachments = attchs.ToArray();
            var cmd3 = indexService.CreateUpdateCommand("0001", container);
            await indexService.Handle(cmd3);
            var paths = dbc.Set<DbDocSourcePath>().Select(p => p).Where(p=>p.PathString == "$.Attachments.[].DownloadUrl").ToArray();
            var tokens = dbc.Set<DbDocToken>().Select(t => t).ToArray();


            // load paths from database
            //var paths = await dbc.Set<DbDocSourcePath>().Where(p=>p.PathString == "$.Attachments.[].DownloadUrl").ToArrayAsync();
            Assert.AreEqual(1, paths.Length);

        }


    }

  
   



   
   



}
