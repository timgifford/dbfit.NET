using System.Collections.Generic;
using System.Data;
using NUnit.Framework;
using Rhino.Mocks;

namespace dbfit.test
{
    public class MySqlTypeConverterSpec : Spec
    {
        private ColumnInfo[] bigIntcolumnInfo;
        private ColumnInfo[] stringColumnInfo;

        public override void SetUp() {
            base.SetUp();
            
            bigIntcolumnInfo = new ColumnInfo[] 
                                   {
                                       new ColumnInfo("column_name", "bigint", 3)
                                   };

            stringColumnInfo = new ColumnInfo[]
                                   {
                                       new ColumnInfo("column_name", "varchar", 3)
                                   };
        }

        [Test]
        public void ShouldMapDisconnectedDataStructureToDbParameterAccessor()
        {
            IList<DbParameterAccessor> accessor = MySqlTypeConverter.BuildDbParameterAccessorFromColumnInfo(bigIntcolumnInfo);

            Assert.That(accessor[0].ActualSqlType, Is.EqualTo("bigint"));
            Assert.That(accessor[0].DbFieldName, Is.EqualTo("column_name"));
            Assert.That(accessor[0].Position, Is.EqualTo(0));
            Assert.That(accessor[0].DbParameter.Direction, Is.EqualTo(ParameterDirection.Input));
            Assert.That(accessor[0].DbParameter.Size, Is.EqualTo(3));
        }

        [Test]
        public void ShouldMapRunTimeTypeToDbParameterAccessorForBigIntColumn() {
            IList<DbParameterAccessor> accessor = MySqlTypeConverter.BuildDbParameterAccessorFromColumnInfo(bigIntcolumnInfo);

            Assert.That(accessor[0].DotNetType, Is.EqualTo(typeof(long)));
        }

        [Test]
        public void ShouldMapMySqlTypeToDbParameterAccessorForBigIntColumn() {
            IList<DbParameterAccessor> accessor = MySqlTypeConverter.BuildDbParameterAccessorFromColumnInfo(bigIntcolumnInfo);

            Assert.That(accessor[0].DbParameter.DbType, Is.EqualTo(DbType.Int64));
        }

        [Test]
        public void ShouldMapMySqlTypeToDbParameterAccessorForStringColumn() {
            IList<DbParameterAccessor> accessor = MySqlTypeConverter.BuildDbParameterAccessorFromColumnInfo(stringColumnInfo);

            Assert.That(accessor[0].DbParameter.DbType, Is.EqualTo(DbType.String));
        }

        [Test]
        public void ShouldMapRunTimeTypeToDbParameterAccessorForStringColumn() {
            IList<DbParameterAccessor> accessor = MySqlTypeConverter.BuildDbParameterAccessorFromColumnInfo(stringColumnInfo);

            Assert.That(accessor[0].DotNetType, Is.EqualTo(typeof(string)));
        }

        [Test]
        public void ShouldMapDataReaderToColumnInfo() {
            IDataReader reader = Mock<IDataReader>();
            ColumnInfo expectedColumn = new ColumnInfo("column_name", "varchar", 20);
            ColumnInfo result = null;

            using (Record) {
                Expect.Call(reader.GetString(0)).Return("column_name");
                Expect.Call(reader.GetString(1)).Return("varchar");
                Expect.Call(reader.GetInt32(2)).Return(20);
            }

            using (Playback) {
                result = MySqlTypeConverter.GetColumnInfoFrom(reader);
            }

            Assert.That(result, Is.EqualTo(expectedColumn));
        }
    }
}