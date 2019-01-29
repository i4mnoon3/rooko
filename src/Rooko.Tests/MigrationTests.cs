using System;
using System.Collections.Generic;
using System.Data;
using MyProj.Migrations;
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
}
