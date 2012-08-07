//	<file>
//		<license></license>
//		<owner name="Ian Escarro" email="ian.escarro@gmail.com"/>
//	</file>

using System;
using System.Reflection;
using NUnit.Framework;
using Rooko.Core;

namespace Rooko.Tests
{
	[TestFixture]
	public class MigrationTests
	{
		[Test]
		public void TestMigrate()
		{
			Migrator m = new Migrator(Assembly.LoadFile(@"C:\ian\projects\nbooks\trunk\bin\NBooks.Core.dll"), new MySQLMigrationRepository());
			m.Migrate();
		}
		
		[Test]
		public void TestRollback()
		{
			Migrator m = new Migrator(Assembly.LoadFile(@"C:\ian\projects\nbooks\trunk\bin\NBooks.Core.dll"), new MySQLMigrationRepository());
			m.Rollback();
		}
		
		[Test]
		public void a()
		{
			var m = new CreateItems();
			m.Repository = new MySQLMigrationRepository();
			m.Migrate();
		}
		
		[Test]
		public void b()
		{
			var m =  new CreateItems();
			m.Repository = new MySQLMigrationRepository();
			m.Rollback();
		}
	}
	
	public class CreateItems : Migration
	{
		public CreateItems() : base("E128A916-A06E-4142-B73D-DD0E6811D618")
		{
		}
		
		public override void Migrate()
		{
			CreateTable(
				"items",
				new Column("id", "integer", true, true, true),
				new Column("name")
			);
		}
		
		public override void Rollback()
		{
			DropTable("items");
		}
	}
}
