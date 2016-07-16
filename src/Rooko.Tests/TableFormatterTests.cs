/*
 * Created by SharpDevelop.
 * User: Dev
 * Date: 7/16/2016
 * Time: 9:46 AM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using NUnit.Framework;
using Rooko.Core;

namespace Rooko.Tests
{
	[TestFixture]
	public class TableFormatterTests
	{
		Table t;
		SqLiteTableFormatter f = new SqLiteTableFormatter();
		
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
		public void TestGetCreateString()
		{
			Console.WriteLine(f.GetCreateString(t));
		}
		
		[Test]
		public void TestGetDropString()
		{
			Console.WriteLine(f.GetDropString("items"));
		}
		
		[Test]
		public void TestGetAddColumnString()
		{
			Console.WriteLine(f.GetAddColumnString("items", new Column("price", "double")));
		}
	}
}
