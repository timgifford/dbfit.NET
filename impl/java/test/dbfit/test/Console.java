package dbfit.test;
//import java.sql.Connection;
//import java.sql.DriverManager;
//import java.sql.PreparedStatement;
//
//
//import com.microsoft.sqlserver.jdbc.SQLServerStatement;
//
//
public class Console {
//	public static void main(String ars[]) throws Exception{
//		String connstr="jdbc:sqlserver://localhost;database=FitNesseTestDB";
//		
//		Class.forName("com.microsoft.sqlserver.jdbc.SQLServerDriver");
//		Connection con = DriverManager.getConnection(connstr,"FitNesseUser","FitNesseUser");
//		con.setAutoCommit(false);
//		
//		PreparedStatement st=con.prepareStatement(
//				"Insert into users(username,name) values ('u1','u2')",
//				SQLServerStatement.RETURN_GENERATED_KEYS);
//		st.getGeneratedKeys();
//		st.execute();
//		con.rollback();
//	}
}
