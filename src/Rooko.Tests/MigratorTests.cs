using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
using MyProj.Migrations;
using NUnit.Framework;
using Rooko.Core;

namespace Rooko.Tests
{
    [TestFixture]
    public class MigratorTests
    {
        List<Migration> migrations;
        List<IMigrationFormatter> formatters;
        
        [SetUp]
        public void Setup()
        {
            migrations = new List<Migration>();
            migrations.Add(new _20190129212349CreateTableUsers());
            migrations.Add(new _20190129212511AddEmailToUsers());
            migrations.Add(new _20190129212625InsertAdminToUsers());
            migrations.Add(new _20190129212903UpdateAdminEmail());
            migrations.Add(new _20190129213034DeleteUserAdmin());
        }
        
        [Test]
        [Ignore]
        public void TestMigrateFromAssembly()
        {
            Migrator m = new Migrator(Assembly.GetExecutingAssembly(), new MigrationFormatterStub());
            m.Migrating += (object sender, MigrationEventArgs e) => Console.WriteLine(e.Message);
            m.Migrate();
        }
        
        [Test]
        [Ignore]
        public void TestRollbackFromAssembly()
        {
            Migrator m = new Migrator(Assembly.GetExecutingAssembly(), new MigrationFormatterStub());
            m.Migrating += (object sender, MigrationEventArgs e) => Console.WriteLine(e.Message);
            m.Rollback();
        }
        
        [Test]
        public void TestMigrate()
        {
            var m = new Migrator(migrations, new MigrationRepositoryStub());
            m.Migrating += (object sender, MigrationEventArgs e) => Console.WriteLine(e.Message);
            m.Migrate();
        }
        
        [Test]
        public void TestRollback()
        {
            var m = new Migrator(migrations, new MigrationRepositoryStub());
            m.Migrating += (object sender, MigrationEventArgs e) => Console.WriteLine(e.Message);
            m.Rollback();
            m.Rollback();
            m.Rollback();
            m.Rollback();
            m.Rollback();
        }
    }
    
    public class MigrationFormatterStub : IMigrationFormatter
    {
        public IDbConnection CreateConnection()
        {
            throw new NotImplementedException();
        }
        
        public string CreateTable(Table table)
        {
            throw new NotImplementedException();
        }
        
        public string DropTable(string tableName)
        {
            throw new NotImplementedException();
        }
        
        public string AddColumn(string tableName, params Column[] columns)
        {
            throw new NotImplementedException();
        }
        
        public string DropColumn(string tableName, params string[] columns)
        {
            throw new NotImplementedException();
        }
        
        public string Insert(string tableName, ICollection<KeyValuePair<string, object>> values)
        {
            throw new NotImplementedException();
        }
        
        public string Delete(string tableName, ICollection<KeyValuePair<string, object>> @where)
        {
            throw new NotImplementedException();
        }
        
        public string Update(string tableName, ICollection<KeyValuePair<string, object>> values, ICollection<KeyValuePair<string, object>> @where)
        {
            throw new NotImplementedException();
        }
        
        public string CheckSchema()
        {
            throw new NotImplementedException();
        }
        
        public string CreateSchema()
        {
            throw new NotImplementedException();
        }
    }
    
    public class MigrationRepositoryStub : IMigrationRepository
    {
        bool schemaExists;
        List<Migration> migrations;
        
        public MigrationRepositoryStub() : this(new List<Migration>())
        {
        }
        
        public MigrationRepositoryStub(List<Migration> migrations) : this(false, migrations)
        {
        }
        
        public MigrationRepositoryStub(bool schemaExists, List<Migration> migrations)
        {
            this.schemaExists = schemaExists;
            this.migrations = migrations;
        }
        
        public bool SchemaExists()
        {
            return schemaExists;
        }
        
        public bool VersionExists(string version)
        {
            return migrations.Find(x => x.Version == version) != null;
        }
        
        public void SaveMigration(Migration migration)
        {
        }
        
        public void BuildSchema()
        {
        }
        
        public string ReadLatestVersion()
        {
            return migrations[migrations.Count - 1].Version;
        }
        
        public void DeleteMigration(Migration migration)
        {
            migrations.Remove(migration);
        }
        
        public void Delete(string tableName, ICollection<KeyValuePair<string, object>> where)
        {
        }
        
        public void Update(string tableName, ICollection<KeyValuePair<string, object>> values, ICollection<KeyValuePair<string, object>> where)
        {
        }
        
        public void Insert(string tableName, ICollection<KeyValuePair<string, object>> values)
        {
        }
        
        public void AddColumns(string tableName, params Column[] columns)
        {
        }
        
        public void RemoveColumns(string tableName, params string[] columns)
        {
        }
        
        public void CreateTable(Table table)
        {
        }
        
        public void DropTable(string tableName)
        {
        }
    }
}
