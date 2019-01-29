using System;
using System.Data;
using Rooko.Core;

namespace MyProj.Migrations
{
    public class _20190129212349CreateTableUsers : Migration
    {
        public _20190129212349CreateTableUsers() : base("eabd9d7e-c2c6-4ba2-9f5c-67969d3cc4f3")
        {
        }
        
        public override void Migrate()
        {
            CreateTable(
                "users",
                new Column("id", DbType.Int32, 0, true, true, true),
                new Column("name"),
                new Column("password")
               );
        }
        
        public override void Rollback()
        {
            DropTable("users");
        }
    }
}
