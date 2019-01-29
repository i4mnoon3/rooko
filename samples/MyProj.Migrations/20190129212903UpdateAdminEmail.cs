using System;
using System.Collections.Generic;
using System.Data;
using Rooko.Core;

namespace MyProj.Migrations
{
    public class _20190129212903UpdateAdminEmail : Migration
    {
        public _20190129212903UpdateAdminEmail() : base("4355cc2c-36f4-4d10-bccf-c7ffaedd35e1")
        {
        }
        
        public override void Migrate()
        {
            Update(
                "users",
                new[] {
                    new KeyValuePair<string, object>("email", "admin@localhost.com")
                },
                new[] {
                    new KeyValuePair<string, object>("name", "admin")
                });
        }
        
        public override void Rollback()
        {
            Update(
                "users",
                new[] {
                    new KeyValuePair<string, object>("email", "")
                },
                new[] {
                    new KeyValuePair<string, object>("name", "admin")
                });
        }
    }
}
