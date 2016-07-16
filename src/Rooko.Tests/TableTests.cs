/*
 * Created by SharpDevelop.
 * User: Dev
 * Date: 7/16/2016
 * Time: 9:29 AM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
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
		public void TestMethod()
		{
			Assert.AreEqual(4, t.Columns.Count);
		}
	}
}
