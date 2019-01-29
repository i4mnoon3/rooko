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
                "table",
                new Column("id", DbType.Int32, 0, true, true, true),
                new Column("some_column"),
                new Column("another_column")
               );
        }
        
        public override void Rollback()
        {
            DropTable("table");
        }
    }
    
    public class AddColumnToTable : Migration
    {
        public AddColumnToTable() : base("DA8965AD-31E3-4085-8125-8396758C4A82")
        {
        }
        
        public override void Migrate()
        {
            AddColumn("table", new Column("another_column"));
        }
        
        public override void Rollback()
        {
            RemoveColumn("table", "another_column");
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
                "table",
                new[] {
                    new KeyValuePair<string, object>("some_column", "some_value"),
                }
               );
        }
        
        public override void Rollback()
        {
            Delete(
                "table",
                new[] {
                    new KeyValuePair<string, object>("some_column", "some_value"),
                }
               );
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
                "table",
                new[] {
                    new KeyValuePair<string, object>("some_column", "new_value")
                },
                new[] {
                    new KeyValuePair<string, object>("id", 1)
                }
               );
        }
        
        public override void Rollback()
        {
            Update(
                "table",
                new[] {
                    new KeyValuePair<string, object>("some_column", "some_value")
                },
                new[] {
                    new KeyValuePair<string, object>("id", 1)
                }
               );
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
                "table",
                new[] {
                    new KeyValuePair<string, object>("id", "1"),
                }
               );
        }
        
        public override void Rollback()
        {
            Insert(
                "table",
                new[] {
                    new KeyValuePair<string, object>("id", "1"),
                    new KeyValuePair<string, object>("some_column", "some_value"),
                }
               );
        }
    }
}
