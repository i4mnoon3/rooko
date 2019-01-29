using System;
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
            t.AddColumn("id", "integer", true, true, true);
            t.AddColumn("name");
            t.AddColumn("address");
            t.AddColumn("phone");
            t.AddColumn("email");
            t.AddColumn("notes");
        }
        
        
        [Test]
        public void TestMethod()
        {
            var s = f.CreateTable(t);
            Assert.AreEqual("", s);
        }
    }
}
