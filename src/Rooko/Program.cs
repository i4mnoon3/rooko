//	<file>
//		<license></license>
//		<owner name="Ian Escarro" email="ian.escarro@gmail.com"/>
//	</file>

using System;
using System.Configuration;
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
			if (args.Length == 4) {
				MigrateRollback(args);
			} else if (args.Length == 2) {
				MigrateRollback2(args);
			} else if (args.Length == 3) {
				Generate(args);
			} else if (args.Length == 1 && args[0] == "-v") {
				DisplayVersion();
			} else {
				DisplayHelp();
			}
		}
		
		static void MigrateRollback(string[] args)
		{
			string command = args[0], assembly = args[1], connectionString = args[2], providerName = args[3];

			var m = new Migrator(Assembly.LoadFile(Path.Combine(Directory.GetCurrentDirectory(), assembly)), GetMigrationFormatter(providerName, connectionString));
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
		
		static void MigrateRollback2(string[] args)
		{
			string command = args[0], assembly = args[1];

			var a = Assembly.LoadFile(Path.Combine(Directory.GetCurrentDirectory(), assembly));
			var config = ConfigurationManager.OpenExeConfiguration(a.Location);
			var connection = config.ConnectionStrings.ConnectionStrings[config.AppSettings.Settings["database"].Value];
			string connectionString = connection.ConnectionString;
			string providerName = connection.ProviderName;
			var m = new Migrator(a, GetMigrationFormatter(providerName, connectionString));
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
		
		static void Generate(string[] args)
		{
			string command = args[0], subCommand = args[1], name = args[2];
			
			if (command == "generate") {
				if (subCommand == "migration") {
					string fileName = string.Format("{1}{0}.cs", name, DateTime.Now.ToString("yyyyMMddHHmm"));
					using (var s = new StreamWriter(Path.Combine(Directory.GetCurrentDirectory(), fileName))) {
						s.WriteLine(
							@"using System;
using Rooko.Core;

namespace Migrations
{{
	public class {0} : Migration
	{{
		public {0}() : base(""{1}"")
		{{
    	}}
		
		public override void Migrate()
		{{
			base.Migrate();
		}}
		
		public override void Rollback()
		{{
			base.Rollback();
		}}
	}}
}}",
							name,
							Guid.NewGuid().ToString()
						);
					}
				}
			}
		}
		
		static void DisplayVersion()
		{
			Console.WriteLine(Assembly.GetExecutingAssembly().GetName().Version);
		}
		
		static void DisplayHelp()
		{
			Console.WriteLine(
				@"Rooko is a simple database migration tool for .Net.

  Usage:
    rook -v
    rooko command [library] [connection_string] [provider_name]
   
  Examples:
    rooko migrate ""Rooko.Tests.dll"" ""Server=.;Database=test;Trusted_Connection=True;"" ""System.Data.SqlClient""
    rooko rollback ""Rooko.Tests.dll"" ""Server=.;Database=test;Trusted_Connection=True;"" ""System.Data.SqlClient""
    rooko generate migration ""CreateUsers""

  Further Information:
    https://github.com/iescarro/rooko
"
			);
		}
		
		static IMigrationFormatter GetMigrationFormatter(string providerName, string connectionString)
		{
			if (providerName == "System.Data.SQLite") {
				return new SQLiteMigrationFormatter(connectionString);
			} else if (providerName == "MySql.Data.MySqlClient") {
				return new MySQLMigrationFormatter(connectionString);
			} else if (providerName == "System.Data.SqlClient") {
				return new SqlMigrationFormatter(connectionString);
			} else {
				throw new NotSupportedException();
			}
		}
	}
}
