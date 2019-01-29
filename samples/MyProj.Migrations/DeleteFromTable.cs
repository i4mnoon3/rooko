using System;
using System.Collections.Generic;
using Rooko.Core;

namespace MyProj.Migrations
{
    public class DeleteFromTable : Migration
    {
        public DeleteFromTable() : base("AACB1EBA-85DF-4DFD-A3AF-998F2BB18DE6")
        {
        }
        
        public override void Migrate()
        {
            Delete(
                "users",
                new[] {
                    new KeyValuePair<string, object>("name", "admin"),
                });
        }
        
        public override void Rollback()
        {
            Insert(
                "users",
                new[] {
                    new KeyValuePair<string, object>("name", "admin"),
                    new KeyValuePair<string, object>("password", "root"),
                    new KeyValuePair<string, object>("email", ""),
                });
        }
    }
}
