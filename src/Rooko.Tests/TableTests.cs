using System;
using NUnit.Framework;
using Rooko.Core;

namespace Rooko.Tests
{
	[TestFixture]
	public class TableTests
	{
		Table t;
		
		[SetUp]
		public void Setup()
		{
			t = new Table("items");
			t.AddColumn("id", "integer", true, true, true);
			t.AddColumn("name");
			t.AddColumn("description");
			t.AddColumn("price", "double");
		}
		
		[Test]
		public void TestProperties()
		{
			Assert.AreEqual("items", t.Name);
			var c = t.Columns[0];
			Assert.IsTrue(c.IsPrimaryKey);
			Assert.AreEqual(4, t.Columns.Count);
		}
	}
}
