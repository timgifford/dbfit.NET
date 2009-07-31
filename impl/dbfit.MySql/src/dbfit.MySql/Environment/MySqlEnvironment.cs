using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Text;
using System.Text.RegularExpressions;
using dbfit.util;
using MySql.Data.MySqlClient;

namespace dbfit
{

    public class ColumnInfo {
        private readonly string columnName;
        private readonly string datatype;
        private readonly string direction;
        private readonly int size;

        public ColumnInfo(string columnName, string datatype, string direction, int size) {
            this.columnName = columnName;
            this.datatype = datatype;
            this.direction = direction;
            this.size = size;
        }

        public int Size
        {
            get { return size; }
        }

        public string ColumnName {
            get { return columnName; }
        }

        public string Datatype {
            get { return datatype; }
        }

        public string Direction {
            get { return direction; }
        }

    }

    public class MySqlEnvironment : AbstractDbEnvironment
    {
        public override string ParameterPrefix
        {
            get { return "?"; }
        }


        public IList<DbParameterAccessor> BuildDbParameterAccessorFromColumnInfo(IEnumerable <ColumnInfo> columnInfo) {
            
            IList <DbParameterAccessor> parameterList = new List<DbParameterAccessor>();
            int position = 0;

            foreach (ColumnInfo column in columnInfo)
            {
                Type dotNetType = GetRuntimeType(column.Datatype);
                MySqlDbType mySqlDbType = GetSqlType(column.Datatype);

                DbParameter mySqlParameter = new MySqlParameter(column.ColumnName, mySqlDbType, column.Size);

                parameterList.Add(new DbParameterAccessor(mySqlParameter, dotNetType, position++, column.Datatype)); 
            }
            return parameterList;
        }


        protected override string GetConnectionString(string dataSource, string username, string password)
        {
            return string.Format("Server={0};Uid={1};Pwd={2};", dataSource, username, password );
        }

        protected override string GetConnectionString(string dataSource, string username, string password, string database)
        {
            return string.Format("Server={0};Database={3};Uid={1};Pwd={2};", dataSource, username, password, database);
        }

        private readonly Regex paramNameRegex = new Regex("\\?([A-Za-z0-9_]+)");
        protected override Regex ParamNameRegex
        {
            get { return paramNameRegex; }
        }

        private static readonly DbProviderFactory MySqlDbProviderFactory = new MySql.Data.MySqlClient.MySqlClientFactory();

        private static readonly List<string> stringTypes = new List<string>(new string[] { "VARCHAR", "CHAR", "TEXT" });
        private static readonly List<string> intTypes = new List<string>(new string[] { "TINYINT", "SMALLINT", "MEDIUMINT", "INT", "INTEGER" });
        private static readonly List<string> longTypes = new List<string>(new string[] { "BIGINT", "INTEGER UNSIGNED", "INT UNSIGNED" });
        private static readonly List<string> floatTypes = new List<string>(new string[] { "FLOAT" });
        private static readonly List<string> doubleTypes = new List<string>(new string[] { "DOUBLE" });
        private static readonly List<string> decimalTypes = new List<string>(new string[] { "DECIMAL", "DEC" });
        private static readonly List<string> dateTypes = new List<string>(new string[] { "DATE" });
        private static readonly List<string> timestampTypes = new List<string>(new string[] { "TIMESTAMP", "DATETIME" });

        private static MySqlDbType GetSqlType(string dataTypeName)
        {
            if(stringTypes.Contains(dataTypeName.ToUpperInvariant())) return MySqlDbType.VarChar;
            if(intTypes.Contains(dataTypeName.ToUpperInvariant())) return MySqlDbType.Int32;
            if(longTypes.Contains(dataTypeName.ToUpperInvariant())) return MySqlDbType.Int64;
            if(floatTypes.Contains(dataTypeName.ToUpperInvariant())) return MySqlDbType.Float;
            if(doubleTypes.Contains(dataTypeName.ToUpperInvariant())) return MySqlDbType.Double;
            if(decimalTypes.Contains(dataTypeName.ToUpperInvariant())) return MySqlDbType.Decimal;
            if(dateTypes.Contains(dataTypeName.ToUpperInvariant())) return MySqlDbType.Date;
            if(timestampTypes.Contains(dataTypeName.ToUpperInvariant())) return MySqlDbType.Timestamp;

            throw new ArgumentException(string.Format("Unable to find matching MySqlDbType for '{0}'", dataTypeName));
        }
        private static Type GetRuntimeType(string dataTypeName)
        {
            if (stringTypes.Contains(dataTypeName.ToUpperInvariant())) return typeof(string);
            if (intTypes.Contains(dataTypeName.ToUpperInvariant())) return typeof(int);
            if (longTypes.Contains(dataTypeName.ToUpperInvariant())) return typeof(long);
            if (floatTypes.Contains(dataTypeName.ToUpperInvariant())) return typeof(float);
            if (doubleTypes.Contains(dataTypeName.ToUpperInvariant())) return typeof(double);
            if (decimalTypes.Contains(dataTypeName.ToUpperInvariant())) return typeof(decimal);
            if (dateTypes.Contains(dataTypeName.ToUpperInvariant())) return typeof(DateTime);
            if (timestampTypes.Contains(dataTypeName.ToUpperInvariant())) return typeof(DateTime);


            throw new ArgumentException(string.Format("Unable to find matching RuntimeType for '{0}'", dataTypeName));
        }

