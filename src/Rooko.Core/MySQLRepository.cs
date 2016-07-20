//	<file>
//		<license></license>
//		<owner name="Ian Escarro" email="ian.escarro@gmail.com"/>
//	</file>

using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using MySql.Data.MySqlClient;

namespace Rooko.Core
{
	// migrate "..\src\Rooko.Tests\bin\Debug\Rooko.Tests.dll" "server=localhost;user id=root;database=test" "MySql.Data.MySqlClient"
	public class BaseMySQLRepository
	{
		protected MySqlConnection con;
		
		public BaseMySQLRepository(string connectionString)
		{
			con = new MySqlConnection(connectionString);
		}
		
		void OpenConnection()
		{
			if (con.State == ConnectionState.Closed) {
				con.Open();
			}
		}
		
		void CloseConnection()
		{
			if (con.State == ConnectionState.Open) {
				con.Close();
			}
		}
		
		public MySqlDataReader ExecuteReader(string query, params MySqlParameter[] parameters)
		{
			var cmd = new MySqlCommand(query, con);
			foreach (var p in parameters) {
				cmd.Parameters.Add(p);
			}
			OpenConnection();
			return cmd.ExecuteReader();
		}
		
		public void ExecuteNonQuery(string query, params MySqlParameter[] parameters)
		{
			try {
				var cmd = new MySqlCommand(query, con);
				foreach (var p in parameters) {
					cmd.Parameters.Add(p);
				}
				OpenConnection();
				cmd.ExecuteNonQuery();
			} catch {
				
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
		
		public void Insert(string tableName, ICollection<KeyValuePair<string, object>> values)
		{
			string cols = "", vals = "";
			int i = 0;
			foreach (var c in values) {
				cols += c.Key;
				vals += "'" + c.Value + "'";
				cols += i < values.Count - 1 ? ", " : "";
				vals += i < values.Count - 1 ? ", " : "";
				i++;
			}
			string query = string.Format("insert into {0}({1}) values({2})", tableName, cols, vals);
			ExecuteNonQuery(query);
		}
		
		public void Delete(string tableName, ICollection<KeyValuePair<string, object>> where)
		{
			string wher = "";
			int i = 0;
			foreach (var w in where) {
				wher += w.Key + " = '" + w.Value + "'";
				wher += i < where.Count - 1 ? " and" : "";
				i++;
			}
			string query = string.Format("delete from {0} where {1}", tableName, where);
			ExecuteNonQuery(query);
		}
		
		public void Update(string tableName, ICollection<KeyValuePair<string, object>> values, ICollection<KeyValuePair<string, object>> where)
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
