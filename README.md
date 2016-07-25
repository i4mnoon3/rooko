# rooko beta
![build](https://travis-ci.org/iescarro/rooko.svg?branch=master)

Simple database migration library

## Install
Install the rooko library from nuget

```
PM> Install-Package Rooko.Core.dll
```

It'll be copied into your solution. The best practice is to create a migration library so if you have application project such as MyApplication, you create a migration library as MyApplication.Migrations. Compile this as a .dll

## Add a migration
This is an example of a migration class

```cs
public class CreateUsers : Rooko.Core.Migration
{
	public CreateUsers() : base("E128A916-A06E-4142-B73D-DD0E6811D618")
	{
	}
	
	public override void Migrate()
	{
		CreateTable(
			"users",
			new Column("id", "integer", true, true, true),
			new Column("name"),
			new Column("password")
		);
	}
	
	public override void Rollback()
	{
		DropTable("users");
	}
}
```

## Rooko
Get the rooko executable [here](https://github.com/iescarro/rooko/releases).

Going to the output of this folder after compiling and running

```
> rooko migrate "MyProj.Migrations.dll" "Server=.;Database=test;Trusted_Connection=True;" "System.Data.SqlClient"
```

This will create a users table with id, name, and password columns specified in the Migrate method.

In any case you want to rollback the migration, you can do so by

```
> rooko rollback "MyProj.Migrations.dll" "Server=.;Database=test;Trusted_Connection=True;" "System.Data.SqlClient"
```
