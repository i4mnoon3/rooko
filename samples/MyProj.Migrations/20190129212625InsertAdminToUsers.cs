using System;
using System.Collections.Generic;
using System.Data;
using Rooko.Core;

namespace MyProj.Migrations
{
    public class _20190129212625InsertAdminToUsers : Migration
    {
        public _20190129212625InsertAdminToUsers() : base("2c94ccaa-2a80-4775-bfed-963625b6089d")
        {
        }
        
        public override void Migrate()
        {
            Insert(
                "users",
                new[] {
                    new KeyValuePair<string, object>("name", "admin"),
                    new KeyValuePair<string, object>("password", "root"),
                });
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
