namespace Core.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddPvProduction : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Usages", "PvProductionStart", c => c.Decimal(nullable: false, precision: 10, scale: 3));
            AlterColumn("dbo.Usages", "PvProductionCurrent", c => c.Decimal(nullable: false, precision: 10, scale: 3));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Usages", "PvProductionCurrent", c => c.Long(nullable: false));
            AlterColumn("dbo.Usages", "PvProductionStart", c => c.Long(nullable: false));
        }
    }
}
