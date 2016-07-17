/*
 * Created by SharpDevelop.
 * User: Dev
 * Date: 7/17/2016
 * Time: 8:31 PM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;

namespace Rooko.Core
{
	public class SqlMigrationRepository // : IMigrationRepository
	{
		public SqlMigrationRepository()
		{
		}
	}
	
	public class SqlTableFormatter : ITableFormatter
	{
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
			string where = "";
			int i = 0;
			foreach (var c in columns) {
				where += c.Name + " = '" + c.Value + "'";
				where += i++ < columns.Length - 1 ? " and " : "";
			}
			return string.Format("delete from {0} where {1}", tableName, where);
		}
	}
}
