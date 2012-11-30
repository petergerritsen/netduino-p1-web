namespace Core.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class FixDailyUsageQuery : DbMigration
    {
        public override void Up()
        {
            Sql(@"IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GetDailyUsage]') AND type in (N'P', N'PC'))
                    DROP PROCEDURE [dbo].[GetDailyUsage]");

            Sql(@"CREATE PROCEDURE [dbo].[GetDailyUsage]	
                    @Key VARCHAR(50),
	                @StartDate DATETIME,
	                @EndDate DATETIME
                AS
                BEGIN
	                SELECT 
		                CONVERT(DATETIME, FLOOR(CONVERT(FLOAT, [Timestamp]))) as Day, 
		                SUM(E1Current - E1Start) As E1, SUM(E2Current-E2Start) As E2, SUM((E1Current - E1Start) + (E2Current - E2Start)) As ETotal, 
		                SUM(E1RetourCurrent - E1RetourStart) As E1Retour, SUM(E2RetourCurrent-E2RetourStart) As E2Retour, SUM((E1RetourCurrent - E1RetourStart) + (E2RetourCurrent - E2RetourStart)) As ERetourTotal, 
                        SUM(GasCurrent - GasStart) As Gas, MIN([References].Electricity) As EleRef, MIN([References].Gas) As GasRef
	                FROM Usages
                    INNER JOIN Users ON Users.UserId = Usages.UserId
                    LEFT JOIN [References] ON Users.UserId = [References].UserId AND [References].Date = CONVERT(DATETIME, FLOOR(CONVERT(FLOAT, [Timestamp])))
	                WHERE CONVERT(DATETIME, FLOOR(CONVERT(FLOAT, [Timestamp]))) BETWEEN @StartDate AND @EndDate AND Users.ApiKey = @Key
	                GROUP BY CONVERT(DATETIME, FLOOR(CONVERT(FLOAT, [Timestamp])))
                END");
        }
        
        public override void Down()
        {
        }
    }
}
