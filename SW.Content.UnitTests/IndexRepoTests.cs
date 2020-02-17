using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;
using SW.Content.Search;
using SW.Content.Search.EF;
using SW.Content.UnitTests.models;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SW.Content.UnitTests
{
    [TestClass]
    public class IndexRepoTests
    {
        readonly DbConnection _connection;
        readonly ILogger<IndexDbRepo> _logger;
        
        public IndexRepoTests()
        {
            _logger = (new LoggerFactory()).CreateLogger<IndexDbRepo>();
            _connection = new SqliteConnection("DataSource=:memory:");
            _connection.Open();
        }

        private async Task<DbContext> GetDbContext()
        {

            var options = new DbContextOptionsBuilder<DbCtxt>()
                .UseSqlite(_connection)
                .EnableSensitiveDataLogging(true)
                .Options;
            
            var dbc = new DbCtxt(options);
            await dbc.Database.EnsureCreatedAsync();
            return dbc;
        }
        [TestMethod]
        public async Task InitialTest()
        {
            var _dbc = await GetDbContext();
            var repo = new IndexDbRepo(_dbc,_logger);
          
            var emps = new List<Employee>() {
                new Employee
            {
                Phones = new[] { "123", "456", "789" },
                Name = "layan",
                EndDate = DateTime.UtcNow,
                Id=1,
                ContractType = EmploymentType.Permenant,
                Contracts = new AccountContractDto[]
                {
                    new AccountContractDto
                    { ChargeType="Dd",
                       InActive = true,
                       Product ="sttd",
                       Ratesheet ="dhdjh"
                        
                    },
                    new AccountContractDto
                    { ChargeType="Dd",
                       InActive = true,
                       Product ="sttd",
                       Ratesheet ="dhdjh"

                    },new AccountContractDto
                    { ChargeType="Dd",
                       InActive = true,
                       Product ="sttd",
                       Ratesheet ="dhdjh"

                    }


                }
              

            }, new Employee
            {
                Phones = new[] { "899", "635" },
                Name = "yaser",
                EndDate = DateTime.UtcNow,
                Id=2,
                ContractType = EmploymentType.Permenant

            }
           };

            var docs = emps.Select(e => new Document()
            {
                Data = e,
                Source = new DocumentSource(new DocumentType(e.GetType()), ContentFactory.Default.CreateFrom($"{e.ContractType.ToString()}:{e.Id}"))
            });

            await repo.UpdateDocuments(docs.ToArray());

            var emps2 = emps.ToArray();
            emps2[0].Name = "layan khater";
            emps[1].Name = "samer";
            emps2[0].Phones = new string[0];
            emps2[0].Contracts = new AccountContractDto[0];
            emps2[0].Contracts = new AccountContractDto[0];
            emps2[1].Phones = new string[] { "899" };

          
            await repo.UpdateDocuments(docs.ToArray());

            _dbc = await GetDbContext();
            
            var docs3 = await _dbc.Set<DbDoc>()
                .Include(d => d.Tokens)
                .ThenInclude(d => d.Path)
                .ToArrayAsync();
     

            foreach (var d in docs3)
            {
                Assert.AreEqual(emps[(int)d.Id - 1].Name, d.Tokens.First(t => t.Path.PathString == "$.Name").ValueAsAny);
                Assert.AreEqual(emps[(int)d.Id - 1].Phones.Length, d.Tokens.Where(t => t.Path.PathString == "$.Phones").Count());
                Assert.AreEqual(JsonUtil.Serialize(emps[(int)d.Id - 1]), d.BodyData);
            }
        }

        [TestMethod]
        public async Task UpdateArrayTest()
        {
            var _dbc = await GetDbContext();
            var repo = new IndexDbRepo(_dbc, _logger);

            var emps = new List<Employee>() {
                new Employee
            {
                Phones = new[] { "123" },
                Name = "layan",
                EndDate = DateTime.UtcNow,
                Id=1,
                ContractType = EmploymentType.Permenant

            }, new Employee
            {
                Phones = new[] { "899", "635", "7877" ,"98898" ,"09090"},
                Name = "yaser",
                EndDate = DateTime.UtcNow,
                Id=2,
                ContractType = EmploymentType.Permenant

            }
           };

            var docs = emps.Select(e => new Document()
            {
                Data = e,
                Source = new DocumentSource(new DocumentType(e.GetType()), ContentFactory.Default.CreateFrom($"{e.ContractType.ToString()}:{e.Id}"))
            });

            await repo.UpdateDocuments(docs.ToArray());

            var emps2 = emps.ToArray();
            emps2[0].Name = "layan khater";
            emps[1].Name = "samer";
            emps2[0].Phones = new string[0];


            await repo.UpdateDocuments(docs.ToArray());

            _dbc = await GetDbContext();

            var docs3 = await _dbc.Set<DbDoc>()
                .Include(d => d.Tokens)
                .ThenInclude(d => d.Path)
                .ToArrayAsync();
            foreach (var d in docs3)
            {
                Assert.AreEqual(emps[(int)d.Id - 1].Name, d.Tokens.First(t => t.Path.PathString == "$.Name").ValueAsAny);
                Assert.AreEqual(emps[(int)d.Id - 1].Phones.Length, d.Tokens.Where(t => t.Path.PathString == "$.Phones").Count());
                Assert.AreEqual(JsonUtil.Serialize(emps[(int)d.Id - 1]), d.BodyData);
            }
        }


        [TestMethod]
        public async Task TestBulks()
        {
            var _dbc = await GetDbContext();
            var repo = new IndexDbRepo(_dbc, _logger);
            var list = Enumerable.Range(1 , 50);
            var emps = new List<Employee>();
            foreach(var i  in list){
                emps.Add(new Employee
                {
                    Phones = new[] { "123", "456", "789" },
                    Name = "layan",
                    EndDate = DateTime.UtcNow,
                    Id = i,
                    ContractType = EmploymentType.Permenant
                });
            };

            var docs = emps.Select(e => new Document()
            {
                Data = e,
                Source = new DocumentSource(new DocumentType(e.GetType()), ContentFactory.Default.CreateFrom($"{e.ContractType.ToString()}:{e.Id}"))
            });

            await repo.UpdateDocuments(docs.ToArray());
            foreach (var i in list)
            {
                var emp = emps[i-1];
                emp.Name = $"{emp.Name}{emp.ContractType.ToString()}:{emp.Id}" ;
                emp.Phones = new string[0];

            };

            await repo.UpdateDocuments(docs.ToArray());

            _dbc = await GetDbContext();

            var docs2 = await _dbc.Set<DbDoc>()
                .Include(d => d.Tokens)
                .ThenInclude(d => d.Path)
                .ToArrayAsync();

            foreach(var d in docs2)
            {
                //check that the phone tokens have deleted
                Assert.AreEqual(4, d.Tokens.Count);

                //check that the name token has changed
                Assert.AreEqual($"layan{d.SourceIdString}",d.Tokens.ToList().First(t => t.Path.PathString =="$.Name").ValueAsAny);


            }

        }
    }
}
