//	<file>
//		<license></license>
//		<owner name="Ian Escarro" email="ian.escarro@gmail.com"/>
//	</file>

using System;
using System.Configuration;
using System.Data;
using MySql.Data.MySqlClient;

namespace Rooko.Core
{
	public class BaseMySQLRepository
	{
//		protected MySqlConnection connection = new MySqlConnection(ConfigurationManager.ConnectionStrings["test"].ConnectionString);
		protected MySqlConnection connection;
		
		public BaseMySQLRepository(string connectionString)
		{
			connection = new MySqlConnection(connectionString);
		}
		
		public void ExecuteNonQuery(string query, params MySqlParameter[] paramz)
		{
			try {
				OpenConnection();
				MySqlCommand cmd = new MySqlCommand(query, connection);
				cmd.Parameters.AddRange(paramz);
				cmd.ExecuteNonQuery();
			} catch {
				throw;
			} finally {
				CloseConnection();
			}
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
		
		public MySqlDataReader ExecuteReader(string query, params MySqlParameter[] paramz)
		{
			try {
				OpenConnection();
				var cmd = new MySqlCommand(query, connection);
				cmd.Parameters.AddRange(paramz);
				return cmd.ExecuteReader();
			} catch {
				throw;
			} finally {
				CloseConnection();
			}
		}
	}
	
	public class MySQLMigrationRepository : BaseMySQLRepository, IMigrationRepository
	{
		MySQLMigrationFormatter f = new MySQLMigrationFormatter();
		
		public MySQLMigrationRepository(string connectionString) : base(connectionString)
		{
		}
		
		public bool VersionExists(string version)
		{
			string query = string.Format("select * from schema_migrations where version = '{0}'", version);
			using (var r = ExecuteReader(query)) {
				if (r.Read()) {
					return true;
				}
			}
			return false;
		}
		
		public Migration ReadLatest()
		{
			string query = string.Format("select * from schema_migrations order by id desc");
			Migration m = null;
			using (var r = ExecuteReader(query)) {
				if (r.Read()) {
					m = new Migration(r.GetString(1));
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
		
		public bool SchemaExists()
		{
			throw new NotImplementedException();
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
			string cols = "";
			foreach (var c in columns) {
				cols += c + " ";
			}
			string query = string.Format("alter table {0} drop column {1}", tableName, cols);
			ExecuteNonQuery(query);
		}
		
		public void Insert(string tableName, params Column[] columns)
		{
			throw new NotImplementedException();
		}
		
		public void Delete(string tableName, params Column[] columns)
		{
			throw new NotImplementedException();
		}
	}
	
	public class MySQLMigrationFormatter : IMigrationFormatter
	{
		public string GetCreateTable(Table table)
		{
			string cols = "";
			int i = 0;
			foreach (var c in table.Columns) {
				string notNull = c.NotNull ? " not null" : "";
				string primaryKey = c.PrimaryKey ? " primary key" : "";
				string autoIncrement = c.AutoIncrement ? " auto_increment" : "";
				cols += string.Format("  {0} {1}{2}{3}{4}", c.Name, c.Type, notNull, primaryKey, autoIncrement);
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
			foreach (var c in columns) {
				cols += c.Name + " " + c.Type + " ";
			}
			return string.Format("alter table {0} add ", tableName, cols);
		}
		
		public string GetDropColumn(string tableName, params string[] columns)
		{
			string cols = "";
			foreach (var c in columns) {
				cols += c + " ";
			}
			return string.Format("alter table {0} drop column ", tableName, cols);
		}
	}
}
