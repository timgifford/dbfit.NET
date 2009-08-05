using System.Collections.Generic;
using System.Data;
using NUnit.Framework;
using Rhino.Mocks;

namespace dbfit.test
{
    public class MySqlTypeConverterSpec : Spec
    {
        private ColumnInfo bigIntcolumnInfo;
        private ColumnInfo stringColumnInfo;

        public override void SetUp() {
            base.SetUp();
            
            bigIntcolumnInfo = new ColumnInfo("column_name", "bigint", 3);
            stringColumnInfo = new ColumnInfo("column_name", "varchar", 3);
        }

        [Test]
        public void ShouldMapDisconnectedDataStructureToDbParameterAccessor()
        {
            DbParameterAccessor accessor = MySqlTypeConverter.BuildDbParameterAccessorFrom(bigIntcolumnInfo, 0);

            Assert.That(accessor.ActualSqlType, Is.EqualTo(bigIntcolumnInfo.Datatype));
            Assert.That(accessor.DbFieldName, Is.EqualTo(bigIntcolumnInfo.ColumnName));
            Assert.That(accessor.Position, Is.EqualTo(0));
            Assert.That(accessor.DbParameter.Direction, Is.EqualTo(ParameterDirection.Input));
            Assert.That(accessor.DbParameter.Size, Is.EqualTo(bigIntcolumnInfo.Size));
        }

        [Test]
        public void ShouldMapRunTimeTypeToDbParameterAccessorForBigIntColumn() {
            DbParameterAccessor accessor = MySqlTypeConverter.BuildDbParameterAccessorFrom(bigIntcolumnInfo, 0);

            Assert.That(accessor.DotNetType, Is.EqualTo(typeof(long)));
        }

        [Test]
        public void ShouldMapMySqlTypeToDbParameterAccessorForBigIntColumn() {
            DbParameterAccessor accessor = MySqlTypeConverter.BuildDbParameterAccessorFrom(bigIntcolumnInfo, 0);

            Assert.That(accessor.DbParameter.DbType, Is.EqualTo(DbType.Int64));
        }

        [Test]
        public void ShouldMapMySqlTypeToDbParameterAccessorForStringColumn() {
            DbParameterAccessor accessor = MySqlTypeConverter.BuildDbParameterAccessorFrom(stringColumnInfo, 0);

            Assert.That(accessor.DbParameter.DbType, Is.EqualTo(DbType.String));
        }

        [Test]
        public void ShouldMapRunTimeTypeToDbParameterAccessorForStringColumn() {
            DbParameterAccessor accessor = MySqlTypeConverter.BuildDbParameterAccessorFrom(stringColumnInfo, 0);

            Assert.That(accessor.DotNetType, Is.EqualTo(typeof(string)));
        }


        [Test]
        public void ShouldMapSourceColumnToDbParameterAccessorForStringColumn() {
            DbParameterAccessor accessor = MySqlTypeConverter.BuildDbParameterAccessorFrom(stringColumnInfo, 0);

            Assert.That(accessor.DbParameter.SourceColumn, Is.EqualTo(stringColumnInfo.ColumnName));
        }


        [Test]
        public void ShouldMapDataReaderToColumnInfoWhenReaderHasNullValues()
        {
            IDataReader reader = Mock<IDataReader>();
            ColumnInfo result = null;

            using (Record) {
                Expect.Call(reader.IsDBNull(0)).IgnoreArguments().Repeat.Times(3).Return(true);
            }
            using (Playback) {
                result = MySqlTypeConverter.GetColumnInfoFrom(reader);
            }

            Assert.That(result, Is.EqualTo(ColumnInfo.Empty));

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