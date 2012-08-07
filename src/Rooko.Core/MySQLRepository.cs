//	<file>
//		<license></license>
//		<owner name="Ian Escarro" email="ian.escarro@gmail.com"/>
//	</file>

using System;
using System.Configuration;
using MySql.Data.MySqlClient;

namespace Rooko.Core
{
	public class BaseMySQLRepository
	{
		protected MySqlConnection connection = new MySqlConnection(ConfigurationManager.ConnectionStrings["test"].ConnectionString);
		
		public void ExecuteNonQuery(string query, params MySqlParameter[] paramz)
		{
			try {
				connection.Open();
				MySqlCommand cmd = new MySqlCommand(query, connection);
				cmd.Parameters.AddRange(paramz);
				cmd.ExecuteNonQuery();
			} catch {
				throw;
			} finally {
				connection.Close();
			}
		}
	}
	
	public class MySQLMigrationRepository : MySQLTableRepository, IMigrationRepository
	{
		public bool ReadByVersion(string version)
		{
			try {
				connection.Open();
				string query = string.Format("select * from schema_migrations where version = '{0}'", version);
				MySqlCommand cmd = new MySqlCommand(query, connection);
				MySqlDataReader reader = cmd.ExecuteReader();
				if (reader.Read()) {
					return true;
				}
				return false;
			} catch {
				throw;
			} finally {
				connection.Close();
			}
		}
		
		public Migration ReadLatest()
		{
			try {
				connection.Open();
				string query = string.Format("select * from schema_migrations order by id desc");
				MySqlCommand cmd = new MySqlCommand(query, connection);
				MySqlDataReader reader = cmd.ExecuteReader();
				Migration migration = null;
				if (reader.Read()) {
					migration = new Migration(reader.GetString(1));
				}
				return migration;
			} catch {
				throw;
			} finally {
				connection.Close();
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
	}
	
	public class MySQLTableRepository : BaseMySQLRepository, ITableRepository
	{
		public void Create(Table table)
		{
			ExecuteNonQuery(new MySQLTableFormatter().ToCreateString(table));
		}
		
		public void Drop(string tableName)
		{
			ExecuteNonQuery(new MySQLTableFormatter().ToDropString(tableName));
		}
		
		public bool Exists(string tableName)
		{
			try {
				connection.Open();
				string query = string.Format("select * from information_schema.tables where table_schema = '{0}' and table_name = '{1}'", connection.Database, tableName);
				MySqlCommand cmd = new MySqlCommand(query, connection);
				MySqlDataReader reader = cmd.ExecuteReader();
				if (reader.Read()) {
					return true;
				}
				return false;
			} catch {
				throw;
			} finally {
				connection.Close();
			}
		}
		
		public Table Read(string tableName)
		{
			try {
				connection.Open();
				string query = string.Format("select * from information_schema.tables where table_schema = '{0}' and table_name = '{1}'", connection.Database, tableName);
				MySqlCommand cmd = new MySqlCommand(query, connection);
				MySqlDataReader reader = cmd.ExecuteReader();
				Table table = null;
				if (reader.Read()) {
					table = new Table(reader["table_name"].ToString());
				}
				return table;
			} catch {
				throw;
			} finally {
				connection.Close();
			}
		}
	}
	
	public class MySQLTableFormatter : ITableFormatter
	{
		public string ToCreateString(Table table)
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
		
		public string ToDropString(string tableName)
		{
			return string.Format("drop table {0};", tableName);
		}
		
//		public string ToInsertString(Table table)
//		{
//			string cols = "";
//			string vals = "";
//			int i = 0;
//			foreach (var c in table.Columns) {
//				if (!c.AutoIncrement) {
//					cols += c.Name;
//					cols += i < table.Columns.Count - 1 ? ", " : "";
//					vals += c.Value.GetType().IsAssignableFrom(typeof(string)) ? "'" + c.Value + "'" : c.Value;
//				}
//				i++;
//			}
//			return string.Format("insert into {0}({1}) values({2})", table.Name, cols, vals);
//		}
//		
//		public string ToDeleteString(Table table)
//		{
//			string colsVals = "";
//			if (table.Columns.Count > 0) {
//				colsVals += " where";
//			}
//			foreach (var c in table.Columns) {
//				if (c.PrimaryKey) {
//					var val = c.Value.GetType().IsAssignableFrom(typeof(string)) ? "'" + c.Value + "'" : c.Value;
//					colsVals += " " + c.Name + " = " + val;
//				}
//			}
//			return string.Format("delete from {0}{1}", table.Name, colsVals);
//		}
	}
}
