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
	// Example: migrate "..\src\Rooko.Tests\bin\Debug\Rooko.Tests.dll" "data source=db.sqlite" "System.Data.SQLite"
	public class SQLiteMigrationFormatter : IMigrationFormatter
	{
		string database;
		string connectionString;
		
		public SQLiteMigrationFormatter(string connectionString)
		{
			this.connectionString = connectionString;
		}
		
		public IDbConnection CreateConnection()
		{
			var connection = new SQLiteConnection(connectionString);
			database = connection.Database;
			return connection;
		}
		
		public string CreateTable(Table table)
		{
			string cols = "";
			int i = 1;
			foreach (var c in table.Columns) {
				string notNull = c.NotNull ? " NOT NULL" : "";
				string primaryKey = c.IsPrimaryKey ? " PRIMARY KEY" : "";
				cols += string.Format("  {0} {1}{2}{3}", c.Name, c.Type, notNull, primaryKey);
				cols += i++ < table.Columns.Count ? "," : "";
				cols += Environment.NewLine;
			}
			return string.Format(@"CREATE TABLE {0}(
{1}
);", table.Name, cols);
		}
		
		public string DropTable(string tableName)
		{
			return string.Format("DROP TABLE {0};", tableName);
		}
		
		public string AddColumn(string tableName, params Column[] columns)
		{
			string cols = "";
			int i = 1;
			foreach (var c in columns) {
				cols += string.Format("alter table {0} add {1} {2}", tableName, c.Name, c.Type);
				if (i++ < columns.Length) {
					cols += Environment.NewLine;
				}
			}
			return cols;
		}
		
		public string AddColumn(string tableName, params string[] columns)
		{
			var cols = new List<Column>();
			foreach (var c in columns) {
				cols.Add(new Column(c));
			}
			return AddColumn(tableName, cols.ToArray());
		}
		
		public string DropColumn(string tableName, params string[] columns)
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
		
		public string Insert(string tableName, ICollection<KeyValuePair<string, object>> values)
		{
			string cols = "", vals = "";
			int i = 1;
			foreach (var v in values) {
				cols += v.Key;
				vals += "'" + v.Value + "'";
				cols += i < values.Count ? ", " : "";
				vals += i < values.Count ? ", " : "";
				i++;
			}
			return string.Format("insert into {0}({1}) values({2})", tableName, cols, vals);
		}
		
		public string Delete(string tableName, ICollection<KeyValuePair<string, object>> @where)
		{
			string wher = "";
			int i = 1;
			foreach (var w in @where) {
				wher += w.Key + " = '" + w.Value + "'";
				wher += i++ < @where.Count ? " and " : "";
			}
			return string.Format("delete from {0} where {1}", tableName, wher);
		}
		
		public string Update(string tableName, ICollection<KeyValuePair<string, object>> values, ICollection<KeyValuePair<string, object>> @where)
		{
			string vals = "", wher = "";
			int i = 1;
			foreach (var v in values) {
				vals += v.Key + " = '" + v.Value + "'";
				vals += i++ < values.Count ? " and " : ""; 
			}
			foreach (var w in @where) {
				wher += w.Key + " = '" + w.Value + "'";
				wher += i++ < @where.Count ? " and " : "";
			}
			return string.Format("update {0} set {1} where {2}", tableName, vals, wher);
		}
		
		public string CreateSchema()
		{
			return string.Format("create table schema_migrations(id integer not null primary key, version varchar(255))");
		}
		
		public string CheckSchema()
		{
			return string.Format("select 1 from sqlite_master where tbl_name = 'schema_migrations'");
		}
	}
}
