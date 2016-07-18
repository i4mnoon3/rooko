//	<file>
//		<license></license>
//		<owner name="Ian Escarro" email="ian.escarro@gmail.com"/>
//	</file>

using System;
using System.Data;
using System.Data.SqlClient;

namespace Rooko.Core
{
	public class BaseSqlRepository
	{
		SqlConnection connection;
		
		public BaseSqlRepository(string connectionString)
		{
			connection = new SqlConnection(connectionString);
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
		
		protected void ExecuteNonQuery(string query, params SqlParameter[] parameters)
		{
			try {
				OpenConnection();
				var cmd = new SqlCommand(query, connection);
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
		
		protected SqlDataReader ExecuteReader(string query, params SqlParameter[] parameters)
		{
			try {
				OpenConnection();
				var cmd = new SqlCommand(query, connection);
				foreach (var p in parameters) {
					cmd.Parameters.Add(p);
				}
				return cmd.ExecuteReader();
			} catch {
				throw;
			}
		}
	}
	
	public class SqlMigrationRepository : BaseSqlRepository, IMigrationRepository
	{
		SqlMigrationFormatter f = new SqlMigrationFormatter();
		
		public SqlMigrationRepository(string connectionString) : base(connectionString)
		{
		}
		
		public bool VersionExists(string version)
		{
			string query = string.Format("select version from schema_migrations where version = '{0}'", version);
			using (var r = ExecuteReader(query)) {
				if (r.Read()) {
					return true;
				}
			}
			return false;
		}
		
		public Migration ReadLatest()
		{
			throw new NotImplementedException();
		}
		
		public bool SchemaExists()
		{
			string query = "select 1 from information_schema.tables where table_name = 'schema_migrations'";
			using (var r = ExecuteReader(query)) {
				if (r.Read()) {
					return true;
				}
			}
			return false;
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
		
		public void Insert(string tableName, params Column[] columns)
		{
			string cols = "", vals = "";
			int i = 0;
			foreach (var c in columns) {
				cols += c.Name;
				vals += "'" + c.Value + "'";
				cols += i < columns.Length - 1 ? ", " : "";
				vals += i < columns.Length - 1 ? ", " : "";
				i++;
			}
			string query = string.Format("insert into {0}({1}) values({2})", tableName, cols, vals);
			ExecuteNonQuery(query);
		}
		
		public void Delete(string tableName, params Column[] columns)
		{
			string where = "";
			int i = 0;
			foreach (var c in columns) {
				where += c.Name + " = '" + c.Value + "'";
				where += i < columns.Length - 1 ? " and" : "";
				i++;
			}
			string query = string.Format("delete from {0} where {1}", tableName, where);
			ExecuteNonQuery(query);
		}
	}
	
	public class SqlMigrationFormatter : IMigrationFormatter
	{
		public string GetCreateTable(Table table)
		{
			string cols = "";
			int i = 0;
			foreach (var c in table.Columns) {
				cols += c.Name + " " + c.Type;
				cols += c.PrimaryKey ? " primary key" : "";
				cols += c.NotNull ? " not null" : "";
				cols += c.AutoIncrement ? " identity" : "";
				cols += i++ < table.Columns.Count - 1 ? ", " : "";
			}
			return string.Format("create table {0}({1})", table.Name, cols);
		}
		
		public string GetDropTable(string tableName)
		{
			return string.Format("drop table {0}", tableName);
		}
		
		public string GetAddColumn(string tableName, params Column[] columns)
		{
			string cols = "";
			int i = 0;
			foreach (var c in columns) {
				cols += c.Name + " " + c.Type;
				cols += i++ < columns.Length - 1 ? ", " : "";
			}
			return string.Format("alter table {0} add {1}", tableName, cols);
		}
		
		public string GetDropColumn(string tableName, params string[] columns)
		{
			string cols = "";
			int i = 0;
			foreach (var c in columns) {
				cols += c;
				cols += i++ < columns.Length - 1 ? ", " : "";
			}
			return string.Format("alter table {0} drop column {1}", tableName, cols);
		}
		
		public string GetInsert(string tableName, params Column[] columns)
		{
			string cols = "", vals = "";
			int i = 0;
			foreach (var c in columns) {
				cols += c.Name;
				vals += "'" + c.Value + "'";
				cols += i < columns.Length - 1 ? ", " : "";
				vals += i < columns.Length - 1 ? ", " : "";
				i++;
			}
			return string.Format("insert into {0}({1}) values({2})", tableName, cols, vals);
		}
		
		public string GetDelete(string tableName, params Column[] columns)
		{
			string where = "";
			int i = 0;
			foreach (var c in columns) {
				where += c.Name + " = '" + c.Value + "'";
				where += i++ < columns.Length - 1 ? " and " : "";
			}
			return string.Format("delete from {0} where {1}", tableName, where);
		}
	}
}
