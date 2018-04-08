using System;
using System.Collections.Generic;
using NUnit.Framework;
using Rooko.Core;

namespace Rooko.Tests
{
	[TestFixture]
	public class MigrationTests
	{
		CreateUsers createUsers;
		AddUsernameToUsers addUsernameToUsers;
		InsertRootUser insertRootUser;
		
		[SetUp]
		public void Setup()
		{
			createUsers = new CreateUsers();
			createUsers.Migrating += delegate(object sender, MigrationEventArgs e) {
				Console.WriteLine(e.Message);
			};
			
			addUsernameToUsers = new AddUsernameToUsers();
			addUsernameToUsers.Migrating += delegate(object sender, MigrationEventArgs e) {
				Console.WriteLine(e.Message);
			};
			
			insertRootUser = new InsertRootUser();
			insertRootUser.Migrating += (object sender, MigrationEventArgs e) => Console.WriteLine(e.Message);
		}
		
		[Test]
		public void TestCreateUsersMigrate()
		{
			createUsers.Migrate();
		}
		
		[Test]
		public void TestCreateUsersRollback()
		{
			createUsers.Rollback();
		}
		
		[Test]
		public void TestAddUsernameToUsersMigrate()
		{
			addUsernameToUsers.Migrate();
		}
		
		[Test]
		public void TestAddUsernameToUsersRollback()
		{
			addUsernameToUsers.Rollback();
		}
		
		[Test]
		public void TestInsertRootUserMigrate()
		{
			insertRootUser.Migrate();
		}
		
		[Test]
		public void TestInsertRootUserRollback()
		{
			insertRootUser.Rollback();
		}
	}
	
	public class CreateUsers : Migration
	{
		public CreateUsers() : base("E128A916-A06E-4142-B73D-DD0E6811D618")
		{
		}
		
		public override void Migrate()
		{
			CreateTable(
				"users",
				new Column("id", "integer", true, true, true),
				new Column("name"),
				new Column("password")
			);
		}
		
		public override void Rollback()
		{
			DropTable("users");
		}
	}
	
	public class AddUsernameToUsers : Migration
	{
		public AddUsernameToUsers() : base("DA8965AD-31E3-4085-8125-8396758C4A82")
		{
		}
		
		public override void Migrate()
		{
			AddColumn("users", new Column("username"));
		}
		
		public override void Rollback()
		{
			RemoveColumn("users", "username");
		}
	}
	
	public class InsertRootUser : Migration
	{
		public InsertRootUser() : base("116AA4B6-5DEE-4621-8D39-DB0FC5D9110E")
		{
		}
		
		public override void Migrate()
		{
			Insert(
				"users",
				new[] {
					new KeyValuePair<string, object>("username", "admin"),
					new KeyValuePair<string, object>("password", "root")
				}
			);
		}
		
		public override void Rollback()
		{
			Delete(
				"users",
				new[] {
					new KeyValuePair<string, object>("username", "admin"),
					new KeyValuePair<string, object>("password", "root")
				}
			);
		}
	}
}
