namespace Core.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddPvCounter : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.LogEntries", "PvCounter", c => c.Int(nullable: false));
            AddColumn("dbo.Usages", "PvProductionStart", c => c.Int(nullable: false));
            AddColumn("dbo.Usages", "PvProductionCurrent", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Usages", "PvProductionCurrent");
            DropColumn("dbo.Usages", "PvProductionStart");
            DropColumn("dbo.LogEntries", "PvCounter");
        }
    }
}
