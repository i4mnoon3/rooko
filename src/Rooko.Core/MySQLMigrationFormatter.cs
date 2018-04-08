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
	// Example: migrate "..\src\Rooko.Tests\bin\Debug\Rooko.Tests.dll" "server=localhost;user id=root;database=test" "MySql.Data.MySqlClient"
	public class MySQLMigrationFormatter : IMigrationFormatter
	{
		string connectionString;
		string database;
		
		public MySQLMigrationFormatter() : this("")
		{
		}
		
		public MySQLMigrationFormatter(string connectionString)
		{
			this.connectionString = connectionString;
		}
		
		public IDbConnection CreateConnection()
		{
			var connection = new MySqlConnection(connectionString);
			database = connection.Database;
			return connection;
		}
		
		public string CreateSchema()
		{
			return string.Format("create table schema_migrations(id integer not null primary key auto_increment, version varchar(255))");
		}
		
		public string CheckSchema()
		{
			return string.Format(@"SELECT 1 FROM INFORMATION_SCHEMA.TABLES
WHERE table_name = 'schema_migrations' AND table_schema = '{0}'", database);
		}
		
		public string CreateTable(Table table)
		{
			string cols = "";
			int i = 1;
			foreach (var c in table.Columns) {
				string notNull = c.NotNull ? " not null" : "";
				string primaryKey = c.IsPrimaryKey ? " primary key" : "";
				string autoIncrement = c.AutoIncrement ? " auto_increment" : "";
				cols += string.Format("	{0} {1}{2}{3}{4}", c.Name, c.Type, notNull, primaryKey, autoIncrement);
				cols += i < table.Columns.Count ? "," : "";
				cols += i < table.Columns.Count ? Environment.NewLine : "";
				i++;
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
				cols += string.Format("ALTER TABLE {0} ADD {1} {2};", tableName, c.Name, c.Type);
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
			int i = 1;
			foreach (var c in columns) {
				cols += "DROP " + c;
				cols += i++ < columns.Length ? ", " : "";
			}
			return string.Format(@"ALTER TABLE {0}
{1};", tableName, cols);
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
			return string.Format(@"INSERT INTO {0}({1})
VALUES({2})", tableName, cols, vals);
		}
		
		public string Delete(string tableName, ICollection<KeyValuePair<string, object>> @where)
		{
			string wher = "";
			int i = 1;
			foreach (var w in @where) {
				wher += w.Key + " = '" + w.Value + "'";
				
				wher += i++ < @where.Count ? " AND " : "";
			}
			return string.Format("DELETE FROM {0} WHERE {1}", tableName, wher);
		}
		
		public string Update(string tableName, ICollection<KeyValuePair<string, object>> values, ICollection<KeyValuePair<string, object>> @where)
		{
			string vals = "", wher = "";
			int i = 1;
			foreach (var v in values) {
				vals += v.Key + " = '" + v.Value + "'";
				vals += i++ < values.Count ? ", " : "";
			}
			i = 1;
			foreach (var w in @where) {
				wher += w.Key + " = '" + w.Value + "'";
				wher += i++ < @where.Count ? ", " : "";
			}
			return string.Format("UPDATE {0} SET {1} WHERE {2}", tableName, vals, wher);
		}
	}
}
