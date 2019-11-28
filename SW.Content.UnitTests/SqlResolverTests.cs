using Microsoft.Data.Sqlite;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SW.Content.Search;
using SW.Content.Search.EF;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace SW.Content.UnitTests
{
    [TestClass]
    public class SqlResolverTests
    {

       



        [TestMethod]
        public void InitialTest()
        {
            var paths = new Dictionary<string, int>
            {
                { "$.Audit.CreatedOn", 199 },
                { "$.Name", 150 },
                { "$.Id", 155 },
                // ...etc.
            };

            var docType = new DocumentType(typeof(Employee));
            var queryLines = new List<SearchQuery.Line>()
            {
                new SearchQuery.Line(
                    ContentPath.Parse($"$.{nameof(Employee.Name)}")
                    ,SearchQuery.Op.Equals,ContentFactory.Default.CreateFrom("John Smith")),
                new SearchQuery.Line(
                    ContentPath.Parse($"$.{nameof(Employee.Id)}")
                    ,SearchQuery.Op.Equals,ContentFactory.Default.CreateFrom(1)),

            };
            var query = new SearchQuery(docType, queryLines, ContentPath.Parse($"$.{nameof(Employee.Id)}"), false, 0, 20);
            var actual = SqlResolver.ResolveSqlText(query, (s) => paths[s], "[search].[Docs]", "[search].[DocTokens]");
            var expectedWithSpaces = $@"SELECT [B].* FROM (SELECT DISTINCT filtered.DocumentId, Sorted.ValueAsAny FROM 
                (SELECT DocumentId FROM [search].[DocTokens] 
                 WHERE ([ValueAsAny]='John Smith' AND [PathId]=150) 
                INTERSECT 
                SELECT DocumentId FROM [search].[DocTokens]
                WHERE ([ValueAsAny]='011.000000' AND [PathId]=155) 
                ) as filtered 
                LEFT JOIN (SELECT DocumentId,ValueAsAny FROM [search].[DocTokens]
                WHERE [PathId] = {paths["$.Id"]}) as Sorted
                ON Sorted.DocumentId = filtered.DocumentId) AS [A] 
                INNER JOIN (SELECT * FROM [search].[Docs]
                WHERE [SourceType] = '{query.DocumentType.Name}') AS [B] ON [A].DocumentId = [B].Id
                 ORDER BY [A].ValueAsAny 
                OFFSET {query.Offset} ROWS
                FETCH NEXT {query.Limit} ROWS ONLY
             ";

            Assert.AreEqual(Regex.Replace(expectedWithSpaces, @"\s+", string.Empty), Regex.Replace(actual, @"\s+", string.Empty));
        }

        [TestMethod]
        public void AnyOf()
        {
            var paths = new Dictionary<string, int>
            {
                { "$.Audit.CreatedOn", 199 },
                { "$.Name", 150 },
                { "$.Id", 155 },
                { "$.Phones", 156 },
                // ...etc.
            };
            var docType = new DocumentType(typeof(Employee));
            var queryLines = new List<SearchQuery.Line>()
            {
                new SearchQuery.Line(
                    ContentPath.Parse($"$.{nameof(Employee.Phones)}")
                    ,SearchQuery.Op.AnyOf,ContentFactory.Default.CreateFrom(new string[]{ "0001ddd","0002ddd","0003ddd"})),

                new SearchQuery.Line(
                    ContentPath.Parse($"$.{nameof(Employee.Name)}")
                    ,SearchQuery.Op.AnyOf,ContentFactory.Default.CreateFrom(new int[]{ 1,2,3}))


            };
            var query = new SearchQuery(docType, queryLines, ContentPath.Parse($"$.{nameof(Employee.Id)}"), true, 0, 20);

            var actual = SqlResolver.ResolveSqlText(query, (s) => paths[s], "[search].[Docs]", "[search].[DocTokens]");
            var expectedWithSpaces= $@"SELECT [B].* FROM (SELECT DISTINCT filtered.DocumentId, Sorted.ValueAsAny FROM 
                (SELECT DocumentId FROM [search].[DocTokens] 
                 WHERE ([ValueAsAny] IN ('0001ddd','0002ddd','0003ddd') AND [PathId]=156) 
                INTERSECT
                SELECT DocumentId FROM [search].[DocTokens] 
                WHERE ([ValueAsAny] IN ('011.000000','012.000000','013.000000') AND [PathId]=150) 
                ) as filtered 
                LEFT JOIN (SELECT DocumentId,ValueAsAny FROM [search].[DocTokens]
                WHERE [PathId] = 155) as Sorted
                ON Sorted.DocumentId = filtered.DocumentId
               ) AS [A] 
                INNER JOIN (SELECT * FROM [search].[Docs]
                WHERE [SourceType] = '{query.DocumentType.Name}') AS [B] ON [A].DocumentId = [B].Id
                ORDER BY [A].ValueAsAny DESC
                OFFSET 0 ROWS
                FETCH NEXT 20 ROWS ONLY
             ";
            Assert.AreEqual(Regex.Replace(expectedWithSpaces, @"\s+", string.Empty), Regex.Replace(actual, @"\s+", string.Empty));

        }

        [TestMethod]
        public void DateRange()
        {
            var paths = new Dictionary<string, int>
            {
                { "$.Audit.CreatedOn", 199 },
                { "$.Name", 150 },
                { "$.Id", 155 },
                // ...etc.
            };
            var docType = new DocumentType(typeof(Employee));
            var queryLines = new List<SearchQuery.Line>()
            {
                new SearchQuery.Line(
                    ContentPath.Parse($"$.{nameof(Employee.Name)}")
                    ,SearchQuery.Op.GreaterThanOrEquals,ContentFactory.Default.CreateFrom(new DateTime(2019,08,01))),

                new SearchQuery.Line(
                    ContentPath.Parse($"$.{nameof(Employee.Name)}")
                    ,SearchQuery.Op.LessThanOrEquals,ContentFactory.Default.CreateFrom(new DateTime(2019,11,27)))
            };
            var query = new SearchQuery(docType, queryLines, ContentPath.Parse($"$.{nameof(Employee.Id)}"), true, 0, 20);
            var actual = SqlResolver.ResolveSqlText(query, (s) => paths[s], "[search].[Docs]", "[search].[DocTokens]");
            var expectedWithSpaces = $@"SELECT [B].* FROM (SELECT DISTINCT filtered.DocumentId, Sorted.ValueAsAny FROM 
                (SELECT DocumentId FROM [search].[DocTokens] 
                 WHERE ([ValueAsAny]>='2019-08-01T00:00:00.0000000' AND [PathId]=150)
                INTERSECT
                SELECT DocumentId FROM [search].[DocTokens] 
                WHERE ([ValueAsAny]<='2019-11-27T00:00:00.0000000' AND [PathId]=150) 
                ) as filtered 
                LEFT JOIN (SELECT DocumentId,ValueAsAny FROM [search].[DocTokens]
                WHERE [PathId] = 155) as Sorted
                ON Sorted.DocumentId = filtered.DocumentId
               ) AS [A] 
                INNER JOIN (SELECT * FROM [search].[Docs]
                WHERE [SourceType] = '{query.DocumentType}') AS [B] ON [A].DocumentId = [B].Id
                 ORDER BY [A].ValueAsAny DESC
                OFFSET 0 ROWS
                FETCH NEXT 20 ROWS ONLY
             ";
            Assert.AreEqual(Regex.Replace(expectedWithSpaces, @"\s+", string.Empty), Regex.Replace(actual, @"\s+", string.Empty));

        }

        [TestMethod]
        public void EqualNull()
        {
            var paths = new Dictionary<string, int>
            {
                { "$.Audit.CreatedOn", 199 },
                { "$.Name", 150 },
                { "$.Id", 155 },
                { "$.StartDate", 156 },


                // ...etc.
            };
            var docType = new DocumentType(typeof(Employee));
            var queryLines = new List<SearchQuery.Line>()
            {
                new SearchQuery.Line(
                    ContentPath.Parse($"$.{nameof(Employee.StartDate)}")
                    ,SearchQuery.Op.Equals,ContentFactory.Default.CreateFrom(null)),


              
            };
            var query = new SearchQuery(docType, queryLines, ContentPath.Parse($"$.{nameof(Employee.Id)}"), true, 0, 20);
            var actual = SqlResolver.ResolveSqlText(query, (s) => paths[s], "[search].[Docs]", "[search].[DocTokens]");
            var expectedWithSpaces = $@"SELECT [B].* FROM (SELECT DISTINCT filtered.DocumentId, Sorted.ValueAsAny FROM 
                (SELECT DocumentId FROM [search].[DocTokens] 
                 WHERE ([ValueAsAny] IS NULL AND [PathId]=156) 
                ) as filtered 
                LEFT JOIN (SELECT DocumentId,ValueAsAny FROM [search].[DocTokens]
                WHERE [PathId] = 155) as Sorted
                ON Sorted.DocumentId = filtered.DocumentId
               ) AS [A] 
                INNER JOIN (SELECT * FROM [search].[Docs]
                WHERE [SourceType] = '{query.DocumentType}') AS [B] ON [A].DocumentId = [B].Id
                ORDER BY [A].ValueAsAny DESC
                OFFSET 0 ROWS
                FETCH NEXT 20 ROWS ONLY";

            Assert.AreEqual(Regex.Replace(expectedWithSpaces, @"\s+", string.Empty), Regex.Replace(actual, @"\s+", string.Empty));
        }

        [TestMethod]
        public void NotNull()
        {
            var paths = new Dictionary<string, int>
            {
                { "$.Audit.CreatedOn", 199 },
                { "$.Name", 150 },
                { "$.Id", 155 },
                { "$.StartDate",156}
                // ...etc.
            };
            var docType = new DocumentType(typeof(Employee));
            var queryLines = new List<SearchQuery.Line>()
            {
                new SearchQuery.Line(
                    ContentPath.Parse($"$.{nameof(Employee.StartDate)}")
                    ,SearchQuery.Op.NotEquals,ContentFactory.Default.CreateFrom(null)),
                 new SearchQuery.Line(
                    ContentPath.Parse($"$.{nameof(Employee.Name)}")
                    ,SearchQuery.Op.GreaterThanOrEquals,ContentFactory.Default.CreateFrom(new DateTime(2019,08,01))),

                new SearchQuery.Line(
                    ContentPath.Parse($"$.{nameof(Employee.Name)}")
                    ,SearchQuery.Op.LessThanOrEquals,ContentFactory.Default.CreateFrom(new DateTime(2019,11,27)))


            };
            var query = new SearchQuery(docType, queryLines, ContentPath.Parse($"$.{nameof(Employee.Id)}"), true, 0, 20);
            var actual = SqlResolver.ResolveSqlText(query, (s) => paths[s], "[search].[Docs]", "[search].[DocTokens]");

            var expectedWithSpaces = $@"SELECT [B].* FROM (SELECT DISTINCT filtered.DocumentId, Sorted.ValueAsAny FROM 
                (SELECT DocumentId FROM [search].[DocTokens] 
                 WHERE ([ValueAsAny] IS NOT NULL AND [PathId]=156)
                INTERSECT
                SELECT DocumentId FROM [search].[DocTokens] 
                 WHERE ([ValueAsAny]>='2019-08-01T00:00:00.0000000' AND [PathId]=150)
                INTERSECT
                SELECT DocumentId FROM [search].[DocTokens] 
                WHERE ([ValueAsAny]<='2019-11-27T00:00:00.0000000' AND [PathId]=150) 
                ) as filtered 
                LEFT JOIN (SELECT DocumentId,ValueAsAny FROM [search].[DocTokens]
                WHERE [PathId] = {paths["$.Id"]}) as Sorted
                ON Sorted.DocumentId = filtered.DocumentId
                ) AS [A] 
                INNER JOIN (SELECT * FROM [search].[Docs]
                WHERE [SourceType] = '{query.DocumentType}') AS [B] ON [A].DocumentId = [B].Id
                ORDER BY [A].ValueAsAny DESC
                OFFSET 0 ROWS
                FETCH NEXT 20 ROWS ONLY
             ";

            Assert.AreEqual(Regex.Replace(expectedWithSpaces, @"\s+", string.Empty), Regex.Replace(actual, @"\s+", string.Empty));

        }

        [TestMethod]
        public void EmptyQuery()
        {
            var paths = new Dictionary<string, int>
            {
                { "$.Audit.CreatedOn", 199 },
                { "$.Id", 155 },


                // ...etc.
            };
            var docType = new DocumentType(typeof(Employee));
            var queryLines = new List<SearchQuery.Line>();
                    
            var query = new SearchQuery(docType, queryLines, ContentPath.Parse($"$.{nameof(Employee.Id)}"), true, 0, 20);
            var actual = SqlResolver.ResolveSqlText(query, (s) => paths[s], "[search].[Docs]", "[search].[DocTokens]");

            var expectedWithSpaces = $@"SELECT [B].* FROM (SELECT DISTINCT filtered.DocumentId, Sorted.ValueAsAny FROM 
                (SELECT DocumentId FROM [search].[DocTokens] Group by [DocumentId]
                ) as filtered 
                LEFT JOIN (SELECT DocumentId,ValueAsAny FROM [search].[DocTokens]
                WHERE [PathId] = {paths["$.Id"]}) as Sorted
                ON Sorted.DocumentId = filtered.DocumentId
                ) AS [A] 
                INNER JOIN (SELECT * FROM [search].[Docs]
                WHERE [SourceType] = '{query.DocumentType}') AS [B] ON [A].DocumentId = [B].Id 
                ORDER BY [A].ValueAsAny DESC
                OFFSET 0 ROWS
                FETCH NEXT 20 ROWS ONLY
             ";

            Assert.AreEqual(Regex.Replace(expectedWithSpaces, @"\s+", string.Empty), Regex.Replace(actual, @"\s+", string.Empty));

        }
    }
}
