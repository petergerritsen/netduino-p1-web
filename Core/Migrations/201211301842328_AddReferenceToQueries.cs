namespace Core.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddReferenceToQueries : DbMigration
    {
        public override void Up()
        {
            Sql(@"IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GetDailyUsage]') AND type in (N'P', N'PC'))
                    DROP PROCEDURE [dbo].[GetDailyUsage]");
            Sql(@"IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GetWeeklyUsage]') AND type in (N'P', N'PC'))
                    DROP PROCEDURE [dbo].[GetWeeklyUsage]");
            Sql(@"IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GetMonthlyUsage]') AND type in (N'P', N'PC'))
                    DROP PROCEDURE [dbo].[GetMonthlyUsage]");

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
                    INNER JOIN [References] ON Users.UserId = [References].UserId AND [References].Date = CONVERT(DATETIME, FLOOR(CONVERT(FLOAT, [Timestamp])))
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
                    SET DATEFIRST 1
    
	                SELECT 
		                DATEPART(isoww, [Timestamp]) as Week, 
		                SUM(E1Current - E1Start) As E1, SUM(E2Current-E2Start) As E2, SUM((E1Current - E1Start) + (E2Current - E2Start)) As ETotal, 
		                SUM(E1RetourCurrent - E1RetourStart) As E1Retour, SUM(E2RetourCurrent-E2RetourStart) As E2Retour, SUM((E1RetourCurrent - E1RetourStart) + (E2RetourCurrent - E2RetourStart)) As ERetourTotal, 
                        SUM(GasCurrent - GasStart) As Gas, ISNULL(MIN(R.Electricity), 0) As EleRef, ISNULL(MIN(R.Gas), 0) As GasRef
	                FROM Usages
                    INNER JOIN Users ON Users.UserId = Usages.UserId
                    LEFT JOIN (SELECT UserId, DATEPART(ww, [Date]) As [Week], SUM(Electricity) As Electricity, SUM(Gas) As Gas FROM [References] WHERE DATEPART(yy, [Date]) = @Year GROUP BY UserId, DATEPART(isoww, [Date]), DATEPART(yy, [Date])) R ON Users.UserId = R.UserId AND R.[Week] = DATEPART(isoww, [Timestamp])
	                WHERE DATEPART(isoww, [Timestamp]) >= @StartWeek AND DATEPART(isoww, [Timestamp]) <= @EndWeek AND DATEPART(yy, [Timestamp]) = @Year AND Users.ApiKey = @Key
	                GROUP BY DATEPART(isoww, [Timestamp])
                END");

            Sql(@"CREATE PROCEDURE [dbo].[GetMonthlyUsage]	
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
                END");     
        }
        
        public override void Down()
        {
        }
    }
}
