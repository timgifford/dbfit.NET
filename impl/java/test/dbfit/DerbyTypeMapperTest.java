package dbfit;

import java.math.BigDecimal;
import java.sql.Date;
import java.sql.Time;
import java.sql.Timestamp;
import java.sql.Types;

import org.junit.AfterClass;
import org.junit.Assert;
import org.junit.BeforeClass;
import org.junit.Test;

import dbfit.environment.DerbyEnvironment;
import dbfit.environment.DerbyEnvironment.TypeMapper;

public class DerbyTypeMapperTest {
	private static TypeMapper mapper = null;

	@BeforeClass
	public static void beforeClass() {
		mapper = new DerbyEnvironment.DerbyTypeMapper();
	}

	@AfterClass
	public static void afterClass() {
		mapper = null;
	}

	@Test
	public void testUnsupportedTypes() {
		// blob and clob are not supported by dbfit afaik
		assertUnsupportedTypes("BLOB", "BINARY LARGE OBJECT");
		assertUnsupportedTypes("CLOB", "CHARACTER LARGE OBJECT");

		// these types are not supported by derby
		assertUnsupportedTypes("text", "DATETIME", "TINYINT", "MEDIUMINT",
				"INTEGER UNSIGNED", "INT UNSIGNED");

		// these are for catching bad coding
		assertUnsupportedTypes("", "INT .", "-integer", "\t", null,
				"X;VALUES 1;", "INT; DROP TABLE dbfit;");
	}

	@Test
	public void testDoubleTypes() {
		assertMappedTypes(Double.class, "DOUBLE PRECISION", "DOUBLE", "FLOAT");
		assertMappedTypes(Double.class, "Double Precision", "dOuBle", "flOAT");
		assertMappedTypes(Types.DOUBLE, "DOUBLE PRECISION", "DOUBLE", "FLOAT");
		assertMappedTypes(Types.DOUBLE, "Double Precision", "dOuBle", "flOAT");
	}

	@Test
	public void testIntegerTypes() {
		assertMappedTypes(Integer.class, "INTEGER", "int", " int ");
		assertMappedTypes(Types.INTEGER, "INTEGER", "int", " int ");
	}

	@Test
	public void testFloatTypes() {
		assertMappedTypes(Float.class, "real");
		assertMappedTypes(Types.FLOAT, "real");
	}

	@Test
	public void testShortTypes() {
		assertMappedTypes(Short.class, "SMALLINT");
		assertMappedTypes(Types.INTEGER, "SMALLINT");
	}

	@Test
	public void testTimeTypes() {
		assertMappedTypes(Time.class, "time");
		assertMappedTypes(Types.TIME, "time");
	}

	@Test
	public void testTimestampTypes() {
		assertMappedTypes(Timestamp.class, "TIMESTAMP");
		assertMappedTypes(Types.TIMESTAMP, "TIMESTAMP");
	}

	@Test
	public void testDateTypes() {
		assertMappedTypes(Date.class, "date");
		assertMappedTypes(Types.DATE, "date");
	}

	@Test
	public void testBigDecimalTypes() {
		assertMappedTypes(BigDecimal.class, "DECIMAL", "DEC", "NUMERIC");
		assertMappedTypes(Types.DECIMAL, "DECIMAL", "DEC", "NUMERIC");
	}

	@Test
	public void testLongTypes() {
		assertMappedTypes(Long.class, "bigint", "bigint not null");
		assertMappedTypes(Types.BIGINT, "bigint");
	}

	@Test
	public void testStringTypes() {
		assertMappedTypes(String.class, "char", "CHARACTER", "LONG VARCHAR",
				"VARCHAR", "CHAR VARYING", "CHARACTER VARYING",
				"LONG VARCHAR FOR BIT DATA", "VARCHAR FOR BIT DATA", "XML");
		assertMappedTypes(String.class, "char(20)", "CHARACTER(50)",
				"VARCHAR(20)");
		
		assertMappedTypes(Types.VARCHAR, "char", "CHARACTER", "LONG VARCHAR",
				"VARCHAR", "CHAR VARYING", "CHARACTER VARYING",
				"LONG VARCHAR FOR BIT DATA", "VARCHAR FOR BIT DATA", "XML");
		assertMappedTypes(Types.VARCHAR, "char(20)", "CHARACTER(50)",
				"VARCHAR(20)");
	}

	/**
	 * Assert that all supplied strings maps to the defined class.
	 */
	private void assertMappedTypes(Class<?> clazz, String... types) {
		for (String type : types) {
			Assert.assertEquals(clazz, mapper.getJavaClassForDBType(type));
		}
	}

	/**
	 * Assert that all supplied strings maps to the defined SQL Type constant.
	 */
	private void assertMappedTypes(int sqlConstant, String... types) {
		for (String type : types) {
			Assert.assertEquals(sqlConstant, mapper
					.getJDBCSQLTypeForDBType(type));
		}
	}

	/**
	 * Assert that all supplied strings are reported as unsupported.
	 */
	private void assertUnsupportedTypes(String... types) {
		for (String type : types) {
			try {
				Class<?> reply = mapper.getJavaClassForDBType(type);
				Assert.fail("Expected type '" + type
						+ "' to be unsupported but was reported as type: "
						+ reply);
			} catch (Exception e) {
				Assert.assertNotNull(e.getMessage());
			}
		}
	}
}
