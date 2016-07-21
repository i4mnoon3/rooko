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
	
	public abstract class Migration
	{
		public Migration(string version)
		{
			this.Version = version;
		}
		
		public event EventHandler<MigrationEventArgs> Migrating;
		
		public string Version { get; set; }
		
		public MigrationRepository Repository { get; set; }
		
		public abstract void Migrate();
		
		public abstract void Rollback();
		
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
		
		public bool SchemaExists()
		{
			return Repository.SchemaExists();
		}
		
		public bool Exists()
		{
			return Repository.VersionExists(Version);
		}
		
		public void BuildSchema()
		{
			OnMigrating(new MigrationEventArgs("No schema found. Building schema_migrations..."));
			Repository.CreateTable(new Table("schema_migrations", new Column("id", "integer", true, true, true), new Column("version")));
		}
		
		protected void CreateTable(Table table)
		{
			OnMigrating(new MigrationEventArgs(string.Format("Creating table {0}...", table.Name)));
			Repository.CreateTable(table);
		}
		
		protected void DropTable(string tableName)
		{
			OnMigrating(new MigrationEventArgs(string.Format("Dropping table {0}...", tableName)));
			Repository.DropTable(tableName);
		}
		
		public void Save()
		{
			Repository.Save(this);
		}
		
		public void DeleteMigration()
		{
			Repository.Delete(this);
		}
		
		protected void AddColumn(string tableName, params Column[] columns)
		{
			string cols = "";
			int i = 1;
			foreach (var c in columns) {
				cols += c.Name;
				if (i++ < columns.Length) {
					cols += ", ";
				}
			}
			OnMigrating(new MigrationEventArgs(string.Format("Adding {0} to {1}...", cols, tableName)));
			Repository.AddColumns(tableName, columns);
		}
		
		public void RemoveColumn(string tableName, params string[] columns)
		{
			string cols = "";
			int i = 1;
			foreach (var c in columns) {
				cols += c;
				if (i++ < columns.Length) {
					cols += ", ";
				}
			}
			OnMigrating(new MigrationEventArgs(string.Format("Removing {0} from {1}...", cols, tableName)));
			Repository.RemoveColumns(tableName, columns);
		}
		
		public void Insert(string tableName, ICollection<KeyValuePair<string, object>> values)
		{
			OnMigrating(new MigrationEventArgs(string.Format("Inserting values to {0}...", tableName)));
			Repository.Insert(tableName, values);
		}
		
		public void Delete(string tableName, ICollection<KeyValuePair<string, object>> @where)
		{
			OnMigrating(new MigrationEventArgs(string.Format("Deleting values from {0}...", tableName)));
			Repository.Delete(tableName, @where);
		}
	}
}
