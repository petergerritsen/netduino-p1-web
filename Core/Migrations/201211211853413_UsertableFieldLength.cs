namespace Core.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UsertableFieldLength : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Users", "Email", c => c.String(maxLength: 100));
            AlterColumn("dbo.Users", "Password", c => c.String(maxLength: 25));
            AlterColumn("dbo.Users", "ApiKey", c => c.String(maxLength: 50));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Users", "ApiKey", c => c.String());
            AlterColumn("dbo.Users", "Password", c => c.String());
            AlterColumn("dbo.Users", "Email", c => c.String());
        }
    }
}
