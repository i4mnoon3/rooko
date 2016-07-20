//	<file>
//		<license></license>
//		<owner name="Ian Escarro" email="ian.escarro@gmail.com"/>
//	</file>

using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Data.SQLite;

namespace Rooko.Core
{
	// migrate "..\src\Rooko.Tests\bin\Debug\Rooko.Tests.dll" "data source=db.sqlite" "System.Data.SQLite"
	public class BaseSQLiteRepository
	{
		protected SQLiteConnection connection;
		
		public BaseSQLiteRepository(string connectionString)
		{
			connection = new SQLiteConnection(connectionString);
		}
		
		protected void OpenConnection()
		{
			if (connection.State == ConnectionState.Closed) {
				connection.Open();
			}
		}
		
		protected void CloseConnection()
		{
			if (connection.State == ConnectionState.Open) {
				connection.Close();
			}
		}
		
		public void ExecuteNonQuery(string query, params SQLiteParameter[] paramz)
		{
			try {
				OpenConnection();
				SQLiteCommand cmd = new SQLiteCommand(query, connection);
				cmd.Parameters.AddRange(paramz);
				cmd.ExecuteNonQuery();
			} catch {
				throw;
			} finally {
				CloseConnection();
			}
		}
		
		public SQLiteDataReader ExecuteReader(string query, params SQLiteParameter[] paramz)
		{
			try {
				OpenConnection();
				SQLiteCommand cmd = new SQLiteCommand(query, connection);
				cmd.Parameters.AddRange(paramz);
				return cmd.ExecuteReader();
			} catch {
				throw;
			}
		}
	}
	
	public class SQLiteMigrationRepository : BaseSQLiteRepository, IMigrationRepository
	{
		SQLiteMigrationFormatter f = new SQLiteMigrationFormatter();
		
		public SQLiteMigrationRepository(string connectionString) : base(connectionString)
		{
		}
		
		public void CreateTable(Table table)
		{
			ExecuteNonQuery(f.GetCreateTable(table));
		}
		
		public bool SchemaExists()
		{
			try {
				OpenConnection();
				string query = "SELECT name FROM sqlite_master WHERE type='table' AND name='schema_migrations'";
				using (var r = ExecuteReader(query)) {
					if (r.Read()) {
						return true;
					}
				}
				return false;
			} catch {
				throw;
			} finally {
				CloseConnection();
			}
		}
		
		public bool VersionExists(string version)
		{
			try {
				OpenConnection();
				string query = string.Format("select * from schema_migrations where version = '{0}'", version);
				using (var reader = ExecuteReader(query)) {
					if (reader.Read()) {
						return true;
					}
				}
				return false;
			} catch {
				throw;
			} finally {
				CloseConnection();
			}
		}
		
		public Migration ReadLatest()
		{
			try {
				OpenConnection();
				string query = string.Format("select * from schema_migrations order by id desc");
				Migration migration = null;
				using (var reader = ExecuteReader(query)) {
					if (reader.Read()) {
						migration = new Migration(reader.GetString(1));
					}
				}
				return migration;
			} catch {
				throw;
			} finally {
				CloseConnection();
			}
		}
		
		public void Save(Migration migration)
		{
			string query = string.Format("insert into schema_migrations(version) values('{0}')", migration.Version);
			ExecuteNonQuery(query);
		}
		
		public void Delete(Migration migration)
		{
			string query = string.Format("delete from schema_migrations where version = '{0}'", migration.Version);
			ExecuteNonQuery(query);
		}
		
		public void DropTable(string tableName)
		{
			ExecuteNonQuery(f.GetDropTable(tableName));
		}
		
		public void AddColumns(string tableName, params Column[] columns)
		{
			string cols = "";
			foreach (var c in columns) {
				cols += c.Name + " " + c.Type;
			}
			string query = string.Format("alter table {0} add {1}", tableName, cols);
			ExecuteNonQuery(query);
		}
		
		public void RemoveColumns(string tableName, params string[] columns)
		{
			string query = string.Format("alter table {0} drop column", tableName);
			ExecuteNonQuery(query);
		}
		
		public void Insert(string tableName, ICollection<KeyValuePair<string, object>> values)
		{
			string query = string.Format(@"");
			ExecuteNonQuery(query);
		}
		
		public void Delete(string tableName, ICollection<KeyValuePair<string, object>> where)
		{
			throw new NotImplementedException();
		}
		
		public void Update(string tableName, ICollection<KeyValuePair<string, object>> values, ICollection<KeyValuePair<string, object>> where)
		{
			throw new NotImplementedException();
		}
	}
	
	public class SQLiteMigrationFormatter : IMigrationFormatter
	{
		public string GetCreateTable(Table table)
		{
			string cols = "";
			int i = 0;
			foreach (var c in table.Columns) {
				string notNull = c.NotNull ? " not null" : "";
				string primaryKey = c.PrimaryKey ? " primary key" : "";
				cols += string.Format("  {0} {1}{2}{3}", c.Name, c.Type, notNull, primaryKey);
				cols += i++ < table.Columns.Count - 1 ? "," : "";
				cols += Environment.NewLine;
			}
			return string.Format(@"create table {0}(
{1});", table.Name, cols);
		}
		
		public string GetDropTable(string tableName)
		{
			return string.Format("drop table {0};", tableName);
		}
		
		public string GetAddColumn(string tableName, params Column[] columns)
		{
			string cols = "";
			int i = 0;
			foreach (var c in columns) {
				cols += string.Format("alter table {0} add {1} {2}", tableName, c.Name, c.Type);
				if (i++ < columns.Length - 1) {
					cols += ";" + Environment.NewLine;
				}
			}
			return cols;
		}
		
		public string GetDropColumn(string tableName, params string[] columns)
		{
			string cols = "";
			int i = 0;
			foreach (var c in columns) {
				cols += string.Format("alter table {0} remove column {1}", tableName, c);
				if (i++ < columns.Length - 1) {
					cols += ";" + Environment.NewLine;
				}
			}
			return cols;
		}
	}
}
