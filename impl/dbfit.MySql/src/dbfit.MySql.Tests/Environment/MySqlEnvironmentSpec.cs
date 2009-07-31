using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Text;
using MySql.Data.MySqlClient;
using NUnit.Framework;

namespace dbfit.test {
    [TestFixture]
    public class MySqlEnvironmentSpec {

        private MySqlEnvironmentTestDouble environment = null;
        private ColumnInfo[] bigIntcolumnInfo;
        private ColumnInfo[] stringColumnInfo;

        [SetUp]
        public void SetUp()
        {
            environment = new MySqlEnvironmentTestDouble();
            bigIntcolumnInfo = new ColumnInfo[] {
                                              new ColumnInfo("column_name", "bigint", "direction", 3)};

            stringColumnInfo = new ColumnInfo[]
                                   {
                                       new ColumnInfo("column_name", "varchar", "direction", 3)
                                   };
        }

        [Test]
        public void DoesNotSupportReturnOnInsert()
        {
            Assert.IsFalse(environment.SupportsReturnOnInsert);
        }

        [Test]
        public void UsesQuestionMarkAsParameterPrefix(){
            Assert.IsTrue(environment.ParameterPrefix == "?");
        }

        [Test]
        public void ShouldGetConnectionStringWithoutDatabase(){
            environment.Connect("dbServer", "dbUser", "dbPassword");
            Assert.That(environment.ConnectionString, Is.EqualTo("Server=dbServer;Uid=dbUser;Pwd=dbPassword;"));
        }

        [Test]
        public void ShouldGetConnectionStringWithDatabase() {
            environment.Connect("dbServer", "dbUser", "dbPassword", "dbName");
            Assert.That(environment.ConnectionString, Is.EqualTo("Server=dbServer;Database=dbName;Uid=dbUser;Pwd=dbPassword;"));
        }

        [Test]
        public void ShouldGetMySqlDbFactoryProvider()
        {
            System.Data.Common.DbProviderFactory factory = environment.DbProviderFactory;
            Assert.That(factory, Is.Not.Null.And.TypeOf(typeof(MySql.Data.MySqlClient.MySqlClientFactory)));
        }

        [Test]
        public void IdentitySelectStatementUsesAtAtIdentity()
        {
            Assert.That(environment.IdentitySelectStatement, Is.EqualTo("SELECT @@identity;"));
        }

        [Test]
        public void ShouldReturnSqlToGetAllColumnsForATable(){
            string sql = environment.GetAllColumnsSql(new SchemaObjectName("tableName"));
        
            Assert.That(sql, Is.StringContaining("WHERE (table_schema=database() AND LOWER(table_name)=?)"));
        }


        [Test]
        public void ShouldReturnSqlToGetAllColumnsForSchemaDotTablename()
        {
            string sql = environment.GetAllColumnsSql(new SchemaObjectName("schemaName","tableName"));

            Assert.That(sql, Is.StringContaining("LOWER(table_schema)=? AND LOWER(table_name)=? "));
        }

        [Test]
        public void CheckEmptyParams() {
            Assert.That( environment.ExtractParamNames("select * from dual").Length, Is.EqualTo(0) );
        }
        [Test]
        public void CheckSingleParam() {
            Assert.AreEqual(new string[] { "mydate" }, environment.ExtractParamNames("select * from dual where sysdate<?mydate"));
        }
        [Test]
        public void CheckMultipleParams() {
            string[] paramnames = environment.ExtractParamNames("select ?myname as zeka from dual where sysdate<?mydate");
            Assert.AreEqual(2, paramnames.Length);
            Assert.Contains("mydate", paramnames);
            Assert.Contains("myname", paramnames);
        }
        [Test]
        public void CheckMultipleParamsRecurring() {
            string[] paramnames = environment.ExtractParamNames("select ?myname,length(?myname) as l, ?myname || ?mydate as zeka2 from dual where sysdate<?mydate");
            Assert.AreEqual(2, paramnames.Length);
            Assert.Contains("mydate", paramnames);
            Assert.Contains("myname", paramnames);
        }
        [Test]
        public void CheckUnderscore() {
            Assert.AreEqual(new string[] { "my_date" }, environment.ExtractParamNames("select * from dual where sysdate<?my_date"));
        }

        // Get the data from the database
        // Package into some disconnected structure

        [Test]
        public void ShouldMapDisconnectedDataStructureToDbParameterAccessor()
        {
            IList<DbParameterAccessor> accessor = environment.BuildDbParameterAccessorFromColumnInfo(bigIntcolumnInfo);

            Assert.That(accessor[0].ActualSqlType, Is.EqualTo("bigint"));
            Assert.That(accessor[0].DbFieldName, Is.EqualTo("column_name"));
            Assert.That(accessor[0].Position, Is.EqualTo(0));
            Assert.That(accessor[0].DbParameter.Direction, Is.EqualTo(ParameterDirection.Input));
            Assert.That(accessor[0].DbParameter.Size, Is.EqualTo(3));
        }

        [Test]
        public void ShouldMapRunTimeTypeToDbParameterAccessorForBigIntColumn() {
            IList<DbParameterAccessor> accessor = environment.BuildDbParameterAccessorFromColumnInfo(bigIntcolumnInfo);

            Assert.That(accessor[0].DotNetType, Is.EqualTo(typeof(long)));
        }

        [Test]
        public void ShouldMapMySqlTypeToDbParameterAccessorForBigIntColumn() {
            IList<DbParameterAccessor> accessor = environment.BuildDbParameterAccessorFromColumnInfo(bigIntcolumnInfo);

            Assert.That(accessor[0].DbParameter.DbType, Is.EqualTo(DbType.Int64));
        }

        [Test]
        public void ShouldMapMySqlTypeToDbParameterAccessorForStringColumn() {
            IList<DbParameterAccessor> accessor = environment.BuildDbParameterAccessorFromColumnInfo(stringColumnInfo);

            Assert.That(accessor[0].DbParameter.DbType, Is.EqualTo(DbType.String));
        }

        [Test]
        public void ShouldMapRunTimeTypeToDbParameterAccessorForStringColumn() {
            IList<DbParameterAccessor> accessor = environment.BuildDbParameterAccessorFromColumnInfo(stringColumnInfo);

            Assert.That(accessor[0].DotNetType, Is.EqualTo(typeof(string)));
        }


        private class MySqlEnvironmentTestDouble : MySqlEnvironment
        {
            public string ConnectionString = null;

            public override void Connect(string connectionString) {
                ConnectionString = connectionString;
            }
            public override void Connect(string dataSource, string username, string password) {
                ConnectionString = base.GetConnectionString(dataSource, username, password);
            }
            public override void Connect(string dataSource, string username, string password, string database) {
                ConnectionString = base.GetConnectionString(dataSource, username, password, database);
            }

        }
    }


}
