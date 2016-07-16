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
	
	public class MySQLMigrationRepository : BaseMySQLRepository, IMigrationRepository // MySQLTableRepository, IMigrationRepository
	{
		MySQLTableFormatter f = new MySQLTableFormatter();
		
		public void CreateTable(Table table)
		{
			ExecuteNonQuery(f.GetCreateString(table));
		}
		
		public void DropTable(string tableName)
		{
			ExecuteNonQuery(f.GetDropString(tableName));
		}
		
		public bool VersionExists(string version)
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
		
		public bool SchemaExists()
		{
			throw new NotImplementedException();
		}
		
		public void AddColumns(string tableName, params Column[] columns)
		{
			throw new NotImplementedException();
		}
	}
	
	public class MySQLTableFormatter : ITableFormatter
	{
		public string GetCreateString(Table table)
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
		
		public string GetDropString(string tableName)
		{
			return string.Format("drop table {0};", tableName);
		}
		
		public string GetAddColumnString(string tableName, params Column[] columns)
		{
			throw new NotImplementedException();
		}
	}
}
