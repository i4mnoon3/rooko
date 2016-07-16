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
		public MigrationEventArgs(string message)
		{
			this.Message = message;
		}
		
		public Migration Migration { get; set; }
		
		public string Message { get; set; }
	}
	
	public class Migration
	{
		public Migration(string version)
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
			if (!Repository.VersionExists(Version)) {
				OnMigrating(new MigrationEventArgs(string.Format("Creating table {0}", table.Name)));
				Repository.CreateTable(table);
				Repository.Save(this);
			}
		}
		
		protected void DropTable(string tableName)
		{
			if (Repository.VersionExists(Version)) {
				OnMigrating(new MigrationEventArgs(string.Format("Dropping table {0}", tableName)));
				Repository.DropTable(tableName);
				Repository.Delete(this);
			}
		}
		
		protected void AddColumn(string tableName, params Column[] columns)
		{
			if (!Repository.VersionExists(Version)) {
				string cols = "";
				int i = 0;
				foreach (var c in columns) {
					cols += c.Name;
					if (i++ < columns.Length - 1) {
						cols += ", ";
					}
				}
				OnMigrating(new MigrationEventArgs(string.Format("Adding {0} to {1}", cols, tableName)));
			}
		}
		
		public void RemoveColumn(string tableName, params string[] columns)
		{
			if (!Repository.VersionExists(Version)) {
				string cols = "";
				int i = 0;
				foreach (var c in columns) {
					cols += c;
					if (i++ < columns.Length - 1) {
						cols += ", ";
					}
				}
				OnMigrating(new MigrationEventArgs(string.Format("Removing {0} from {1}", cols, tableName)));
			}
		}
	}
}
