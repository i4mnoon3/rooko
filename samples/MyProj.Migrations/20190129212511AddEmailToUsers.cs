using System;
using System.Data;
using Rooko.Core;

namespace MyProj.Migrations
{
    public class _20190129212511AddEmailToUsers : Migration
    {
        public _20190129212511AddEmailToUsers() : base("9c720f2f-8f42-4b3c-85a3-66fcd127ff03")
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
