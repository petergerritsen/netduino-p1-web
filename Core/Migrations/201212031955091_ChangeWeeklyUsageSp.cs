namespace Core.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ChangeWeeklyUsageSp : DbMigration
    {
        public override void Up()
        {
            Sql(@"ALTER PROCEDURE [dbo].[GetWeeklyUsage]	
                    @Key VARCHAR(50),
	                @StartDate DATETIME,
	                @EndDate DATETIME
                AS
                BEGIN
	                SELECT 
		                DATEPART(isoww, [Timestamp]) as Week, 
		                SUM(E1Current - E1Start) As E1, SUM(E2Current-E2Start) As E2, SUM((E1Current - E1Start) + (E2Current - E2Start)) As ETotal, 
		                SUM(E1RetourCurrent - E1RetourStart) As E1Retour, SUM(E2RetourCurrent-E2RetourStart) As E2Retour, SUM((E1RetourCurrent - E1RetourStart) + (E2RetourCurrent - E2RetourStart)) As ERetourTotal, 
                        SUM(GasCurrent - GasStart) As Gas, ISNULL(MIN(R.Electricity), 0) As EleRef, ISNULL(MIN(R.Gas), 0) As GasRef
	                FROM Usages
                    INNER JOIN Users ON Users.UserId = Usages.UserId
                    LEFT JOIN (SELECT UserId, DATEPART(isoww, [Date]) As [Week], SUM(Electricity) As Electricity, SUM(Gas) As Gas FROM [References] WHERE [Date] >= @StartDate AND [Date] <= @EndDate GROUP BY UserId, DATEPART(isoww, [Date]), DATEPART(yy, [Date])) R ON Users.UserId = R.UserId AND R.[Week] = DATEPART(isoww, [Timestamp])
	                WHERE [Timestamp] >= @StartDate AND [Timestamp] <= @EndDate AND Users.ApiKey = @Key
	                GROUP BY DATEPART(isoww, [Timestamp])
                END");
        }
        
        public override void Down()
        {
        }
    }
}
