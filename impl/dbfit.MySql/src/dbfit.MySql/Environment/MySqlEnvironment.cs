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
    public class MySqlStoredProcedure
    {
        public MySqlStoredProcedure Parse(string parameterList)
        {
            throw new NotImplementedException();
        }
    }

    public class MySqlEnvironment : AbstractDbEnvironment
    {
        public override string ParameterPrefix
        {
            get { return "?"; }
        }

        public override DbCommand CreateCommand(string statement, CommandType commandType) {
            
            Console.WriteLine("CreateCommand: {0}", statement);

            DbCommand command = base.CreateCommand(statement, commandType);
            command.Prepare();

            return command;
        }
        public override string BuildInsertCommand(string tableName, DbParameterAccessor[] accessors) {
            string insertCommand = base.BuildInsertCommand(tableName, accessors);
            //insertCommand = paramNameRegex.Replace(insertCommand, "?");
            //string blah = Regex.Replace(insertCommand, paramNameRegex.ToString(), "?");
            Console.WriteLine("MySqlEnvironment.BuildInsertCommand:{0}", insertCommand);
            return insertCommand;
        }

        protected void AddInput(IDbCommand dbCommand, string name, object value)
        {
            IDbDataParameter dbParameter = dbCommand.CreateParameter();
            dbParameter.Direction = ParameterDirection.Input;
            dbParameter.ParameterName = string.Concat(ParameterPrefix, name);
            dbParameter.Value = (value ?? DBNull.Value);
            dbCommand.Parameters.Add(dbParameter);
        }

        protected override void AddInput(DbCommand dbCommand, string name, object value) {
            AddInput(dbCommand, name, value);
        }

        public IDbDataParameter BuildParameter(IDbCommand command, string name, object value)
        {
            throw new NotImplementedException();
        }

        protected override string GetConnectionString(string dataSource, string username, string password)
        {
            return string.Format("Server={0};Uid={1};Pwd={2};", dataSource, username, password );
        }

        protected override string GetConnectionString(string dataSource, string username, string password, string database)
        {
            return string.Format("Server={0};Database={3};Uid={1};Pwd={2};", dataSource, username, password, database);
        }

        public readonly Regex paramNameRegex = new Regex("\\?([A-Za-z0-9_]+)");
        protected override Regex ParamNameRegex
        {
            get { return paramNameRegex; }
        }

        private static readonly DbProviderFactory MySqlDbProviderFactory = new MySqlClientFactory();


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
                .Append("SELECT column_name, data_type, character_maximum_length as data_type_size ")
                .Append("FROM information_schema.columns ")
                .Append("WHERE ");

            if(tableOrViewName.HasSchema())
            {
                query.Append("LOWER(table_schema)=?dbname AND LOWER(table_name)=?tablename ");
            }
            else
            {
                query.Append("(table_schema=database() AND LOWER(table_name)=?tablename) ");
            }

            query.Append("ORDER BY ordinal_position");

            return query.ToString();
        }

        public override Dictionary<string, DbParameterAccessor> GetAllColumns(string tableOrViewName)
        {
            String[] qualifiers = NameNormaliser.NormaliseName(tableOrViewName).Split('.');
            SchemaObjectName name = SchemaObjectName.Parse(qualifiers);
            string qry = GetAllColumnsSql(name);
            return ReadIntoParams(name, qry);
        }



        private Dictionary<string, DbParameterAccessor> ReadIntoParams(SchemaObjectName tableOrViewname, string query)
        {
            Dictionary<string, DbParameterAccessor> accessorDictionary = new Dictionary<string, DbParameterAccessor>();
            int position = 0;
            DbCommand dc = CurrentConnection.CreateCommand();
            dc.Transaction = CurrentTransaction;
            dc.CommandText = query;
            dc.CommandType = CommandType.Text;
            
            AddSchemaAndTablenameParametersToCommand(dc, tableOrViewname);

            using(DbDataReader reader = dc.ExecuteReader())
            {
                while (reader.Read())
                {
                    DbParameterAccessor accessor = MySqlTypeConverter.BuildDbParameterAccessorFrom(reader, position++);
                    Console.WriteLine("DbParameter DbFieldName={0}", accessor.DbFieldName);
                    accessorDictionary.Add(accessor.DbFieldName.ToLowerInvariant(), accessor);
                }
            }

            return accessorDictionary;
        }

        public void AddSchemaAndTablenameParametersToCommand(IDbCommand dc, SchemaObjectName tableOrViewname)
        {
            if (tableOrViewname.HasSchema()) AddInput(dc, "dbname", tableOrViewname.Schema);
            AddInput(dc, "tablename", tableOrViewname.Name);
        }

        public override string[] ExtractParamNames(string commandText) {
            Console.WriteLine("ExtractParamNames: {0}",commandText);

            string[] paramNames = base.ExtractParamNames(commandText);
            foreach (string parameterName in paramNames)
            {
                Console.WriteLine("Parameter Name: {0}", parameterName);
            }
            return paramNames;
        }

        /*
         private Map<String, DbParameterAccessor> ReadIntoParams(String[] queryParameters, String query) 
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
