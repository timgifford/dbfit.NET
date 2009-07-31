using NUnit.Framework;

namespace dbfit.test
{
    [TestFixture]
    public class SchemaObjectNameSpec
    {
        [Test]
        public void ShouldParseArrayWithName()
        {
            Assert.That(SchemaObjectName.Parse(new string[] { "tablename" }).HasSchema(), Is.False);
        }

        [Test]
        public void ShouldParseArrayWithSchemaAndName()
        {
            Assert.That(SchemaObjectName.Parse(new string[]{"schema","tablename"}).HasSchema());
        }

        [Test]
        public void ShouldToStringWithSchema()
        {
            Assert.That(new SchemaObjectName("Schema", "TableName").ToString(), Is.EqualTo("Schema.TableName"));
        }

        [Test]
        public void ShouldToStringWithoutSchema()
        {
            Assert.That(new SchemaObjectName(null, "TableName").ToString(), Is.EqualTo("TableName"));
        }
    }
}