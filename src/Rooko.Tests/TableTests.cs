using System;
using System.Data;
using NUnit.Framework;
using Rooko.Core;

namespace Rooko.Tests
{
    [TestFixture]
    public class TableTests
    {
        Table items;
        
        [SetUp]
        public void Setup()
        {
            items = new Table("items");
            items.AddColumn("id", DbType.Int32, true, true, true);
            items.AddColumn("name");
            items.AddColumn("description");
            items.AddColumn("price", DbType.Double);
        }
        
        [Test]
        public void TestProperties()
        {
            Assert.AreEqual("items", items.Name);
            var c = items.Columns[0];
            Assert.IsTrue(c.IsPrimaryKey);
            Assert.AreEqual(4, items.Columns.Count);
            Assert.AreEqual(4, items.ColumnNames.Count);
        }
    }
}
