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
		
		string GetCreateTable(Table table);
		
		string GetDropTable(string tableName);
		
		string GetAddColumn(string tableName, params Column[] columns);
		
		string GetDropColumn(string tableName, params string[] columns);
		
		string GetInsert(string tableName, ICollection<KeyValuePair<string, object>> values);
		
		string GetDelete(string tableName, ICollection<KeyValuePair<string, object>> where);
		
		string GetUpdate(string tableName, ICollection<KeyValuePair<string, object>> values, ICollection<KeyValuePair<string, object>> where);
		
		string GetCheckSchema();
		
		string GetCreateSchema();
	}
	
	public class MigrationRepository : BaseMigrationRepository
	{
		IMigrationFormatter f;
		
		public MigrationRepository(IMigrationFormatter f) : base(f.CreateConnection())
		{
			this.f = f;
		}
		
		public bool VersionExists(string version)
		{
			string query = string.Format("select 1 from schema_migrations where version = '{0}'", version);
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
			string query = string.Format("select version from schema_migrations order by id desc");
			using (var r = ExecuteReader(query)) {
				if (r.Read()) {
					m = r.GetString(0);
				}
			}
			return m;
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
		
		public void BuildSchema()
		{
			ExecuteNonQuery(f.GetCreateSchema());
		}
		
		public bool SchemaExists()
		{
			using (var r = ExecuteReader(f.GetCheckSchema())) {
				if (r.Read()) {
					return true;
				}
			}
			return false;
		}
		
		public void CreateTable(Table table)
		{
			ExecuteNonQuery(f.GetCreateTable(table));
		}
		
		public void DropTable(string tableName)
		{
			ExecuteNonQuery(f.GetDropTable(tableName));
		}
		
		public void AddColumns(string tableName, params Column[] columns)
		{
			ExecuteNonQuery(f.GetAddColumn(tableName, columns));
		}
		
		public void RemoveColumns(string tableName, params string[] columns)
		{
			ExecuteNonQuery(f.GetDropColumn(tableName, columns));
		}
		
		public void Insert(string tableName, ICollection<KeyValuePair<string, object>> vals)
		{
			ExecuteNonQuery(f.GetInsert(tableName, vals));
		}
		
		public void Delete(string tableName, ICollection<KeyValuePair<string, object>> where)
		{
			ExecuteNonQuery(f.GetDelete(tableName, where));
		}
		
		public void Update(string tableName, ICollection<KeyValuePair<string, object>> vals, ICollection<KeyValuePair<string, object>> where)
		{
			ExecuteNonQuery(f.GetUpdate(tableName, vals, where));
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
