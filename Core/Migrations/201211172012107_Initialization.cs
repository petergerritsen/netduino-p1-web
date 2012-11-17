namespace Core.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Initialization : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.LogEntries",
                c => new
                    {
                        LogEntryId = c.Long(nullable: false, identity: true),
                        Timestamp = c.DateTime(nullable: false),
                        E1 = c.Decimal(nullable: false, precision: 10, scale: 3),
                        E2 = c.Decimal(nullable: false, precision: 10, scale: 3),
                        E1Retour = c.Decimal(nullable: false, precision: 10, scale: 3),
                        E2Retour = c.Decimal(nullable: false, precision: 10, scale: 3),
                        CurrentTariff = c.Int(nullable: false),
                        CurrentUsage = c.Decimal(nullable: false, precision: 10, scale: 2),
                        CurrentRetour = c.Decimal(nullable: false, precision: 10, scale: 2),
                        GasMeasurementMoment = c.DateTime(nullable: false),
                        GasMeasurementValue = c.Decimal(nullable: false, precision: 10, scale: 3),
                        UserId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.LogEntryId)
                .ForeignKey("dbo.Users", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.Users",
                c => new
                    {
                        UserId = c.Int(nullable: false, identity: true),
                        Email = c.String(),
                        Password = c.String(),
                        ApiKey = c.String(),
                    })
                .PrimaryKey(t => t.UserId);
            
            CreateTable(
                "dbo.Usages",
                c => new
                    {
                        UsageId = c.Int(nullable: false, identity: true),
                        UsageType = c.Int(nullable: false),
                        Timestamp = c.DateTime(nullable: false),
                        E1 = c.Decimal(nullable: false, precision: 10, scale: 3),
                        E2 = c.Decimal(nullable: false, precision: 10, scale: 3),
                        E1Retour = c.Decimal(nullable: false, precision: 10, scale: 3),
                        E2Retour = c.Decimal(nullable: false, precision: 10, scale: 3),
                        Gas = c.Decimal(nullable: false, precision: 10, scale: 3),
                        UserId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.UsageId)
                .ForeignKey("dbo.Users", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId)
                .Index(t=> t.UsageType)
                .Index(t=> t.Timestamp);           
            
        }
        
        public override void Down()
        {
            DropIndex("dbo.Usages", new[] { "UserId" });
            DropIndex("dbo.LogEntries", new[] { "UserId" });
            DropForeignKey("dbo.Usages", "UserId", "dbo.Users");
            DropForeignKey("dbo.LogEntries", "UserId", "dbo.Users");
            DropTable("dbo.Usages");
            DropTable("dbo.Users");
            DropTable("dbo.LogEntries");
        }
    }
}
