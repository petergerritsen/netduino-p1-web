namespace Core.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddLogentryIndexes : DbMigration
    {
        public override void Up()
        {
            CreateIndex("dbo.LogEntries", "Timestamp");
        }
        
        public override void Down()
        {
        }
    }
}
