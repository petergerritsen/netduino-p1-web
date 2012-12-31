namespace Core.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddSortingToSps : DbMigration
    {
        public override void Up()
        {
            Sql(@"ALTER PROCEDURE [dbo].[GetMonthlyUsage]	
                    @Key VARCHAR(50),   
	                @Year INT	
                AS
                BEGIN
	                SELECT 
		                DATEPART(mm, [Timestamp]) as Month, 
		                SUM(E1Current - E1Start) As E1, SUM(E2Current-E2Start) As E2, SUM((E1Current - E1Start) + (E2Current - E2Start)) As ETotal, 
		                SUM(E1RetourCurrent - E1RetourStart) As E1Retour, SUM(E2RetourCurrent-E2RetourStart) As E2Retour, SUM((E1RetourCurrent - E1RetourStart) + (E2RetourCurrent - E2RetourStart)) As ERetourTotal, 
                        SUM(GasCurrent - GasStart) As Gas, ISNULL(MIN(R.Electricity), 0) As EleRef, ISNULL(MIN(R.Gas), 0) As GasRef
	                FROM Usages
                    INNER JOIN Users ON Users.UserId = Usages.UserId
                    LEFT JOIN (SELECT UserId, DATEPART(mm, [Date]) As [Month], SUM(Electricity) As Electricity, SUM(Gas) As Gas FROM [References] WHERE DATEPART(yy, [Date]) = @Year GROUP BY UserId, DATEPART(mm, [Date])) R ON Users.UserId = R.UserId AND R.[Month] = DATEPART(mm, [Timestamp])
	                WHERE DATEPART(yy, [Timestamp]) = @Year AND Users.ApiKey = @Key
	                GROUP BY DATEPART(mm, [Timestamp])
	                ORDER BY DATEPART(mm, [TimeStamp])
                END");

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
                    LEFT JOIN (SELECT UserId, DATEPART(yy, [Date]) As [Year], DATEPART(isoww, [Date]) As [Week], SUM(Electricity) As Electricity, SUM(Gas) As Gas FROM [References] WHERE [Date] >= @StartDate AND [Date] <= @EndDate GROUP BY UserId, DATEPART(isoww, [Date]), DATEPART(yy, [Date])) R ON Users.UserId = R.UserId AND R.[Week] = DATEPART(isoww, [Timestamp]) AND R.[Year] = DATEPART(YY, [Timestamp])
	                WHERE [Timestamp] >= @StartDate AND [Timestamp] <= @EndDate AND Users.ApiKey = @Key
	                GROUP BY DATEPART(isoww, [Timestamp])
	                ORDER BY MAX([Timestamp])
                END");

            Sql(@"ALTER PROCEDURE [dbo].[GetDailyUsage]	
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
	                ORDER BY CONVERT(DATETIME, FLOOR(CONVERT(FLOAT, [Timestamp])))
                END");

            Sql(@"ALTER PROCEDURE [dbo].[GetHourlyUsage]	
                        @Key VARCHAR(50),
	                    @Date DATETIME	
                    AS
                    BEGIN
	                    SELECT DATEPART(HH, [Timestamp]) as Hour, E1Current - E1Start As E1, E2Current-E2Start As E2, (E1Current - E1Start) + (E2Current - E2Start) As ETotal, E1RetourCurrent - E1RetourStart As E1Retour, E2RetourCurrent-E2RetourStart As E2Retour, (E1RetourCurrent - E1RetourStart) + (E2RetourCurrent - E2RetourStart) As ERetourTotal, GasCurrent - GasStart As Gas
	                    FROM Usages
                        INNER JOIN Users ON Users.UserId = Usages.UserId
	                    WHERE CONVERT(DATETIME, FLOOR(CONVERT(FLOAT, [Timestamp]))) = @Date AND Users.ApiKey = @Key
	                    ORDER BY DATEPART(HH, [Timestamp])
                    END");
        }
        
        public override void Down()
        {
        }
    }
}
