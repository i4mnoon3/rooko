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
		
		public event EventHandler<TableEventArgs> Inserting;
		
		public event EventHandler<TableEventArgs> TableCreate;
		
		public event EventHandler<TableEventArgs> TableDrop;
		
		public event EventHandler<TableEventArgs> ColumnAdd;
		
		public event EventHandler<TableEventArgs> ColumnRemove;
		
		public event EventHandler<TableEventArgs> Deleting;
		
		public event EventHandler<MigrationEventArgs> Migrating;
		
		public event EventHandler<TableEventArgs> Updating;
		
		protected virtual void OnUpdating(TableEventArgs e)
		{
			if (Updating != null) {
				Updating(this, e);
			}
		}
		
		public string Version { get; set; }
		
		public abstract void Migrate();
		
		public abstract void Rollback();
		
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
			
			var t = new Table(tableName);
			foreach (var c in columns) {
				t.AddColumn(c);
			}
			OnColumnRemove(new TableEventArgs(t));
		}
		
		public void Insert(string tableName, ICollection<KeyValuePair<string, object>> values)
		{
			OnMigrating(new MigrationEventArgs(string.Format("Inserting values to {0}...", tableName)));
			OnInserting(new TableEventArgs(new Table(tableName), values));
		}
		
		public void Delete(string tableName, ICollection<KeyValuePair<string, object>> @where)
		{
			OnMigrating(new MigrationEventArgs(string.Format("Deleting values from {0}...", tableName)));
			OnDeleting(new TableEventArgs(new Table(tableName), null, @where));
		}
		
		public void Update(string tableName, ICollection<KeyValuePair<string, object>> values, ICollection<KeyValuePair<string, object>> @where)
		{
			OnMigrating(new MigrationEventArgs(string.Format("Updating values to {0}...", tableName)));
			OnUpdating(new TableEventArgs(new Table(tableName), values, @where));
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
			OnMigrating(new MigrationEventArgs(string.Format("Creating table {0}...", table.Name)));
			OnTableCreate(new TableEventArgs(table));
		}
		
		protected void DropTable(string tableName)
		{
			OnMigrating(new MigrationEventArgs(string.Format("Dropping table {0}...", tableName)));
			OnTableDrop(new TableEventArgs(tableName));
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
			OnColumnAdd(new TableEventArgs(new Table(tableName, columns)));
		}
		
		protected virtual void OnDeleting(TableEventArgs e)
		{
			if (Deleting != null) {
				Deleting(this, e);
			}
		}
		
		protected virtual void OnColumnRemove(TableEventArgs e)
		{
			if (ColumnRemove != null) {
				ColumnRemove(this, e);
			}
		}
		
		protected virtual void OnColumnAdd(TableEventArgs e)
		{
			if (ColumnAdd != null) {
				ColumnAdd(this, e);
			}
		}
		
		protected virtual void OnTableDrop(TableEventArgs e)
		{
			if (TableDrop != null) {
				TableDrop(this, e);
			}
		}
		
		protected virtual void OnTableCreate(TableEventArgs e)
		{
			if (TableCreate != null) {
				TableCreate(this, e);
			}
		}
		
		protected virtual void OnInserting(TableEventArgs e)
		{
			if (Inserting != null) {
				Inserting(this, e);
			}
		}
	}
}
