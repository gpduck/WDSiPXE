using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WDSiPXE;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Data.Common;
using System.IO;
using System.Linq;
using NDbUnit.Core;
using NDbUnit.Core.SqlLite;
using System.Threading;

namespace WDSiPXE.Tests
{
    [TestClass]
    public class MDTDeviceRepositoryTests : BaseDeviceRepository
    {
        protected override System.Collections.Generic.IList<System.Collections.Generic.KeyValuePair<string,object>> TestValues
        {
	        get {
                return new List<KeyValuePair<String, Object>>(
                    new KeyValuePair<String, Object>[] {
                        new KeyValuePair<String, Object>("ComputerName", "test"),
                        new KeyValuePair<String, Object>("MacAddress", "00:00:00:00:00:01")
                    }
                );
            }
        }

        private const string _connectionString = @"Data Source=MdtTables\Mdt2013U1.db";
        private static readonly object LockObject = new Object();

        [ClassInitialize]
        public static void ClassInit(TestContext ctx)
        {
            Monitor.Enter(LockObject);
            using(IDbConnection connection = new SQLiteConnection(_connectionString)) {
                INDbUnitTest db = new SqlLiteDbUnitTest(connection);
                db.ReadXmlSchema(@"MdtTables\Mdt2013U1.xsd");
                
                db.ReadXml(@"MdtTables\Mdt2013U1.xml");
                db.PerformDbOperation(DbOperationFlag.InsertIdentity);
                //CleanInsertIdentity locks the database :(
                //db.PerformDbOperation(DbOperationFlag.CleanInsertIdentity);
                connection.Close();
            }
            
        }

        [ClassCleanup]
        public static void TestCleanup()
        {
            using (IDbConnection connection = new SQLiteConnection(_connectionString))
            {
                connection.Open();
                using (IDbCommand command = connection.CreateCommand())
                {
                    command.CommandText = @"DELETE FROM ComputerIdentity;";
                    command.ExecuteNonQuery();
                    command.CommandText = @"DELETE FROM Settings;";
                    command.ExecuteNonQuery();
                }
            }
            Monitor.Exit(LockObject);
        }


        protected override IDeviceRepository GetRepository()
        {
            return new MDTDeviceRepository(_connectionString, SQLiteFactory.Instance);
        }
    }
}
