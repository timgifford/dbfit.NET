package dbfit;

import java.sql.SQLException;

import org.junit.After;
import org.junit.Assert;
import org.junit.Before;
import org.junit.BeforeClass;
import org.junit.Test;

import dbfit.environment.DerbyEnvironment;
import dbfit.environment.EmbeddedDerbyEnvironment;
import fit.Fixture;

/**
 * Test the {@link DerbyEnvironment} using the {@link EmbeddedDerbyEnvironment}
 * subclass.
 * 
 * @author P&aring;l Brattberg, pal.brattberg@acando.com
 */
public class DerbyTestIntegrationTest {
	private static DerbyEnvironment environment;
	private DatabaseTest tester = null;
	private static final String CONNECTION_STRING = "jdbc:derby:testdb";

	@BeforeClass
	public static void beforeClass() throws SQLException {
		environment = new EmbeddedDerbyEnvironment();
		environment.connect(CONNECTION_STRING + ";create=true");
		Assert.assertNotNull(environment.getConnection());
	}

	@Before
	public void beforeTest() throws SQLException {
		tester = new DerbyTest();
		tester.connect(CONNECTION_STRING);
	}

	@After
	public void afterTest() throws SQLException {
		tester.rollback();
		tester.close();
	}

	@Test
	public void testConnectingToDatabaseShouldWork() throws SQLException {
		Assert.assertTrue(tester.environment.getConnection().isValid(1));
	}

	@Test
	public void testCreateTableShouldWork() throws SQLException {
		Fixture result = tester.execute("CREATE TABLE DERBYFITTEST "
				+ "(MYID INT NOT NULL, NOTE VARCHAR (30) NOT NULL, COUNTER INT)");
		System.out.println(result.counts());
		for (String table : tester.environment.getAllColumns("derbyfittest").keySet()) {
			System.out.println("table: " + table);
		}
		// System.out.println(tester..getAllColumns("DERBYFITTEST"));
		// Assert.assertNotNull(environment.getAllColumns("DERBYFITTEST"));
	}
}
