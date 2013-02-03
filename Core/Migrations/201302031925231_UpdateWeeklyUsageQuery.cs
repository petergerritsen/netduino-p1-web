namespace Core.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdateWeeklyUsageQuery : DbMigration
    {
        public override void Up()
        {
            Sql(@"CREATE PROCEDURE [dbo].[GetTotalUsage]	
                @Key VARCHAR(50),
                @StartDate DATETIME,
                @EndDate DATETIME
            AS
            BEGIN
		        SELECT COUNT(RS.Day) As NumberOfDays, SUM(RS.E1) As E1, SUM(RS.E2) As E2, SUM(RS.ETotal) AS ETotal, SUM(RS.E1Retour) As E1Retour, SUM(RS.E2Retour) As E2Retour, SUM(RS.ERetourTotal) As E1RetourTotal,
		        SUM(RS.Gas) As Gas, SUM(RS.EleRef) As EleRef, SUM(RS.GasRef) As GasRef
		        FROM
		        (SELECT 
                    CONVERT(DATETIME, FLOOR(CONVERT(FLOAT, [Timestamp]))) as Day, 
                    SUM(E1Current - E1Start) As E1, SUM(E2Current-E2Start) As E2, SUM((E1Current - E1Start) + (E2Current - E2Start)) As ETotal, 
                    SUM(E1RetourCurrent - E1RetourStart) As E1Retour, SUM(E2RetourCurrent-E2RetourStart) As E2Retour, SUM((E1RetourCurrent - E1RetourStart) + (E2RetourCurrent - E2RetourStart)) As ERetourTotal, 
                    SUM(GasCurrent - GasStart) As Gas, MIN([References].Electricity) As EleRef, MIN([References].Gas) As GasRef
                FROM Usages
                INNER JOIN Users ON Users.UserId = Usages.UserId
                LEFT JOIN [References] ON Users.UserId = [References].UserId AND [References].Date = CONVERT(DATETIME, FLOOR(CONVERT(FLOAT, [Timestamp])))
                WHERE CONVERT(DATETIME, FLOOR(CONVERT(FLOAT, [Timestamp]))) BETWEEN @StartDate AND @EndDate AND Users.ApiKey = @Key
                GROUP BY CONVERT(DATETIME, FLOOR(CONVERT(FLOAT, [Timestamp])))
                ) AS RS
        
           
            END");
        }
        
        public override void Down()
        {
        }
    }
}
