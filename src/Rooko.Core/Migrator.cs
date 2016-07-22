//	<file>
//		<license></license>
//		<owner name="Ian Escarro" email="ian.escarro@gmail.com"/>
//	</file>

using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SQLite;
using System.Reflection;
using MySql.Data.MySqlClient;

namespace Rooko.Core
{
	public class Migrator
	{
		MigrationRepository repository;
		List<Migration> migrations;
		
		public Migrator(Assembly assembly, IMigrationFormatter formatter)
		{
			this.repository = new MigrationRepository(formatter);
			this.migrations = new List<Migration>();
			foreach (var t in assembly.GetTypes()) {
				if (t != typeof(Migration) && typeof(Migration).IsAssignableFrom(t)) {
					var m = (Migration)assembly.CreateInstance(t.ToString());
					migrations.Add(m);
				}
			}
		}
		
		public event EventHandler<MigrationEventArgs> Migrating;
		
		public void Migrate()
		{
			foreach (var m in migrations) {
				try {
					m.Migrating += new EventHandler<MigrationEventArgs>(MigrationMigrating);
					
					m.TableCreate += new EventHandler<TableEventArgs>(MigrationTableCreate);
					m.TableDrop += new EventHandler<TableEventArgs>(MigrationTableDrop);
					m.ColumnAdd += new EventHandler<TableEventArgs>(MigrationColumnAdd);
					m.ColumnRemove += new EventHandler<TableEventArgs>(MigrationColumnRemove);
					m.Inserting += new EventHandler<TableEventArgs>(MigrationInserting);
					m.Deleting += new EventHandler<TableEventArgs>(MigrationDeleting);
					m.Updating += new EventHandler<TableEventArgs>(MigrationUpdating);
					
					if (!repository.SchemaExists()) {
						repository.BuildSchema();
					}
					if (!repository.VersionExists(m.Version)) {
						m.Migrate();
						repository.Save(m);
					}
				} catch (Exception ex) {
					Console.WriteLine("Error: " + ex.Message);
				} finally {
					m.Migrating -= new EventHandler<MigrationEventArgs>(MigrationMigrating);
					
					m.TableCreate -= new EventHandler<TableEventArgs>(MigrationTableCreate);
					m.TableDrop -= new EventHandler<TableEventArgs>(MigrationTableDrop);
					m.ColumnAdd -= new EventHandler<TableEventArgs>(MigrationColumnAdd);
					m.ColumnRemove -= new EventHandler<TableEventArgs>(MigrationColumnRemove);
					m.Inserting -= new EventHandler<TableEventArgs>(MigrationInserting);
					m.Deleting -= new EventHandler<TableEventArgs>(MigrationDeleting);
					m.Updating -= new EventHandler<TableEventArgs>(MigrationUpdating);
				}
			}
		}

		void MigrationUpdating(object sender, TableEventArgs e)
		{
			repository.Update(e.Table.Name, e.Values, e.Where);
		}
		
		public void Rollback()
		{
			try {
				string latestVersion = repository.ReadLatestVersion();
				if (latestVersion != null && latestVersion != "") {
					Migration m = migrations.Find(x => x.Version == latestVersion);
					if (m != null) {
						m.Migrating += new EventHandler<MigrationEventArgs>(MigrationMigrating);
						m.Rollback();
						repository.Delete(m);
					}
				}
			} catch (Exception ex) {
				Console.WriteLine("Error: " + ex.Message);
			}
		}
		
		protected virtual void OnMigrating(MigrationEventArgs e)
		{
			if (Migrating != null) {
				Migrating(this, e);
			}
		}

		void MigrationDeleting(object sender, TableEventArgs e)
		{
			repository.Delete(e.Table.Name, e.Where);
		}

		void MigrationInserting(object sender, TableEventArgs e)
		{
			repository.Insert(e.Table.Name, e.Values);
		}

		void MigrationColumnRemove(object sender, TableEventArgs e)
		{
			repository.RemoveColumns(e.Table.Name, e.Table.ColumnNames.ToArray());
		}

		void MigrationColumnAdd(object sender, TableEventArgs e)
		{
			repository.AddColumns(e.Table.Name, e.Table.Columns.ToArray());
		}

		void MigrationTableDrop(object sender, TableEventArgs e)
		{
			repository.DropTable(e.Table.Name);
		}

		void MigrationTableCreate(object sender, TableEventArgs e)
		{
			repository.CreateTable(e.Table);
		}

		void MigrationMigrating(object sender, MigrationEventArgs e)
		{
			OnMigrating(e);
		}
	}
}
