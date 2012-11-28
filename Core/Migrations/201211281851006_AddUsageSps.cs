namespace Core.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddUsageSps : DbMigration
    {
        public override void Up()
        {
            Sql(@"CREATE PROCEDURE [dbo].[GetHourlyUsage]	
	                    @Date DATETIME	
                    AS
                    BEGIN
	                    SELECT DATEPART(HH, [Timestamp]) as Hour, E1Current - E1Start As E1, E2Current-E2Start As E2, (E1Current - E1Start) + (E2Current - E2Start) As ETotal, GasCurrent - GasStart As Gas
	                    FROM Usages
	                    WHERE CONVERT(DATETIME, FLOOR(CONVERT(FLOAT, [Timestamp]))) = @Date
                    END");

            Sql(@"CREATE PROCEDURE [dbo].[GetDailyUsage]	
	                @StartDate DATETIME,
	                @EndDate DATETIME
                AS
                BEGIN
	                SELECT 
		                CONVERT(DATETIME, FLOOR(CONVERT(FLOAT, [Timestamp]))) as Day, 
		                SUM(E1Current - E1Start) As E1, SUM(E2Current-E2Start) As E2, SUM((E1Current - E1Start) + (E2Current - E2Start)) As ETotal, SUM(GasCurrent - GasStart) As Gas
	                FROM Usages
	                WHERE CONVERT(DATETIME, FLOOR(CONVERT(FLOAT, [Timestamp]))) BETWEEN @StartDate AND @EndDate
	                GROUP BY CONVERT(DATETIME, FLOOR(CONVERT(FLOAT, [Timestamp])))
                END");

            Sql(@"CREATE PROCEDURE [dbo].[GetWeeklyUsage]	
	                @Year INT,
	                @StartWeek DATETIME,
	                @EndWeek DATETIME
                AS
                BEGIN
	                SELECT 
		                DATEPART(ww, [Timestamp]) as Week, 
		                SUM(E1Current - E1Start) As E1, SUM(E2Current-E2Start) As E2, SUM((E1Current - E1Start) + (E2Current - E2Start)) As ETotal, SUM(GasCurrent - GasStart) As Gas
	                FROM Usages
	                WHERE DATEPART(ww, [Timestamp]) >= @StartWeek AND DATEPART(ww, [Timestamp]) <= @EndWeek AND DATEPART(yy, [Timestamp]) = @Year
	                GROUP BY DATEPART(ww, [Timestamp])
                END");

            Sql(@"CREATE PROCEDURE [dbo].[GetMonthlyUsage]	
	                @Year INT	
                AS
                BEGIN
	                SELECT 
		                DATEPART(mm, [Timestamp]) as Month, 
		                SUM(E1Current - E1Start) As E1, SUM(E2Current-E2Start) As E2, SUM((E1Current - E1Start) + (E2Current - E2Start)) As ETotal, SUM(GasCurrent - GasStart) As Gas
	                FROM Usages
	                WHERE DATEPART(yy, [Timestamp]) = @Year
	                GROUP BY DATEPART(mm, [Timestamp])
                END");
        }
        
        public override void Down()
        {
        }
    }
}
