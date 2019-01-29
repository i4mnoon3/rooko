using System;
using System.Collections.Generic;
using Rooko.Core;

namespace MyProj.Migrations
{
    public class UpdateTable : Migration
    {
        public UpdateTable() : base("38AC0CD4-9306-41AA-A91C-86B0140DB101")
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
