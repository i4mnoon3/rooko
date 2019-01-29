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
        _20190129212349CreateTableUsers createUsers;
        _20190129212511AddEmailToUsers addUsernameToUsers;
        _20190129212625InsertAdminToUsers insertAdminUser;
        
        [SetUp]
        public void Setup()
        {
            createUsers = new _20190129212349CreateTableUsers();
            createUsers.Migrating += delegate(object sender, MigrationEventArgs e) {
                Console.WriteLine(e.Message);
            };
            
            addUsernameToUsers = new _20190129212511AddEmailToUsers();
            addUsernameToUsers.Migrating += delegate(object sender, MigrationEventArgs e) {
                Console.WriteLine(e.Message);
            };
            
            insertAdminUser = new _20190129212625InsertAdminToUsers();
            insertAdminUser.Migrating += (object sender, MigrationEventArgs e) => Console.WriteLine(e.Message);
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
            insertAdminUser.Migrate();
        }
        
        [Test]
        public void TestInsertRootUserRollback()
        {
            insertAdminUser.Rollback();
        }
    }
}
