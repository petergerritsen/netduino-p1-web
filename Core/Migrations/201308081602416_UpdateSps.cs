namespace Core.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdateSps : DbMigration
    {
        public override void Up()
        {
            Sql(@"ALTER PROCEDURE [dbo].[GetTotalUsage]	
        @Key VARCHAR(50),
        @StartDate DATETIME,
        @EndDate DATETIME,
        @Year INT
    AS
    BEGIN
		SELECT DATEPART(dy, GETDATE()) As NumberOfDays, SUM(RS.E1) As E1, SUM(RS.E2) As E2, SUM(RS.ETotal) AS ETotal, SUM(RS.E1Retour) As E1Retour, SUM(RS.E2Retour) As E2Retour, SUM(RS.ERetourTotal) As ERetourTotal,
		SUM(RS.Gas) As Gas, SUM(RS.EleRef) As EleRef, SUM(RS.GasRef) As GasRef, MAX(RYEAR.ERefYear) As ERefYear, MAX(RYEAR.GasRefYear) As GasRefYear,
		CAST((SUM(RS.ETotal) / SUM(RS.EleRef)) * 100 As Int) As EPercentage, CAST((SUM(RS.ETotal) / SUM(RS.EleRef)) * MAX(RYEAR.ERefYear) As Int) As EEstimated,
		CAST((SUM(RS.Gas) / SUM(RS.GasRef)) * 100 As Int) As GasPercentage, CAST((SUM(RS.Gas) / SUM(RS.GasRef)) * MAX(RYEAR.GasRefYear) As Int) As GasEstimated
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
        INNER JOIN (
			SELECT SUM(R.Electricity) As ERefYear, SUM(R.Gas) As GasRefYear
			FROM [References] R
			INNER JOIN Users ON Users.UserId = R.UserId
			WHERE Users.ApiKey = @Key AND DATEPART(yy, R.[Date]) = @Year
        ) AS RYEAR
        ON 1 = 1                   
    END");

            Sql(@"ALTER PROCEDURE [dbo].[ConsolidateUsages]

AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    update Usages set E1Current = (select MAX(e1current) from Usages U2 where CONVERT(DATETIME, FLOOR(CONVERT(FLOAT, Usages.[Timestamp]))) = CONVERT(DATETIME, FLOOR(CONVERT(FLOAT, U2.[Timestamp])))) where DATEPART(HH, [timestamp]) = (SELECT MIN(DATEPART(HH, [timestamp])) FROM Usages u2 where CONVERT(DATETIME, FLOOR(CONVERT(FLOAT, Usages.[Timestamp]))) = CONVERT(DATETIME, FLOOR(CONVERT(FLOAT, U2.[Timestamp])))) AND [Timestamp] < CONVERT(DATETIME, FLOOR(CONVERT(FLOAT, DATEADD(dd, -14, GETDATE()))))
	update Usages set E2Current = (select MAX(e2current) from Usages U2 where CONVERT(DATETIME, FLOOR(CONVERT(FLOAT, Usages.[Timestamp]))) = CONVERT(DATETIME, FLOOR(CONVERT(FLOAT, U2.[Timestamp])))) where DATEPART(HH, [timestamp]) = (SELECT MIN(DATEPART(HH, [timestamp])) FROM Usages u2 where CONVERT(DATETIME, FLOOR(CONVERT(FLOAT, Usages.[Timestamp]))) = CONVERT(DATETIME, FLOOR(CONVERT(FLOAT, U2.[Timestamp])))) AND [Timestamp] < CONVERT(DATETIME, FLOOR(CONVERT(FLOAT, DATEADD(dd, -14, GETDATE()))))
	update Usages set GasCurrent = (select MAX(GasCurrent) from Usages U2 where CONVERT(DATETIME, FLOOR(CONVERT(FLOAT, Usages.[Timestamp]))) = CONVERT(DATETIME, FLOOR(CONVERT(FLOAT, U2.[Timestamp])))) where DATEPART(HH, [timestamp]) = (SELECT MIN(DATEPART(HH, [timestamp])) FROM Usages u2 where CONVERT(DATETIME, FLOOR(CONVERT(FLOAT, Usages.[Timestamp]))) = CONVERT(DATETIME, FLOOR(CONVERT(FLOAT, U2.[Timestamp])))) AND [Timestamp] < CONVERT(DATETIME, FLOOR(CONVERT(FLOAT, DATEADD(dd, -14, GETDATE()))))
	delete from Usages where DATEPART(HH, [timestamp]) <> (SELECT MIN(DATEPART(HH, [timestamp])) FROM Usages u2 where CONVERT(DATETIME, FLOOR(CONVERT(FLOAT, Usages.[Timestamp]))) = CONVERT(DATETIME, FLOOR(CONVERT(FLOAT, U2.[Timestamp])))) AND [Timestamp] < CONVERT(DATETIME, FLOOR(CONVERT(FLOAT, DATEADD(dd, -14, GETDATE()))))

END");
        }
        
        public override void Down()
        {
        }
    }
}
