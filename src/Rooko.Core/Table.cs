//	<file>
//		<license></license>
//		<owner name="Ian Escarro" email="ian.escarro@gmail.com"/>
//	</file>

using System;
using System.Collections.Generic;
using System.Data;

namespace Rooko.Core
{
	public interface IMigrationRepository
	{
		bool VersionExists(string version);
		
		Migration ReadLatest();
		
		bool SchemaExists();
		
		void Save(Migration migration);
		
		void Delete(Migration migration);
		
		void CreateTable(Table table);
		
		void DropTable(string tableName);
		
		void AddColumns(string tableName, params Column[] columns);
		
		void RemoveColumns(string tableName, params string[] columns);
		
		void Insert(string tableName, params Column[] columns);
		
		void Delete(string tableName, params Column[] columns);
		
//		void Update(string tableName);
	}
	
	public class BaseMigrationRepository
	{
		IDbConnection connection;
		
		public void OpenConnection()
		{
			if (connection.State == ConnectionState.Closed) {
				connection.Open();
			}
		}
		
		public void CloseConnection()
		{
			if (connection.State == ConnectionState.Open) {
				connection.Close();
			}
		}
		
		public void ExecuteNonQuery(string query, params IDbDataParameter[] parameters)
		{
			try {
				OpenConnection();
				IDbCommand cmd = connection.CreateCommand();
				cmd.CommandText = query;
				foreach (var p in parameters) {
					cmd.Parameters.Add(p);
				}
				cmd.ExecuteNonQuery();
			} catch {
				throw;
			} finally {
				CloseConnection();
			}
		}
		
		public IDataReader ExecuteReader(string query, params IDbDataParameter[] parameters)
		{
			try {
				OpenConnection();
				IDbCommand cmd = connection.CreateCommand();
				cmd.CommandText = query;
				foreach (var p in parameters) {
					cmd.Parameters.Add(p);
				}
				return cmd.ExecuteReader();
			} catch {
				throw;
			}
		}
	}
	
	public class MigrationRepostory : BaseMigrationRepository, IMigrationRepository
	{
		IMigrationFormatter f;
		
		public MigrationRepostory(IMigrationFormatter f)
		{
			this.f = f;
		}
		
		public bool VersionExists(string version)
		{
			throw new NotImplementedException();
		}
		
		public Migration ReadLatest()
		{
			Migration m = null;
			string query = string.Format("select version from schema_migrations order by id desc");
			using (var r = ExecuteReader(query)) {
				if (r.Read()) {
					m = new Migration(r.GetString(0));
				}
			}
			return m;
		}
		
		public bool SchemaExists()
		{
			throw new NotImplementedException();
		}
		
		public void Save(Migration migration)
		{
			string query = string.Format("insert into schema_migrations(version) values('{0}')", migration.Version);
			ExecuteNonQuery(query);
		}
		
		public void Delete(Migration migration)
		{
			string query = string.Format("delete from schema_migrations where version = '{0}'", migration.Version);
			ExecuteNonQuery(query);
		}
		
		public void CreateTable(Table table)
		{
			ExecuteNonQuery(f.GetCreateTable(table));
		}
		
		public void DropTable(string tableName)
		{
			ExecuteNonQuery(f.GetDropTable(tableName));
		}
		
		public void AddColumns(string tableName, params Column[] columns)
		{
			ExecuteNonQuery(f.GetAddColumn(tableName, columns));
		}
		
		public void RemoveColumns(string tableName, params string[] columns)
		{
			ExecuteNonQuery(f.GetDropColumn(tableName, columns));
		}
		
		public void Insert(string tableName, params Column[] columns)
		{
			throw new NotImplementedException();
		}
		
		public void Delete(string tableName, params Column[] columns)
		{
			throw new NotImplementedException();
		}
	}
	
	public interface IMigrationFormatter
	{
		string GetCreateTable(Table table);
		
		string GetDropTable(string tableName);
		
		string GetAddColumn(string tableName, params Column[] columns);
		
		string GetDropColumn(string tableName, params string[] columns);
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
