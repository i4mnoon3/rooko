//	<file>
//		<license></license>
//		<owner name="Ian Escarro" email="ian.escarro@gmail.com"/>
//	</file>

using System;
using System.Configuration;
using System.Data.SqlClient;
using System.Data.SQLite;

namespace Rooko.Core
{
	public class BaseSQLiteRepository
	{
		protected SQLiteConnection connection;
		
		public BaseSQLiteRepository() : this(ConfigurationManager.ConnectionStrings[ConfigurationManager.AppSettings["database"]].ConnectionString)
		{
		}
		
		public BaseSQLiteRepository(string connectionString)
		{
			connection = new SQLiteConnection(connectionString);
		}
		
		public void ExecuteNonQuery(string query, params SQLiteParameter[] paramz)
		{
			try {
				connection.Open();
				SQLiteCommand cmd = new SQLiteCommand(query, connection);
				cmd.Parameters.AddRange(paramz);
				cmd.ExecuteNonQuery();
			} catch {
				throw;
			} finally {
				connection.Close();
			}
		}
	}
	
	public class SQLiteMigrationRepository : BaseSQLiteRepository, IMigrationRepository // SQLiteTableRepository, IMigrationRepository
	{
		SqLiteTableFormatter f = new SqLiteTableFormatter();
		
		public void CreateTable(Table table)
		{
			ExecuteNonQuery(f.GetCreateString(table));
		}
		
		public bool VersionExists(string version)
		{
			try {
				connection.Open();
				string query = string.Format("select * from schema_migrations where version = '{0}'", version);
				var cmd = new SQLiteCommand(query, connection);
				using (var reader = cmd.ExecuteReader()) {
					if (reader.Read()) {
						return true;
					}
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
				var cmd = new SQLiteCommand(query, connection);
				Migration migration = null;
				using (var reader = cmd.ExecuteReader()) {
					if (reader.Read()) {
						migration = new Migration(reader.GetString(1));
					}
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
		
		public void DropTable(string tableName)
		{
			ExecuteNonQuery(f.GetDropString(tableName));
		}
	}
	
	public class SqLiteTableFormatter : ITableFormatter
	{
		public string GetCreateString(Table table)
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
		
		public string GetDropString(string tableName)
		{
			return string.Format("drop table {0};", tableName);
		}
		
		public string GetAddColumnString(string tableName, params Column[] columns)
		{
			string cols = "";
			foreach (var c in columns) {
				cols += c.Name + " " + c.Type;
			}
			return string.Format(@"alter table {0} add ", tableName, cols);
		}
	}
}
