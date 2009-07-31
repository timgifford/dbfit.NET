using NUnit.Framework;

namespace dbfit.test
{
    [TestFixture]
    public class SchemaObjectNameSpec
    {
        [Test]
        public void ShouldParseArrayWithName()
        {
            string[] schemaAndName = new string[] { "tablename" };

            SchemaObjectName testName = SchemaObjectName.Parse(schemaAndName);

            Assert.That(testName.HasSchema(), Is.False);
        }

        [Test]
        public void ShouldParseArrayWithSchemaAndName()
        {
            string[] schemaAndName = new string[]{"schema","tablename"};
            
            SchemaObjectName testName = SchemaObjectName.Parse(schemaAndName);

            Assert.That(testName.HasSchema());
        }

        [Test]
        public void ShouldToStringWithSchema()
        {
            SchemaObjectName name = new SchemaObjectName("Schema", "TableName");

            Assert.That(name.ToString(), Is.EqualTo("Schema.TableName"));
        }

        [Test]
        public void ShouldToStringWithoutSchema()
        {
            SchemaObjectName name = new SchemaObjectName(null, "TableName");

            Assert.That(name.ToString(), Is.EqualTo("TableName"));
        }
    }
}