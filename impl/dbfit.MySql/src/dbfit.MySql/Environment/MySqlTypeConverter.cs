using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using MySql.Data.MySqlClient;

namespace dbfit
{
    public class MySqlTypeConverter
    {
        private static readonly List<string> stringTypes = new List<string>(new string[] { "VARCHAR", "CHAR", "TEXT" });
        private static readonly List<string> intTypes = new List<string>(new string[] { "TINYINT", "SMALLINT", "MEDIUMINT", "INT", "INTEGER" });
        private static readonly List<string> longTypes = new List<string>(new string[] { "BIGINT", "INTEGER UNSIGNED", "INT UNSIGNED" });
        private static readonly List<string> floatTypes = new List<string>(new string[] { "FLOAT" });
        private static readonly List<string> doubleTypes = new List<string>(new string[] { "DOUBLE" });
        private static readonly List<string> decimalTypes = new List<string>(new string[] { "DECIMAL", "DEC" });
        private static readonly List<string> dateTypes = new List<string>(new string[] { "DATE" });
        private static readonly List<string> timestampTypes = new List<string>(new string[] { "TIMESTAMP", "DATETIME" });

        public static MySqlDbType GetSqlType(string dataTypeName)
        {
            if(stringTypes.Contains(dataTypeName.ToUpperInvariant())) return MySqlDbType.VarChar;
            if(intTypes.Contains(dataTypeName.ToUpperInvariant())) return MySqlDbType.Int32;
            if(longTypes.Contains(dataTypeName.ToUpperInvariant())) return MySqlDbType.Int64;
            if(floatTypes.Contains(dataTypeName.ToUpperInvariant())) return MySqlDbType.Float;
            if(doubleTypes.Contains(dataTypeName.ToUpperInvariant())) return MySqlDbType.Double;
            if(decimalTypes.Contains(dataTypeName.ToUpperInvariant())) return MySqlDbType.Decimal;
            if(dateTypes.Contains(dataTypeName.ToUpperInvariant())) return MySqlDbType.Date;
            if(timestampTypes.Contains(dataTypeName.ToUpperInvariant())) return MySqlDbType.Timestamp;

            throw new ArgumentException(String.Format("Unable to find matching MySqlDbType for '{0}'", dataTypeName));
        }

        public static Type GetRuntimeType(string dataTypeName)
        {
            if (stringTypes.Contains(dataTypeName.ToUpperInvariant())) return typeof(string);
            if (intTypes.Contains(dataTypeName.ToUpperInvariant())) return typeof(int);
            if (longTypes.Contains(dataTypeName.ToUpperInvariant())) return typeof(long);
            if (floatTypes.Contains(dataTypeName.ToUpperInvariant())) return typeof(float);
            if (doubleTypes.Contains(dataTypeName.ToUpperInvariant())) return typeof(double);
            if (decimalTypes.Contains(dataTypeName.ToUpperInvariant())) return typeof(decimal);
            if (dateTypes.Contains(dataTypeName.ToUpperInvariant())) return typeof(DateTime);
            if (timestampTypes.Contains(dataTypeName.ToUpperInvariant())) return typeof(DateTime);


            throw new ArgumentException(String.Format("Unable to find matching RuntimeType for '{0}'", dataTypeName));
        }

        public static DbParameterAccessor BuildDbParameterAccessorFrom(IDataReader reader, int position)
        {
            ColumnInfo columnInfo = GetColumnInfoFrom(reader);
            return BuildDbParameterAccessorFrom(columnInfo, position);
        }

        public static DbParameterAccessor BuildDbParameterAccessorFrom(ColumnInfo column, int position) {
            Type dotNetType = GetRuntimeType(column.Datatype);
            MySqlDbType mySqlDbType = GetSqlType(column.Datatype);
            DbParameter mySqlParameter = new MySqlParameter(column.ColumnName, mySqlDbType, column.Size, column.ColumnName);
            return new DbParameterAccessor(mySqlParameter, dotNetType, position, column.Datatype); 
        }

        public static ColumnInfo GetColumnInfoFrom(IDataReader reader)
        {
            string columnName = reader.IsDBNull(0) ? string.Empty : reader.GetString(0);
            string dataType = reader.IsDBNull(1) ? string.Empty : reader.GetString(1);
            int size = reader.IsDBNull(2) ? 0 : reader.GetInt32(2);
            ColumnInfo column = new ColumnInfo(
                columnName,
                dataType,
                size
                );

            return column;
        }
    }
}