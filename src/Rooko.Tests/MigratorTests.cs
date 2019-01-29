using System;
using System.Collections.Generic;
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
        IMigrationFormatter sqlMigrationFormatter = new SqlMigrationFormatter(@"Server=.\SQLEXPRESS;Database=test;Trusted_Connection=True;");
        
        [SetUp]
        public void Setup()
        {
            migrations = new List<Migration>();
            migrations.Add(new CreateTable());
            migrations.Add(new AddColumnToTable());
            migrations.Add(new InsertValueToTable());
            migrations.Add(new UpdateTable());
//            migrations.Add(new DeleteFromTable());
            
            formatters = new List<IMigrationFormatter>();
            formatters.Add(sqlMigrationFormatter);
            formatters.Add(new SQLiteMigrationFormatter("data source=db.sqlite"));
        }
        
        [Test]
        [Ignore]
        public void TestMigrateFromAssembly()
        {
            foreach (var f in formatters) {
                Migrator m = new Migrator(Assembly.GetExecutingAssembly(), f);
                m.Migrating += (object sender, MigrationEventArgs e) => Console.WriteLine(e.Message);
                m.Migrate();
            }
        }
        
        [Test]
        [Ignore]
        public void TestRollbackFromAssembly()
        {
            foreach (var f in formatters) {
                Migrator m = new Migrator(Assembly.GetExecutingAssembly(), f);
                m.Migrating += (object sender, MigrationEventArgs e) => Console.WriteLine(e.Message);
                m.Rollback();
            }
        }
        
        [Test]
        public void TestMigrate()
        {
            var m = new Migrator(migrations, sqlMigrationFormatter);
            m.Migrating += (object sender, MigrationEventArgs e) => Console.WriteLine(e.Message);
            m.Migrate();
        }
        
        [Test]
        public void TestRollback()
        {
            var m = new Migrator(migrations, sqlMigrationFormatter);
            m.Migrating += (object sender, MigrationEventArgs e) => Console.WriteLine(e.Message);
            m.Rollback();
            m.Rollback();
            m.Rollback();
            m.Rollback();
            m.Rollback();
        }
    }
    
    public class MigrationFormatterStub : IMigrationRepository
    {
        bool schemaExists;
        List<Migration> migrations;
        
        public MigrationFormatterStub() : this(new List<Migration>())
        {
        }
        
        public MigrationFormatterStub(List<Migration> migrations) : this(false, migrations)
        {
        }
        
        public MigrationFormatterStub(bool schemaExists, List<Migration> migrations)
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
