//	<file>
//		<license></license>
//		<owner name="Ian Escarro" email="ian.escarro@gmail.com"/>
//	</file>

using System;
using NUnit.Framework;
using Rooko.Core;

namespace Rooko.Tests
{
	[TestFixture]
	public class TableFormatterTests
	{
		Table t;
		SqlMigrationFormatter s = new SqlMigrationFormatter();
		MySQLMigrationFormatter m = new MySQLMigrationFormatter();
		
		[SetUp]
		public void Setup()
		{
			t = new Table("users");
			t.AddColumn("id", "integer", true, true, true);
			t.AddColumn("name");
			t.AddColumn("password");
		}
		
		[TearDown]
		public void Dispose()
		{
		}
		
		[Test]
		public void TestGetCreateTable()
		{
			Console.WriteLine(s.GetCreateTable(t));
			Console.WriteLine(s.GetDropTable("users"));
		}
		
		[Test]
		public void TestGetAddColumn()
		{
			Console.WriteLine(s.GetAddColumn("users", new Column("username"), new Column("salt")));
			Console.WriteLine(s.GetDropColumn("users", "username", "salt"));
		}
		
		[Test]
		public void TestGetInsert()
		{
			Console.WriteLine(s.GetInsert("users", new Column { Name = "username", Value = "admin" }, new Column { Name = "password", Value = "root" }));
			Console.WriteLine(s.GetDelete("users", new Column { Name = "username", Value = "admin" }, new Column { Name = "password", Value = "root" }));
		}
	}
}
