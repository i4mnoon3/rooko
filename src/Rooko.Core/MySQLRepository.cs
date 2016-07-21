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
		
		public string GetCreateSchema()
		{
			return string.Format("create table schema_migrations(id integer not null primary key auto_increment, version varchar(255))");
		}
		
		public string GetCheckSchema()
		{
			return string.Format("select 1 from information_schema.tables where table_name = 'schema_migrations' and table_schema = '{0}'", database);
		}
		
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
			return string.Format(@"create table {0}({1})", table.Name, cols);
		}
		
		public string GetDropTable(string tableName)
		{
			return string.Format("drop table {0}", tableName);
		}
		
		public string GetAddColumn(string tableName, params Column[] columns)
		{
			string cols = "";
			foreach (var c in columns) {
				cols += c.Name + " " + c.Type + " ";
			}
			return string.Format("alter table {0} add {1}", tableName, cols);
		}
		
		public string GetDropColumn(string tableName, params string[] columns)
		{
			string cols = "";
			foreach (var c in columns) {
				cols += c + " ";
			}
			return string.Format("alter table {0} drop column {1}", tableName, cols);
		}
		
		public string GetInsert(string tableName, ICollection<KeyValuePair<string, object>> values)
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
		
		public string GetDelete(string tableName, ICollection<KeyValuePair<string, object>> @where)
		{
			string wher = "";
			int i = 1;
			foreach (var w in @where) {
				wher += w.Key + " = '" + w.Value + "'";
				
				wher += i++ < @where.Count ? " and " : "";
			}
			return string.Format("delete from {0} where {1}", tableName, wher);
		}
		
		public string GetUpdate(string tableName, ICollection<KeyValuePair<string, object>> values, ICollection<KeyValuePair<string, object>> @where)
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
			return string.Format("update {0} set {1} where {2}", tableName, vals, wher);
		}
	}
}
