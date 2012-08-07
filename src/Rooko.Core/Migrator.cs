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
		IMigrationRepository repository;
		List<Migration> migrations;
		
		public Migrator(Assembly assembly, IMigrationRepository repository)
		{
			this.repository = repository;
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
					m.Migrate();
				} catch {
					throw;
				} finally {
					m.Migrating -= new EventHandler<MigrationEventArgs>(MigrationMigrating);
				}
			}
		}
		
		public void Rollback()
		{
			Migration latest = repository.ReadLatest();
			if (latest != null) {
				Migration m = migrations.Find(x => x.Version == latest.Version);
				m.Repository = repository;
				m.Rollback();
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
