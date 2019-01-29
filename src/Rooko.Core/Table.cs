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
            AddColumn(name, DbType.String);
        }
        
        public void AddColumn(string name, DbType type)
        {
            AddColumn(name, type, 0, false, false, false);
        }
        
        public void AddColumn(string name, DbType type, int size)
        {
            AddColumn(name, type, size, false, false, false);
        }
        
        public void AddColumn(string name, DbType type, bool primaryKey, bool notNull, bool autoIncrement)
        {
            AddColumn(name, type, 0, primaryKey, notNull, autoIncrement);
        }
        
        public void AddColumn(string name, DbType type, int size, bool primaryKey, bool notNull, bool autoIncrement)
        {
            AddColumn(new Column(name, type, size, primaryKey, notNull, autoIncrement));
        }
        
        public void AddColumn(Column column)
        {
            column.Table = this;
            Columns.Add(column);
        }
    }
    
    public class Column
    {
        public Column(string name) : this(name, DbType.String, 255)
        {
        }
        
        public Column(string name, DbType type, int size) : this(name, type, size, false)
        {
        }
        
        public Column(string name, DbType type, int size, bool primaryKey) : this(name, type, size, primaryKey, false, false)
        {
        }
        
        public Column(string name, DbType type, int size, bool primaryKey, bool notNull, bool autoIncrement)
        {
            this.Name = name;
            this.Type = type;
            this.Size = size;
            this.IsPrimaryKey = primaryKey;
            this.NotNull = notNull;
            this.AutoIncrement = autoIncrement;
        }
        
        public Table Table { get; set; }
        
        public int Size { get; set; }
        
        public string Name { get; set; }

        public DbType Type { get; set; }
        
        public bool NotNull { get; set; }

        public bool IsPrimaryKey { get; set; }

        public bool AutoIncrement { get; set; }
        
        public object Value { get; set; }
        
        public bool HasSize {
            get { return Size > 0; }
        }
        
        public string GetDbType()
        {
            if (Type == DbType.String && HasSize) {
                return "VARCHAR(" + Size + ")";
            } else if (Type == DbType.Int32) {
                return "INT";
            } else {
                return "VARCHAR(255)";
            }
        }
        
        public override string ToString()
        {
            return Name;
        }
    }
}
