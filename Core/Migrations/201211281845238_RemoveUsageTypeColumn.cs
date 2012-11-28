namespace Core.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RemoveUsageTypeColumn : DbMigration
    {
        public override void Up()
        {
            DropIndex("dbo.Usages", "IX_UsageType");
            DropColumn("dbo.Usages", "UsageType");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Usages", "UsageType", c => c.Int(nullable: false));
            CreateIndex("dbo.Usages", "UsageType");
        }
    }
}
