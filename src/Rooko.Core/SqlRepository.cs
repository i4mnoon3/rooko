//	<file>
//		<license></license>
//		<owner name="Ian Escarro" email="ian.escarro@gmail.com"/>
//	</file>

using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace Rooko.Core
{
	// Example: migrate "..\src\Rooko.Tests\bin\Debug\Rooko.Tests.dll" "Server=.;Database=test;Trusted_Connection=True;" "System.Data.SqlClient"
	public class SqlMigrationFormatter : IMigrationFormatter
	{
//		SqlConnection connection;
		string connectionString;
		string database;
		
		public SqlMigrationFormatter(string connectionString)
		{
//			this.connection = new SqlConnection(connectionString);
			this.connectionString = connectionString;
		}
		
		public IDbConnection CreateConnection()
		{
			var connection = new SqlConnection(connectionString);
			this.database = connection.Database;
			return connection;
		}
		
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
		
		public string GetAddColumn(string tableName, params string[] columns)
		{
			var cols = new List<Column>();
			foreach (var c in columns) {
				cols.Add(new Column(c));
			}
			return GetAddColumn(tableName, cols.ToArray());
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
			string @where = "";
			int i = 0;
			if (columns.Length > 0) {
				@where = " where ";
				foreach (var c in columns) {
					@where += c.Name + " = '" + c.Value + "'";
					@where += i++ < columns.Length - 1 ? " and " : "";
				}
			}
			return string.Format("delete from {0} {1}", tableName, @where);
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
				vals += i++ < values.Count ? " and " : ""; 
			}
			foreach (var w in @where) {
				wher += w.Key + " = '" + w.Value + "'";
				wher += i++ < @where.Count ? " and " : "";
			}
			return string.Format("update {0} set {1} where {2}", tableName, vals, wher);
		}
		
		public string GetCreateSchema()
		{
			return string.Format("create table schema_migrations(id integer not null primary key identity, version varchar(255))");
		}
		
		public string GetCheckSchema()
		{
			return string.Format("select 1 from {0}.information_schema.tables where table_name = 'schema_migrations'", database);
		}
	}
}
