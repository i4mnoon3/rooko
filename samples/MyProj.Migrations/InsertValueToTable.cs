using System;
using System.Collections.Generic;
using Rooko.Core;

namespace MyProj.Migrations
{
    public class InsertValueToTable : Migration
    {
        public InsertValueToTable() : base("116AA4B6-5DEE-4621-8D39-DB0FC5D9110E")
        {
        }
        
        public override void Migrate()
        {
            Insert(
                "users",
                new[] {
                    new KeyValuePair<string, object>("name", "admin"),
                    new KeyValuePair<string, object>("password", "root"),
                }
               );
        }
        
        public override void Rollback()
        {
            Delete(
                "users",
                new[] {
                    new KeyValuePair<string, object>("name", "admin"),
                    new KeyValuePair<string, object>("password", "root"),
                });
        }
    }
}
