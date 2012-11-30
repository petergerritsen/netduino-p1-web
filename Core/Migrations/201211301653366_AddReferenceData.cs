namespace Core.Migrations {
    using System;
    using System.Data.Entity.Migrations;

    public partial class AddReferenceData : DbMigration {
        public override void Up() {
            CreateTable(
                "dbo.References",
                c => new {
                    ReferenceId = c.Long(nullable: false, identity: true),                    
                    Date = c.DateTime(nullable: false),
                    Electricity = c.Decimal(nullable: false, precision: 10, scale: 3),
                    Gas = c.Decimal(nullable: false, precision: 10, scale: 3),
                })
                .PrimaryKey(t => t.ReferenceId)
                .Index(t => t.Date);
        }

        public override void Down() {
        }
    }
}
