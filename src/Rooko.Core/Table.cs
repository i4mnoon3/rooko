//	<file>
//		<license></license>
//		<owner name="Ian Escarro" email="ian.escarro@gmail.com"/>
//	</file>

using System;
using System.Collections.Generic;

namespace Rooko.Core
{
	public interface IMigrationRepository : ITableRepository
	{
		bool ReadByVersion(string version);
		
		Migration ReadLatest();
		
		void Save(Migration migration);
		
		void Delete(Migration migration);
	}
	
	public interface ITableRepository
	{
		void Create(Table table);
		
		void Drop(string tableName);
		
		bool Exists(string tableName);
	}
	
	public interface ITableFormatter
	{
		string GetCreateString(Table table);
		
		string GetDropString(string tableName);
	}
	
//	public class Schema : Table
//	{
//		public Schema(string version) 
//			: base("schema_migrations", new Column("id", "integer", true, true, true), new Column("version") { Value = version })
//		{
//			this.Version = version;
//		}
//		
//		public int Id { get; set; }
//		
//		public string Version { get; set; }
//	}
	
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
