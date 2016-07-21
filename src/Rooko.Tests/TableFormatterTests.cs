//	<file>
//		<license></license>
//		<owner name="Ian Escarro" email="ian.escarro@gmail.com"/>
//	</file>

using System;
using System.Collections.Generic;
using NUnit.Framework;
using Rooko.Core;

namespace Rooko.Tests
{
	[TestFixture]
	public class TableFormatterTests
	{
		Table t;
//		SqlMigrationFormatter s = new SqlMigrationFormatter("");
//		MySQLMigrationFormatter m = new MySQLMigrationFormatter("");
		IMigrationFormatter f;
		
		[SetUp]
		public void Setup()
		{
			t = new Table("users");
			t.AddColumn("id", "integer", true, true, true);
			t.AddColumn("name");
			t.AddColumn("password");
			
			f = new MySQLMigrationFormatter("");
		}
		
		[TearDown]
		public void Dispose()
		{
		}
		
		[Test]
		public void TestGetCreateTable()
		{
			Console.WriteLine(f.GetCreateTable(t));
			Console.WriteLine(f.GetDropTable("users"));
		}
		
		[Test]
		public void TestGetAddColumn()
		{
			Console.WriteLine(f.GetAddColumn("users", new Column("username"), new Column("salt")));
			Console.WriteLine(f.GetDropColumn("users", "username", "salt"));
		}
		
		[Test]
		public void TestGetInsert()
		{
			Console.WriteLine(f.GetInsert("users", new [] { new KeyValuePair<string, object>("username", "admin"), new KeyValuePair<string, object>("password", "root") }));
		}
		
		[TestAttribute]
		public void TestGetDelete()
		{
			Console.WriteLine(f.GetDelete("users", new [] { new KeyValuePair<string, object>("username", "admin"), new KeyValuePair<string, object>("password", "root") }));
		}
		
		[Test]
		public void TestGetUpdate()
		{
			Console.WriteLine(
				f.GetUpdate(
					"users",
					new [] { new KeyValuePair<string, object>("password", "r00t") },
					new [] { new KeyValuePair<string, object>("username", "admin") }
				)
			);
		}
		
		[Test]
		public void TestGetCheckSchema()
		{
			Console.WriteLine(f.GetCheckSchema());
		}
	}
}
