using System;
using System.Collections.Generic;
using System.Data;
using Rooko.Core;

namespace MyProj.Migrations
{
    public class CreateTable : Migration
    {
        public CreateTable() : base("E128A916-A06E-4142-B73D-DD0E6811D618")
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
