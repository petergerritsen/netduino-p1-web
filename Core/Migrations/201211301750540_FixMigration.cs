namespace Core.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class FixMigration : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.References", "UserId", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.References", "UserId");
        }
    }
}
