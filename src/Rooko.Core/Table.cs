//	<file>
//		<license></license>
//		<owner name="Ian Escarro" email="ian.escarro@gmail.com"/>
//	</file>

using System;
using System.Collections.Generic;

namespace Rooko.Core
{
	public interface IMigrationRepository// : ITableRepository
	{
		bool VersionExists(string version);
		
		Migration ReadLatest();
		
		void Save(Migration migration);
		
		void Delete(Migration migration);
		
		void CreateTable(Table table);
		
		void DropTable(string tableName);
		
		bool SchemaExists();
		
		void AddColumns(string tableName, params Column[] columns);
	}
	
//	public interface ITableRepository
//	{
//		void Create(Table table);
//		
//		void Drop(string tableName);
//		
//		bool Exists(string tableName);
//	}
	
	public interface ITableFormatter
	{
		string GetCreateString(Table table);
		
		string GetDropString(string tableName);
		
		string GetAddColumnString(string tableName, params Column[] columns);
	}
	
	public class Table
	{
		public Table(string name, params Column[] columns)
		{
			this.Name = name;
			this.Columns = new List<Column>(columns);
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

		public string Name { get; set; }

		public List<Column> Columns { get; set; }
	}
	
	public class Column
	{
		public Table Table { get; set; }
		
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
			this.PrimaryKey = primaryKey;
			this.NotNull = notNull;
			this.AutoIncrement = autoIncrement;
		}
		
		public string Name { get; set; }

		public string Type { get; set; }
		
		public bool NotNull { get; set; }

		public bool PrimaryKey { get; set; }

		public bool AutoIncrement { get; set; }
		
		public object Value { get; set; }
	}
}
