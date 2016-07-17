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
		SqlTableFormatter s = new SqlTableFormatter();
		MySQLTableFormatter m = new MySQLTableFormatter();
		
		[SetUp]
		public void Setup()
		{
			t = new Table("users");
			t.AddColumn("id", "integer", true, true, true);
			t.AddColumn("name");
			t.AddColumn("password");
		}
		
		[Test]
		public void TestGetCreateTable()
		{
			Console.WriteLine(s.GetCreateTable(t));
			Console.WriteLine(s.GetDropTable("users"));
		}
		
		[Test]
		public void TestGetAddColumn()
		{
			Console.WriteLine(s.GetAddColumn("users", new Column("username"), new Column("salt")));
			Console.WriteLine(s.GetDropColumn("users", "username", "salt"));
		}
		
		[Test]
		public void TestGetInsert()
		{
			Console.WriteLine(s.GetInsert("users", new Column { Name = "username", Value = "admin" }, new Column { Name = "password", Value = "root" }));
			Console.WriteLine(s.GetDelete("users", new Column { Name = "username", Value = "admin" }, new Column { Name = "password", Value = "root" }));
		}
	}
}
