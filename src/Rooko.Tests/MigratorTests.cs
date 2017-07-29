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
	public class MigratorTests
	{
		[Test]
		public void TestMigrate()
		{
//			Migrator m = new Migrator(Assembly.GetExecutingAssembly(), new SQLiteMigrationFormatter("data source=db.sqlite"));
//			m.Migrate();
		}
		
		[Test]
		public void TestRollback()
		{
//			Migrator m = new Migrator(Assembly.GetExecutingAssembly(), new SQLiteMigrationFormatter("data source=db.sqlite"));
//			m.Rollback();
		}
	}
}
