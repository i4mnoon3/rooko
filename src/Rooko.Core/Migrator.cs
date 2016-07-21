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
					migrations.Add((Migration)assembly.CreateInstance(t.ToString()));
				}
			}
		}
		
		public event EventHandler<MigrationEventArgs> Migrating;
		
		public void Migrate()
		{
			foreach (var m in migrations) {
				try {
					m.Migrating += new EventHandler<MigrationEventArgs>(MigrationMigrating);
					m.Repository = repository;
					if (!m.SchemaExists()) {
						m.BuildSchema();
					}
					if (!m.Exists()) {
						m.Migrate();
						m.Save();
					}
				} catch (Exception ex) {
					Console.WriteLine("Error: " + ex.Message);
				} finally {
					m.Migrating -= new EventHandler<MigrationEventArgs>(MigrationMigrating);
				}
			}
		}
		
		public void Rollback()
		{
			try {
				string latestVersion = repository.ReadLatestVersion();
				if (latestVersion != null && latestVersion != "") {
					Migration m = migrations.Find(x => x.Version == latestVersion);
					if (m != null) {
						m.Migrating += new EventHandler<MigrationEventArgs>(MigrationMigrating);
						m.Repository = repository;
						m.Rollback();
						m.DeleteMigration();
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

		void MigrationMigrating(object sender, MigrationEventArgs e)
		{
			OnMigrating(e);
		}
	}
}
