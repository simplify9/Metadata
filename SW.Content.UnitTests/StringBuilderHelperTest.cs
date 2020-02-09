using Microsoft.Data.Sqlite;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SW.Content.Search.EF;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Text;

namespace SW.Content.UnitTests
{
   [TestClass]
    public class StringBuilderHelperTest
    {
        [TestMethod]
        public void InitialTest()
        {

            var comm = @"UPDATE DocTokens
                        SET
                            LastUpdatedOn = ?,
                            ValueAsAny = ?
                        WHERE DocumentId = ? and PathId = ? and [Offset] = ?";
            var stringBuilder = new StringBuilderHelper();
            stringBuilder.Append(comm, DateTime.UtcNow, "layan", 1, 2, 0);
            DbConnection _dbConn= new SqliteConnection("DataSource=:memory:");
            var command = stringBuilder.CreateCommand(_dbConn);

        }
    }
}
