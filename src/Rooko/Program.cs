//	<file>
//		<license></license>
//		<owner name="Ian Escarro" email="ian.escarro@gmail.com"/>
//	</file>

using System;
using System.IO;
using System.Reflection;
using System.Windows.Forms;
using Rooko.Core;

namespace Rooko
{
	class Program
	{
		private static void Main(string[] args)
		{
			if (args.Length >= 4) {
				string command = args[0], assembly = args[1], connectionString = args[2], providerName =  args[3];
				var m = new Migrator(Assembly.LoadFile(Path.Combine(Directory.GetCurrentDirectory(), assembly)), GetMigrationRepository(providerName, connectionString));
				m.Migrating += delegate(object sender, MigrationEventArgs e) {
					Console.WriteLine(e.Message);
				};
				if (command == "migrate") {
					m.Migrate();
				} else if (command == "rollback") {
					m.Rollback();
				} else {
					throw new NotSupportedException();
				}
			}
			#if DEBUG
			Console.ReadLine();
			#endif
		}
		
		static IMigrationRepository GetMigrationRepository(string providerName,string connectionString)
		{
			if(providerName == "System.Data.SQLite") {
				return new SQLiteMigrationRepository(connectionString);
			} else if (providerName == "MySql.Data.MySqlClient"){
				return new MySQLMigrationRepository();
			} else {
				throw new NotSupportedException();
			}
		}
	}
}
