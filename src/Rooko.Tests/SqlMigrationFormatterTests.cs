using System;
using System.Data;
using NUnit.Framework;
using Rooko.Core;

namespace Rooko.Tests
{
    [TestFixture]
    public class SqlMigrationFormatterTests
    {
        SqlMigrationFormatter f;
        Table t;
        
        [SetUp]
        public void Setup()
        {
            f = new SqlMigrationFormatter("Data Source=.;Initial Catalog=test;Integrated Security=True;");
            
            t = new Table("customers");
            t.AddColumn("id", DbType.Int32, true, true, true);
            t.AddColumn("name");
            t.AddColumn("address");
            t.AddColumn("phone");
            t.AddColumn("email");
            t.AddColumn("notes");
        }
        
        [Test]
        public void TestAddColumn()
        {
            var s = f.AddColumn("customers", "notes");
            Assert.AreEqual(
                "ALTER TABLE customers ADD\r\n" +
                "    notes VARCHAR(255)", s);
            s = f.AddColumn("customers", "notes", "status");
            Console.WriteLine(s);
            Assert.AreEqual(
                "ALTER TABLE customers ADD\r\n" +
                "    notes VARCHAR(255),\r\n" +
                "    status VARCHAR(255)", s);
        }
        
        [Test]
        public void TestDropTable()
        {
            var s = f.DropTable("customers");
            Assert.AreEqual("DROP TABLE customers", s);
        }
        
        [Test]
        public void TestCreateTable()
        {
            var s = f.CreateTable(t);
            Console.WriteLine(s);
                
            Assert.AreEqual(
                "CREATE TABLE customers(\r\n" +
                "    id INT NOT NULL PRIMARY KEY IDENTITY,\r\n" +
                "    name VARCHAR(255),\r\n" +
                "    address VARCHAR(255),\r\n" +
                "    phone VARCHAR(255),\r\n" +
                "    email VARCHAR(255),\r\n" +
                "    notes VARCHAR(255)\r\n" +
                ")", s);
        }
    }
}
