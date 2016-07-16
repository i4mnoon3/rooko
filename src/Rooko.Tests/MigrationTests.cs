/*
 * Created by SharpDevelop.
 * User: Dev
 * Date: 7/16/2016
 * Time: 9:27 AM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using NUnit.Framework;
using Rooko.Core;

namespace Rooko.Tests
{
	[TestFixture]
	public class MigrationTests
	{
		CreateItems m1;
		AddPriceToItems m2;
		
		[SetUpAttribute]
		public void Setup()
		{
			m1 = new CreateItems();
			m1.Migrating += delegate(object sender, MigrationEventArgs e) {
				Console.WriteLine(e.Message);
			};
			m1.Repository = new SQLiteMigrationRepository();
			
			m2 = new AddPriceToItems();
			m2.Migrating += delegate(object sender, MigrationEventArgs e) {
				Console.WriteLine(e.Message);
			};
			m2.Repository = new SQLiteMigrationRepository();
		}
		
		[Test]
		public void TestCreateItemsMigrate()
		{
			m1.Migrate();
		}
		
		[Test]
		public void TestCreateItemsRollback()
		{
			m1.Rollback();
		}
		
		[Test]
		public void TestAddPriceToItemsMigrate()
		{
			m2.Migrate();
		}
		
		[Test]
		public void TestAddPriceToItemsRollback()
		{
			m2.Rollback();
		}
	}
	
	public class CreateItems : Migration
	{
		public CreateItems() : base("E128A916-A06E-4142-B73D-DD0E6811D618")
		{
		}
		
		public override void Migrate()
		{
			CreateTable(
				"items",
				new Column("id", "integer", true, true, true),
				new Column("name"),
				new Column("description"),
				new Column("cost", "double")
			);
		}
		
		public override void Rollback()
		{
			DropTable("items");
		}
	}
	
	public class AddPriceToItems : Migration
	{
		public AddPriceToItems() : base("DA8965AD-31E3-4085-8125-8396758C4A82")
		{
		}
		
		public override void Migrate()
		{
			AddColumn("items", new Column("price", "double"));
		}
		
		public override void Rollback()
		{
			RemoveColumn("items", "price");
		}
	}
}
