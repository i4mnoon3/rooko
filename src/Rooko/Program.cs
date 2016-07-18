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
				#if DEBUG
				Console.Write("Press any key to continue...");
				Console.ReadLine();
				#endif
			} else if (args.Length == 1 && args[0] == "-v") {
				Console.WriteLine(Assembly.GetExecutingAssembly().GetName().Version);
			} else {
				Console.WriteLine(@"Rooko is a simple database migration tool for .Net.

  Usage:
    rook -v
    rooko command [library] [connection_string] [provider_name]
    
  Examples:
    rooko migrate ""Rooko.Tests.dll"" ""Server=.;Database=test;Trusted_Connection=True;"" ""System.Data.SqlClient""
    rooko rollback ""Rooko.Tests.dll"" ""Server=.;Database=test;Trusted_Connection=True;"" ""System.Data.SqlClient""
  	
  Further Information:
    https://github.com/iescarro/rooko
");
			}
		}
		
		static IMigrationRepository GetMigrationRepository(string providerName,string connectionString)
		{
			if(providerName == "System.Data.SQLite") {
				return new SQLiteMigrationRepository(connectionString);
			} else if (providerName == "MySql.Data.MySqlClient"){
				return new MySQLMigrationRepository(connectionString);
			} else if (providerName == "System.Data.SqlClient") {
				return new SqlMigrationRepository(connectionString);
			} else {
				throw new NotSupportedException();
			}
		}
	}
}
