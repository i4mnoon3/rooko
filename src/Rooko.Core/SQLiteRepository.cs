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
		SQLiteConnection connection;
		
		public SQLiteMigrationFormatter(string connectionString)
		{
			this.connection = new SQLiteConnection(connectionString);
		}
		
		public IDbConnection CreateConnection()
		{
			return connection;
		}
		
		public string GetCreateTable(Table table)
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
			return string.Format(@"create table {0}({1});", table.Name, cols);
		}
		
		public string GetDropTable(string tableName)
		{
			return string.Format("drop table {0};", tableName);
		}
		
		public string GetAddColumn(string tableName, params Column[] columns)
		{
			string cols = "";
			int i = 0;
			foreach (var c in columns) {
				cols += string.Format("alter table {0} add {1} {2}", tableName, c.Name, c.Type);
				if (i++ < columns.Length - 1) {
					cols += ";" + Environment.NewLine;
				}
			}
			return cols;
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
				cols += string.Format("alter table {0} remove column {1}", tableName, c);
				if (i++ < columns.Length - 1) {
					cols += ";" + Environment.NewLine;
				}
			}
			return cols;
		}
		
		public string GetInsert(string tableName, ICollection<KeyValuePair<string, object>> vals)
		{
			throw new NotImplementedException();
		}
		
		public string GetDelete(string tableName, ICollection<KeyValuePair<string, object>> @where)
		{
			throw new NotImplementedException();
		}
		
		public string GetUpdate(string tableName, ICollection<KeyValuePair<string, object>> vals, ICollection<KeyValuePair<string, object>> @where)
		{
			throw new NotImplementedException();
		}
		
		public string GetCreateSchema()
		{
			return string.Format("create table schema_migrations(id integer not null primary key, version varchar(255))");
		}
		
		public string GetCheckSchema()
		{
			throw new NotImplementedException();
		}
	}
}
