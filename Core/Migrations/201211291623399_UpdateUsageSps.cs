namespace Core.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdateUsageSps : DbMigration
    {
        public override void Up()
        {
            Sql(@"IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GetDailyUsage]') AND type in (N'P', N'PC'))
                    DROP PROCEDURE [dbo].[GetDailyUsage]");
            Sql(@"IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GetHourlyUsage]') AND type in (N'P', N'PC'))
                    DROP PROCEDURE [dbo].[GetHourlyUsage]");
            Sql(@"IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GetWeeklyUsage]') AND type in (N'P', N'PC'))
                    DROP PROCEDURE [dbo].[GetWeeklyUsage]");
            Sql(@"IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GetMonthlyUsage]') AND type in (N'P', N'PC'))
                    DROP PROCEDURE [dbo].[GetMonthlyUsage]");

            Sql(@"CREATE PROCEDURE [dbo].[GetHourlyUsage]	
                        @Key VARCHAR(50),
	                    @Date DATETIME	
                    AS
                    BEGIN
	                    SELECT DATEPART(HH, [Timestamp]) as Hour, E1Current - E1Start As E1, E2Current-E2Start As E2, (E1Current - E1Start) + (E2Current - E2Start) As ETotal, GasCurrent - GasStart As Gas
	                    FROM Usages
                        INNER JOIN Users ON Users.UserId = Usages.UserId
	                    WHERE CONVERT(DATETIME, FLOOR(CONVERT(FLOAT, [Timestamp]))) = @Date AND Users.ApiKey = @Key
                    END");

            Sql(@"CREATE PROCEDURE [dbo].[GetDailyUsage]	
                    @Key VARCHAR(50),
	                @StartDate DATETIME,
	                @EndDate DATETIME
                AS
                BEGIN
	                SELECT 
		                CONVERT(DATETIME, FLOOR(CONVERT(FLOAT, [Timestamp]))) as Day, 
		                SUM(E1Current - E1Start) As E1, SUM(E2Current-E2Start) As E2, SUM((E1Current - E1Start) + (E2Current - E2Start)) As ETotal, SUM(GasCurrent - GasStart) As Gas
	                FROM Usages
                    INNER JOIN Users ON Users.UserId = Usages.UserId
	                WHERE CONVERT(DATETIME, FLOOR(CONVERT(FLOAT, [Timestamp]))) BETWEEN @StartDate AND @EndDate AND Users.ApiKey = @Key
	                GROUP BY CONVERT(DATETIME, FLOOR(CONVERT(FLOAT, [Timestamp])))
                END");

            Sql(@"CREATE PROCEDURE [dbo].[GetWeeklyUsage]	
                    @Key VARCHAR(50),
	                @Year INT,
	                @StartWeek DATETIME,
	                @EndWeek DATETIME
                AS
                BEGIN
	                SELECT 
		                DATEPART(ww, [Timestamp]) as Week, 
		                SUM(E1Current - E1Start) As E1, SUM(E2Current-E2Start) As E2, SUM((E1Current - E1Start) + (E2Current - E2Start)) As ETotal, SUM(GasCurrent - GasStart) As Gas
	                FROM Usages
                    INNER JOIN Users ON Users.UserId = Usages.UserId
	                WHERE DATEPART(ww, [Timestamp]) >= @StartWeek AND DATEPART(ww, [Timestamp]) <= @EndWeek AND DATEPART(yy, [Timestamp]) = @Year AND Users.ApiKey = @Key
	                GROUP BY DATEPART(ww, [Timestamp])
                END");

            Sql(@"CREATE PROCEDURE [dbo].[GetMonthlyUsage]	
                    @Key VARCHAR(50),   
	                @Year INT	
                AS
                BEGIN
	                SELECT 
		                DATEPART(mm, [Timestamp]) as Month, 
		                SUM(E1Current - E1Start) As E1, SUM(E2Current-E2Start) As E2, SUM((E1Current - E1Start) + (E2Current - E2Start)) As ETotal, SUM(GasCurrent - GasStart) As Gas
	                FROM Usages
                    INNER JOIN Users ON Users.UserId = Usages.UserId
	                WHERE DATEPART(yy, [Timestamp]) = @Year AND Users.ApiKey = @Key
	                GROUP BY DATEPART(mm, [Timestamp])
                END");            
        }
        
        public override void Down()
        {
        }
    }
}
