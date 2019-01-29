using System;
using System.Collections.Generic;
using System.Data;
using NUnit.Framework;
using Rooko.Core;

namespace Rooko.Tests
{
	[TestFixture]
	public class MySQLMigrationFormatterTests
	{
		Table table;
		IMigrationFormatter mySQLFormatter;
		
		[SetUp]
		public void Setup()
		{
			table = new Table("users");
			table.AddColumn("id", DbType.Int32, true, true, true);
			table.AddColumn("name");
			table.AddColumn("password");
			
			mySQLFormatter = new MySQLMigrationFormatter("");
		}
		
		[TearDown]
		public void Dispose()
		{
		}
		
		[Test]
		[Ignore()]
		public void TestCreateTable()
		{
			const string query = @"CREATE TABLE users(
	id integer not null primary key auto_increment,
	name varchar(255),
	password varchar(255)
);";
			Assert.AreEqual(query, mySQLFormatter.CreateTable(table));
			
		}
		
		[Test]
		public void TestDropTable()
		{
			const string expected = @"DROP TABLE users;";
			string actual = mySQLFormatter.DropTable("users");
			Assert.AreEqual(expected, actual);
		}
		
		[Test]
		[Ignore()]
		public void TestAddColumn()
		{
			const string expected = @"ALTER TABLE users ADD username varchar(255);
ALTER TABLE users ADD salt varchar(255);";
			string actual = mySQLFormatter.AddColumn("users", new Column("username"), new Column("salt"));
			Assert.AreEqual(expected, actual);
		}
		
		[Test]
		[Ignore()]
		public void TestDropColumn()
		{
			const string expected = @"ALTER TABLE users
DROP username, DROP salt;";
			string actual = mySQLFormatter.DropColumn("users", "username", "salt");
			Console.WriteLine(actual);
			Assert.AreEqual(expected, actual);
		}
		
		[Test]
		[Ignore()]
		public void TestInsert()
		{
			const string expected = @"INSERT INTO users(username, password)
VALUES('admin', 'root')";
			string actual = mySQLFormatter.Insert("users", new[] { new KeyValuePair<string, object>("username", "admin"), new KeyValuePair<string, object>("password", "root") });
			Assert.AreEqual(expected, actual);
		}
		
		[TestAttribute]
		public void TestDelete()
		{
			const string expected = @"DELETE FROM users WHERE username = 'admin' AND password = 'root'";
			string actual = mySQLFormatter.Delete("users", new[] { new KeyValuePair<string, object>("username", "admin"), new KeyValuePair<string, object>("password", "root") });
			Assert.AreEqual(expected, actual);
		}
		
		[Test]
		public void TestUpdate()
		{
			const string expected = @"UPDATE users SET password = 'r00t' WHERE username = 'admin'";
			string actual = mySQLFormatter.Update(
				"users",
				new[] { new KeyValuePair<string, object>("password", "r00t") },
				new[] { new KeyValuePair<string, object>("username", "admin") }
			);
			Assert.AreEqual(expected, actual);
		}
		
		[Test]
		[Ignore()]
		public void TestCheckSchema()
		{
			const string expected = @"SELECT 1 FROM INFORMATION_SCHEMA.TABLES
WHERE table_name = 'schema_migrations' AND table_schema = ''";
			string actual = mySQLFormatter.CheckSchema();
			Console.WriteLine(actual);
			Assert.AreEqual(expected, actual);
		}
	}
}
