//	<file>
//		<license></license>
//		<owner name="Ian Escarro" email="ian.escarro@gmail.com"/>
//	</file>

using System;
using System.Configuration;
using System.Data.SQLite;

namespace Rooko.Core
{
	public class BaseSQLiteRepository
	{
		protected SQLiteConnection connection = new SQLiteConnection(ConfigurationManager.ConnectionStrings["test"].ConnectionString);
		
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
	
	public class SQLiteTableRepository : BaseSQLiteRepository, ITableRepository
	{
		public void Create(Table table)
		{
			ExecuteNonQuery(new SQLiteTableFormatter().ToCreateString(table));
		}
		
		public void Drop(string tableName)
		{
			ExecuteNonQuery(new SQLiteTableFormatter().ToDropString(tableName));
		}
		
		public bool Exists(string tableName)
		{
			try {
				connection.Open();
				string query = string.Format("select * from sqlite_master where type = 'table' and tbl_name = '{0}'", tableName);
				SQLiteCommand cmd = new SQLiteCommand(query, connection);
				SQLiteDataReader reader = cmd.ExecuteReader();
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
		
//		public Table Read(string tableName)
//		{
//			try {
//				connection.Open();
//				string query = string.Format("select * from sqlite_master where type = 'table' and tbl_name = '{0}'", tableName);
//				SQLiteCommand cmd = new SQLiteCommand(query, connection);
//				SQLiteDataReader reader = cmd.ExecuteReader();
//				Table table = null;
//				if (reader.Read()) {
//					table = new Table(reader["tbl_name"].ToString());
//				}
//				return table;
//			} catch {
//				throw;
//			} finally {
//				connection.Close();
//			}
//		}
		
//		public void Save(Table table)
//		{
//			ExecuteNonQuery(new SQLiteTableFormatter().ToInsertString(table));
//		}
//
//		public void Delete(Table table)
//		{
//			ExecuteNonQuery(new SQLiteTableFormatter().ToDeleteString(table));
//		}
	}
	
	public class SQLiteTableFormatter : ITableFormatter
	{
		public string ToCreateString(Table table)
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
		
//		public string ToDeleteString(Table table)
//		{
//			string colsVals = "";
//			if (table.Columns.Count > 0) {
//				colsVals += " where";
//			}
//			foreach (var c in table.Columns) {
//				var val = c.Value.GetType().IsAssignableFrom(typeof(string)) ? "'" + c.Value + "'" : c.Value;
//				colsVals += c.Name + " = " + val;
//			}
//			return string.Format("delete from {0}{1}", table.Name, colsVals);
//		}
	}
}
