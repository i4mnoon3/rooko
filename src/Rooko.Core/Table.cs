//	<file>
//		<license></license>
//		<owner name="Ian Escarro" email="ian.escarro@gmail.com"/>
//	</file>

using System;
using System.Collections.Generic;
using System.Data;

namespace Rooko.Core
{
	public class TableEventArgs : EventArgs
	{
		public TableEventArgs(string tableName) : this(new Table(tableName))
		{
		}
		
		public TableEventArgs(Table table)
		{
			this.Table = table;
		}
		
		public TableEventArgs(Table table, ICollection<KeyValuePair<string, object>> values)
		{
			this.Table = table;
			this.Values = values;
		}
		
		public TableEventArgs(Table table, ICollection<KeyValuePair<string, object>> values, ICollection<KeyValuePair<string, object>> @where)
		{
			this.Table = table;
			this.Values = values;
			this.Where = @where;
		}
		
		public Table Table { get; set; }
		
		public ICollection<KeyValuePair<string, object>> Values { get; set; }
		
		public ICollection<KeyValuePair<string, object>> Where { get; set; }
	}
	
	public class Table
	{
		public Table(string name, params Column[] columns)
		{
			this.Name = name;
			this.Columns = new List<Column>(columns);
		}
		
		public string Name { get; set; }

		public List<Column> Columns { get; set; }
		
		public List<string> ColumnNames {
			get {
				var columns = new List<string>();
				foreach (var c in Columns) {
					columns.Add(c.Name);
				}
				return columns;
			}
		}
		
		public void AddColumn(string name)
		{
			AddColumn(name, "varchar(255)");
		}
		
		public void AddColumn(string name, string type)
		{
			AddColumn(name, type, false, false, false);
		}
		
		public void AddColumn(string name, string type, bool primaryKey, bool notNull, bool autoIncrement)
		{
			AddColumn(new Column(name, type, primaryKey, notNull, autoIncrement));
		}
		
		public void AddColumn(Column column)
		{
			column.Table = this;
			Columns.Add(column);
		}
	}
	
	public class Column
	{
		public Column() : this("")
		{
		}
		
		public Column(string name) : this(name, "varchar(255)")
		{
		}
		
		public Column(string name, string type) : this(name, type, false)
		{
		}
		
		public Column(string name, string type, bool primaryKey) : this(name, type, primaryKey, false, false)
		{
		}
		
		public Column(string name, string type, bool primaryKey, bool notNull, bool autoIncrement)
		{
			this.Name = name;
			this.Type = type;
			this.IsPrimaryKey = primaryKey;
			this.NotNull = notNull;
			this.AutoIncrement = autoIncrement;
		}
		
		public Table Table { get; set; }
		
		public string Name { get; set; }

		public string Type { get; set; }
		
		public bool NotNull { get; set; }

		public bool IsPrimaryKey { get; set; }

		public bool AutoIncrement { get; set; }
		
		public object Value { get; set; }
		
		public override string ToString()
		{
			return Name;
		}
	}
}