        public override DbProviderFactory DbProviderFactory {
            get { return MySqlDbProviderFactory; }
        }

        public override string IdentitySelectStatement
        {
            get { return "SELECT @@identity;"; }
        }

        public override Dictionary<string, DbParameterAccessor> GetAllProcedureParameters(string procName)
        {
            throw new NotImplementedException();
            /*
String[] qualifiers = NameNormaliser.normaliseName(procName).split("\\.");
		String qry = " select type,param_list,returns from mysql.proc where ";
		if (qualifiers.length == 2) {
			qry += " lower(db)=? and lower(name)=? ";
		} else {
			qry += " (db=database() and lower(name)=?)";
		}
		
		PreparedStatement dc=currentConnection.prepareStatement(qry);
		for (int i = 0; i < qualifiers.length; i++) {
			dc.setString(i+1,NameNormaliser.normaliseName(qualifiers[i]));
		}
		ResultSet rs=dc.executeQuery();		
		if (!rs.next()) {
			throw new SQLException("Unknown procedure "   +procName );
		}
		Map<String, DbParameterAccessor>
			allParams = new HashMap<String, DbParameterAccessor>();
		String type=rs.getString(1);
		String paramList=rs.getString(2);
		String returns=rs.getString(3);
		rs.close();
		int position=0;
		for (String param: paramList.split(",")){
			StringTokenizer s=new StringTokenizer(param.trim().toLowerCase()," ()");
			String token=s.nextToken();
			int direction=DbParameterAccessor.INPUT;
			
			if (token.equals("inout")) {
				direction=DbParameterAccessor.INPUT_OUTPUT;
				token=s.nextToken();
			}				
			if (token.equals("in")) {
					token=s.nextToken();
			}
			else if (token.equals("out")){
				direction=DbParameterAccessor.OUTPUT;
				token=s.nextToken();
			}
			String paramName=token;
			String dataType=s.nextToken();
			
			DbParameterAccessor dbp=new DbParameterAccessor (paramName,direction,
				getSqlType(dataType), getJavaClass(dataType), position++);			
			allParams.put(NameNormaliser.normaliseName(paramName),
					dbp);
		}
		if ("FUNCTION".equals(type)){
			StringTokenizer s=new StringTokenizer(returns.trim().toLowerCase()," ()");
			String dataType=s.nextToken();
			allParams.put("",
					new DbParameterAccessor ("",DbParameterAccessor.RETURN_VALUE,
							getSqlType(dataType), getJavaClass(dataType),-1));
		}
		return allParams;		
             */
        }

        public string GetAllColumnsSql(SchemaObjectName tableOrViewName)
        {
            StringBuilder query = new StringBuilder();
            
            query
                .Append("SELECT column_name, data_type, character_maximum_length as direction ")
                .Append("FROM information_schema.columns ")
                .Append("WHERE ");

            if(tableOrViewName.HasSchema())
            {
                query.Append("LOWER(table_schema)=? AND LOWER(table_name)=? ");
            }
            else
            {
                query.Append("(table_schema=database() AND LOWER(table_name)=?) ");
            }

            query.Append("ORDER BY ordinal_position");

            return query.ToString();
        }

        public override Dictionary<string, DbParameterAccessor> GetAllColumns(string tableOrViewName)
        {
            String[] qualifiers = NameNormaliser.NormaliseName(tableOrViewName).Split('.');
            string qry = GetAllColumnsSql(SchemaObjectName.Parse(qualifiers));
            return readIntoParams(qualifiers, qry);
        }

       

        private Dictionary<string, DbParameterAccessor> readIntoParams(string[] qualifiers, string query)
        {
            throw new NotImplementedException();
        }

        /*
         private Map<String, DbParameterAccessor> readIntoParams(String[] queryParameters, String query) 
		throws SQLException{
		PreparedStatement dc=currentConnection.prepareStatement(query);
		for (int i = 0; i < queryParameters.length; i++) {
			dc.setString(i+1,NameNormaliser.normaliseName(queryParameters[i]));
		}
		ResultSet rs=dc.executeQuery();		
		Map<String, DbParameterAccessor>
			allParams = new HashMap<String, DbParameterAccessor>();
		int position=0;
		while (rs.next()) {
			String paramName=rs.getString(1);			
			if (paramName==null) paramName="";
			String dataType = rs.getString(2);
			DbParameterAccessor dbp=new DbParameterAccessor (paramName,DbParameterAccessor.INPUT,
					getSqlType(dataType), getJavaClass(dataType), position++);
			allParams.put(NameNormaliser.normaliseName(paramName),
				dbp);
		}
		rs.close();
		return allParams;
         
         */

        public override bool SupportsReturnOnInsert {
            get {
                return false;
            }
        }
    }
}
