using System;

namespace dbfit
{
    public class SchemaObjectName
    {
        private readonly string name;
        private readonly string schema = string.Empty;

        public SchemaObjectName(string schema, string name)
        {
            if(!String.IsNullOrEmpty( schema )) this.schema = schema;
            this.name = name;
        }

        public SchemaObjectName(string name):this(null, name)
        {
        }

        public string Name
        {
            get { return name; }
        }

        public string Schema
        {
            get { return schema; }
        }

        public override string ToString()
        {
            return !HasSchema() ? name : string.Format("{0}.{1}", schema, name);
        }

        public static SchemaObjectName Parse(string[] valueArray)
        {
            switch (valueArray.Length)
            {
                case 1:
                    return new SchemaObjectName(valueArray[0]);
                case 2:
                    return new SchemaObjectName(valueArray[0], valueArray[1]);
                default:
                    throw new ArgumentOutOfRangeException("valueArray", valueArray.Length,
                                                          "The length of the array needs to be 1 or 2.");
            }
        }

        public bool HasSchema()
        {
            return (!String.IsNullOrEmpty(schema));
        }
    }
}