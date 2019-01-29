using System;
using System.Collections.Generic;
using System.Data;
using NUnit.Framework;
using Rooko.Core;

namespace Rooko.Tests
{
    [TestFixture]
    public class MigrationTests
    {
        CreateTable createUsers;
        AddColumnToTable addUsernameToUsers;
        InsertValueToTable insertRootUser;
        
        [SetUp]
        public void Setup()
        {
            createUsers = new CreateTable();
            createUsers.Migrating += delegate(object sender, MigrationEventArgs e) {
                Console.WriteLine(e.Message);
            };
            
            addUsernameToUsers = new AddColumnToTable();
            addUsernameToUsers.Migrating += delegate(object sender, MigrationEventArgs e) {
                Console.WriteLine(e.Message);
            };
            
            insertRootUser = new InsertValueToTable();
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
    
    public class CreateTable : Migration
    {
        public CreateTable() : base("E128A916-A06E-4142-B73D-DD0E6811D618")
        {
        }
        
        public override void Migrate()
        {
            CreateTable(
                "users",
                new Column("id", DbType.Int32, 0, true, true, true),
                new Column("name"),
                new Column("password")
               );
        }
        
        public override void Rollback()
        {
            DropTable("users");
        }
    }
    
    public class AddColumnToTable : Migration
    {
        public AddColumnToTable() : base("DA8965AD-31E3-4085-8125-8396758C4A82")
        {
        }
        
        public override void Migrate()
        {
            AddColumn("users", new Column("email"));
        }
        
        public override void Rollback()
        {
            RemoveColumn("users", "email");
        }
    }
    
    public class InsertValueToTable : Migration
    {
        public InsertValueToTable() : base("116AA4B6-5DEE-4621-8D39-DB0FC5D9110E")
        {
        }
        
        public override void Migrate()
        {
            Insert(
                "users",
                new[] {
                    new KeyValuePair<string, object>("name", "admin"),
                    new KeyValuePair<string, object>("password", "root"),
                }
               );
        }
        
        public override void Rollback()
        {
            Delete(
                "users",
                new[] {
                    new KeyValuePair<string, object>("name", "admin"),
                    new KeyValuePair<string, object>("password", "root"),
                });
        }
    }
    
    public class UpdateTable : Migration
    {
        public UpdateTable() : base("38AC0CD4-9306-41AA-A91C-86B0140DB101")
        {
        }
        
        public override void Migrate()
        {
            Update(
                "users",
                new[] {
                    new KeyValuePair<string, object>("email", "admin@localhost.com")
                },
                new[] {
                    new KeyValuePair<string, object>("name", "admin")
                });
        }
        
        public override void Rollback()
        {
            Update(
                "users",
                new[] {
                    new KeyValuePair<string, object>("email", "")
                },
                new[] {
                    new KeyValuePair<string, object>("name", "admin")
                });
        }
    }
    
    public class DeleteFromTable : Migration
    {
        public DeleteFromTable() : base("AACB1EBA-85DF-4DFD-A3AF-998F2BB18DE6")
        {
        }
        
        public override void Migrate()
        {
            Delete(
                "users",
                new[] {
                    new KeyValuePair<string, object>("name", "admin"),
                });
        }
        
        public override void Rollback()
        {
            Insert(
                "users",
                new[] {
                    new KeyValuePair<string, object>("name", "admin"),
                    new KeyValuePair<string, object>("password", "root"),
                    new KeyValuePair<string, object>("email", ""),
                });
        }
    }
}
