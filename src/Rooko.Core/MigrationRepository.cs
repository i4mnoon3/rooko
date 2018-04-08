//	<file>
//		<license></license>
//		<owner name="Ian Escarro" email="ian.escarro@gmail.com"/>
//	</file>

using System;
using System.Collections.Generic;
using System.Data;

namespace Rooko.Core
{
	public interface IMigrationFormatter
	{
		IDbConnection CreateConnection();
		
		string CreateTable(Table table);
		
		string DropTable(string tableName);
		
		string AddColumn(string tableName, params Column[] columns);
		
		string DropColumn(string tableName, params string[] columns);
		
		string Insert(string tableName, ICollection<KeyValuePair<string, object>> values);
		
		string Delete(string tableName, ICollection<KeyValuePair<string, object>> @where);
		
		string Update(string tableName, ICollection<KeyValuePair<string, object>> values, ICollection<KeyValuePair<string, object>> @where);
		
		string CheckSchema();
		
		string CreateSchema();
	}
	
	public class MigrationRepository : BaseMigrationRepository
	{
		IMigrationFormatter formatter;
		
		public MigrationRepository(IMigrationFormatter formatter) : base(formatter.CreateConnection())
		{
			this.formatter = formatter;
		}
		
		public bool VersionExists(string version)
		{
			string query = string.Format("SELECT 1 FROM SCHEMA_MIGRATIONS WHERE VERSION = '{0}'", version);
			using (var r = ExecuteReader(query)) {
				if (r.Read()) {
					return true;
				}
			}
			return false;
		}
		
		public string ReadLatestVersion()
		{
			string m = null;
			string query = string.Format("SELECT version FROM SCHEMA_MIGRATIONS ORDER BY id DESC");
			using (var r = ExecuteReader(query)) {
				if (r.Read()) {
					m = r.GetString(0);
				}
			}
			return m;
		}
		
		public void Save(Migration migration)
		{
			string query = string.Format("INSERT INTO schema_migrations(version) VALUES('{0}')", migration.Version);
			ExecuteNonQuery(query);
		}
		
		public void Delete(Migration migration)
		{
			string query = string.Format("DELETE FROM schema_migrations WHERE VERSION = '{0}'", migration.Version);
			ExecuteNonQuery(query);
		}
		
		public void BuildSchema()
		{
			ExecuteNonQuery(formatter.CreateSchema());
		}
		
		public bool SchemaExists()
		{
			using (var r = ExecuteReader(formatter.CheckSchema())) {
				if (r.Read()) {
					return true;
				}
			}
			return false;
		}
		
		public void CreateTable(Table table)
		{
			ExecuteNonQuery(formatter.CreateTable(table));
		}
		
		public void DropTable(string tableName)
		{
			ExecuteNonQuery(formatter.DropTable(tableName));
		}
		
		public void AddColumns(string tableName, params Column[] columns)
		{
			ExecuteNonQuery(formatter.AddColumn(tableName, columns));
		}
		
		public void RemoveColumns(string tableName, params string[] columns)
		{
			ExecuteNonQuery(formatter.DropColumn(tableName, columns));
		}
		
		public void Insert(string tableName, ICollection<KeyValuePair<string, object>> values)
		{
			ExecuteNonQuery(formatter.Insert(tableName, values));
		}
		
		public void Delete(string tableName, ICollection<KeyValuePair<string, object>> @where)
		{
			ExecuteNonQuery(formatter.Delete(tableName, @where));
		}
		
		public void Update(string tableName, ICollection<KeyValuePair<string, object>> values, ICollection<KeyValuePair<string, object>> @where)
		{
			ExecuteNonQuery(formatter.Update(tableName, values, @where));
		}
	}
	
	public class BaseMigrationRepository
	{
		IDbConnection connection;
		
		public BaseMigrationRepository(IDbConnection connection)
		{
			this.connection = connection;
		}
		
		public void OpenConnection()
		{
			if (connection.State == ConnectionState.Closed) {
				connection.Open();
			}
		}
		
		public void CloseConnection()
		{
			if (connection.State == ConnectionState.Open) {
				connection.Close();
			}
		}
		
		public void ExecuteNonQuery(string query, params IDbDataParameter[] parameters)
		{
			try {
				OpenConnection();
				IDbCommand cmd = connection.CreateCommand();
				cmd.CommandText = query;
				foreach (var p in parameters) {
					cmd.Parameters.Add(p);
				}
				cmd.ExecuteNonQuery();
			} catch {
				throw;
			} finally {
				CloseConnection();
			}
		}
		
		public IDataReader ExecuteReader(string query, params IDbDataParameter[] parameters)
		{
			try {
				OpenConnection();
				IDbCommand cmd = connection.CreateCommand();
				cmd.CommandText = query;
				foreach (var p in parameters) {
					cmd.Parameters.Add(p);
				}
				return cmd.ExecuteReader();
			} catch {
				throw;
			}
		}
	}
}
