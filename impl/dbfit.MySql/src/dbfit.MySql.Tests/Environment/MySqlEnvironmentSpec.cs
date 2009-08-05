using System;
using System.Data;
using System.Data.Common;
using System.Text;
using dbfit.fixture;
using MySql.Data.MySqlClient;
using NUnit.Framework;
using Rhino.Mocks;

namespace dbfit.test {
    public class MySqlEnvironmentSpec : Spec {

        protected MySqlEnvironmentTestDouble environment = null;

        public override void SetUp() {
            base.SetUp();
            environment = new MySqlEnvironmentTestDouble();
        }

        [Test]
        public void DoesNotSupportReturnOnInsert()
        {
            Assert.IsFalse(environment.SupportsReturnOnInsert);
        }

        [Test]
        public void UsesQuestionMarkAsParameterPrefix(){
            Assert.IsTrue(environment.ParameterPrefix == "@");
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
        
            Assert.That(sql, Is.StringContaining("WHERE (table_schema=database() AND LOWER(table_name)=?tablename)"));
        }


        [Test]
        public void ShouldReturnSqlToGetAllColumnsForSchemaDotTablename()
        {
            string sql = environment.GetAllColumnsSql(new SchemaObjectName("schemaName","tableName"));

            Assert.That(sql, Is.StringContaining("LOWER(table_schema)=?dbname AND LOWER(table_name)=?tablename "));
        }

        [Test]
        public void CheckEmptyParams() {
            Assert.That( environment.ExtractParamNames("select * from dual").Length, Is.EqualTo(0) );
        }
        [Test]
        public void CheckSingleParam() {
            Assert.AreEqual(new string[] { "@mydate" }, environment.ExtractParamNames("select * from dual where sysdate<@mydate"));
        }
        [Test]
        public void CheckMultipleParams() {
            string[] paramnames = environment.ExtractParamNames("select @myname as zeka from dual where sysdate<@mydate");
            Assert.AreEqual(2, paramnames.Length);
            Assert.Contains("@mydate", paramnames);
            Assert.Contains("@myname", paramnames);
        }
        [Test]
        public void CheckMultipleParamsRecurring() {
            string[] paramnames = environment.ExtractParamNames("select @myname,length(@myname) as l, @myname || @mydate as zeka2 from dual where sysdate<@mydate");
            Assert.AreEqual(2, paramnames.Length);
            Assert.Contains("@mydate", paramnames);
            Assert.Contains("@myname", paramnames);
        }
        [Test]
        public void CheckUnderscore() {
            Assert.AreEqual(new string[] { "@my_date" }, environment.ExtractParamNames("select * from dual where sysdate<@my_date"));
        }


        [Test]
        public void ShouldExtractParametersFromCommand()
        {
            const string COMMAND_WITH_PARAMETERS = "INSERT INTO Test_DBFit VALUES (@name,10)";

            string[] parameterNames = environment.ExtractParamNames(COMMAND_WITH_PARAMETERS);

            Assert.That(parameterNames, Has.Length.EqualTo(1));
            Assert.That(parameterNames, Has.Member("@name"));

        }

        [Test]
        public void ShouldIncludeAtSymbolInParameterName()
        {
            Assert.That(environment.paramNameRegex.Match("@name").Groups[1].Value, Is.EqualTo("@name"));
        }

        // Get the data from the database
        // Package into some disconnected structure





        protected class MySqlEnvironmentTestDouble : MySqlEnvironment
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
