using System;
using System.Collections.Generic;
using System.Data;
using Rooko.Core;

namespace MyProj.Migrations
{
    public class _20190129213034DeleteUserAdmin : Migration
    {
        public _20190129213034DeleteUserAdmin() : base("b7f832c8-d266-44f8-8133-0b050ce3d458")
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
