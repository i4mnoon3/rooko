//	<file>
//		<license></license>
//		<owner name="Ian Escarro" email="ian.escarro@gmail.com"/>
//	</file>

using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SQLite;
using System.Reflection;
using MySql.Data.MySqlClient;

namespace Rooko.Core
{
	public class MigrationEventArgs : EventArgs
	{
		public MigrationEventArgs(Migration migration)
		{
			this.Migration = migration;
		}
		
		public Migration Migration { get; set; }
	}
	
	public class Migration : Table
	{
		public Migration(string version) : base("schema_migrations", new Column("id", "integer", true, true, true), new Column("version"))
		{
			this.Version = version;
		}
		
		public event EventHandler<MigrationEventArgs> Migrating;
		
		public string Version { get; set; }
		
		public IMigrationRepository Repository { get; set; }
		
		public virtual void Migrate()
		{
		}
		
		public virtual void Rollback()
		{
		}
		
		protected virtual void OnMigrating(MigrationEventArgs e)
		{
			if (Migrating != null) {
				Migrating(this, e);
			}
		}
		
		protected void CreateTable(string tableName, params Column[] columns)
		{
			CreateTable(new Table(tableName, columns));
		}
		
		protected void CreateTable(Table table)
		{
			if (!Repository.Exists(Name)) {
				Repository.Create(this);
			}
			OnMigrating(new MigrationEventArgs(this));
			Repository.Create(table);
			Repository.Save(this);
		}
		
		protected void DropTable(string tableName)
		{
			Repository.Drop(tableName);
			Repository.Delete(this);
		}
	}
}
