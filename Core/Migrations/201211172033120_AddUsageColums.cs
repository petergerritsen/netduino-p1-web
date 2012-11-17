namespace Core.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddUsageColums : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Usages", "E1Start", c => c.Decimal(nullable: false, precision: 10, scale: 3));
            AddColumn("dbo.Usages", "E1Current", c => c.Decimal(nullable: false, precision: 10, scale: 3));
            AddColumn("dbo.Usages", "E2Start", c => c.Decimal(nullable: false, precision: 10, scale: 3));
            AddColumn("dbo.Usages", "E2Current", c => c.Decimal(nullable: false, precision: 10, scale: 3));
            AddColumn("dbo.Usages", "E1RetourStart", c => c.Decimal(nullable: false, precision: 10, scale: 3));
            AddColumn("dbo.Usages", "E1RetourCurrent", c => c.Decimal(nullable: false, precision: 10, scale: 3));
            AddColumn("dbo.Usages", "E2RetourStart", c => c.Decimal(nullable: false, precision: 10, scale: 3));
            AddColumn("dbo.Usages", "E2RetourCurrent", c => c.Decimal(nullable: false, precision: 10, scale: 3));
            AddColumn("dbo.Usages", "GasStart", c => c.Decimal(nullable: false, precision: 10, scale: 3));
            AddColumn("dbo.Usages", "GasCurrent", c => c.Decimal(nullable: false, precision: 10, scale: 3));
            DropColumn("dbo.Usages", "E1");
            DropColumn("dbo.Usages", "E2");
            DropColumn("dbo.Usages", "E1Retour");
            DropColumn("dbo.Usages", "E2Retour");
            DropColumn("dbo.Usages", "Gas");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Usages", "Gas", c => c.Decimal(nullable: false, precision: 10, scale: 3));
            AddColumn("dbo.Usages", "E2Retour", c => c.Decimal(nullable: false, precision: 10, scale: 3));
            AddColumn("dbo.Usages", "E1Retour", c => c.Decimal(nullable: false, precision: 10, scale: 3));
            AddColumn("dbo.Usages", "E2", c => c.Decimal(nullable: false, precision: 10, scale: 3));
            AddColumn("dbo.Usages", "E1", c => c.Decimal(nullable: false, precision: 10, scale: 3));
            DropColumn("dbo.Usages", "GasCurrent");
            DropColumn("dbo.Usages", "GasStart");
            DropColumn("dbo.Usages", "E2RetourCurrent");
            DropColumn("dbo.Usages", "E2RetourStart");
            DropColumn("dbo.Usages", "E1RetourCurrent");
            DropColumn("dbo.Usages", "E1RetourStart");
            DropColumn("dbo.Usages", "E2Current");
            DropColumn("dbo.Usages", "E2Start");
            DropColumn("dbo.Usages", "E1Current");
            DropColumn("dbo.Usages", "E1Start");
        }
    }
}
