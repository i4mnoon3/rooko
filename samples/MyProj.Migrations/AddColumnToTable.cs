using System;
using System.Collections.Generic;
using Rooko.Core;

namespace MyProj.Migrations
{
    public class AddColumnToTable : Migration
    {
        public AddColumnToTable() : base("DA8965AD-31E3-4085-8125-8396758C4A82")
        {
        }
        
        public override void Migrate()
        {
            AddColumn("users", new Column("email"));
        }
        
        public override void Rollback()
        {
            RemoveColumn("users", "email");
        }
    }
}
