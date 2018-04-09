using System;
using System.Collections.Generic;
using System.Reflection;
using NUnit.Framework;
using Rooko.Core;

namespace Rooko.Tests
{
	[TestFixture]
	public class MigratorTests
	{
		List<Migration> migrations;
		
		[SetUp]
		public void Setup()
		{
			migrations = new List<Migration>();
			migrations.Add(new CreateUsers());
			migrations.Add(new AddUsernameToUsers());
			migrations.Add(new InsertRootUser());
		}
		
		[Test]
		public void TestMigrateFromAssembly()
		{
			Migrator m = new Migrator(Assembly.GetExecutingAssembly(), new SQLiteMigrationFormatter("data source=db.sqlite"));
			m.Migrating += (object sender, MigrationEventArgs e) => Console.WriteLine(e.Message);
			m.Migrate();
		}
		
		[Test]
		public void TestMigrate()
		{
			var m = new Migrator(migrations, new MigrationFormatterStub("data source=db.sqlite"));
			m.Migrating += (object sender, MigrationEventArgs e) => Console.WriteLine(e.Message);
			m.Migrate();
		}
		
		[Test]
		public void TestRollbackFromAssembly()
		{
			Migrator m = new Migrator(Assembly.GetExecutingAssembly(), new SQLiteMigrationFormatter("data source=db.sqlite"));
			m.Migrating += (object sender, MigrationEventArgs e) => Console.WriteLine(e.Message);
			m.Rollback();
		}
		
		[Test]
		public void TestRollback()
		{
			var m = new Migrator(migrations, new SQLiteMigrationFormatter("data source=db.sqlite"));
			m.Migrating += (object sender, MigrationEventArgs e) => Console.WriteLine(e.Message);
			m.Migrate();
		}
	}
}
