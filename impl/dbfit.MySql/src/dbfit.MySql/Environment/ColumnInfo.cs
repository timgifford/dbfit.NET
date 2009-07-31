using System;

namespace dbfit
{
    public class ColumnInfo : IEquatable<ColumnInfo>
    {
        private readonly string columnName;
        private readonly string datatype;
        private readonly int size;

        public ColumnInfo(string columnName, string datatype, int size) {
            this.columnName = columnName;
            this.datatype = datatype;
            this.size = size;
        }

        public int Size{
            get { return size; }
        }

        public string ColumnName {
            get { return columnName; }
        }

        public string Datatype {
            get { return datatype; }
        }

        public bool Equals(ColumnInfo other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Equals(other.columnName, columnName) && Equals(other.datatype, datatype) && other.size == size;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != typeof (ColumnInfo)) return false;
            return Equals((ColumnInfo) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int result = (columnName != null ? columnName.GetHashCode() : 0);
                result = (result*397) ^ (datatype != null ? datatype.GetHashCode() : 0);
                result = (result*397) ^ size;
                return result;
            }
        }

        public static bool operator ==(ColumnInfo left, ColumnInfo right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(ColumnInfo left, ColumnInfo right)
        {
            return !Equals(left, right);
        }
    }
}