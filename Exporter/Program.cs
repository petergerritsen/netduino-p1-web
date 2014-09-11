using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using Newtonsoft.Json;

namespace Exporter {
    public class Program {
        static void Main(string[] args) {
            var connectionString = args[0];
            var key = args[1];

            using (var conn = new SqlConnection(connectionString)) {
                conn.Open();

                var usages = conn.Query<Db.Usage>("SELECT U.* FROM Usages U INNER JOIN Users ON Users.UserId = U.UserId WHERE ApiKey = @Key ORDER BY [Timestamp]", new { Key = key }, commandType: CommandType.Text).ToList();
                var references = conn.Query<Db.Reference>("SELECT R.* FROM [References] R INNER JOIN Users ON Users.UserId = R.UserId WHERE ApiKey = @Key ORDER BY [Date]", new { Key = key }, commandType: CommandType.Text).ToList();

                var startDate = usages.First().Timestamp.Date;
                var endDate = usages.Last().Timestamp.Date;

                var lastUsage = usages.First();

                var newUsages = new List<DailyUsage>();

                for (DateTime date = startDate; date <= endDate; date = date.AddDays(1)) {
                    var newDailyUsage = new DailyUsage() { Date = date };
                    
                    var reference = references.SingleOrDefault(x => x.Date == date);
                    newDailyUsage.Reference = new Reference();
                    if (reference != null) {
                        newDailyUsage.Reference.Electricity = reference.Electricity;
                        newDailyUsage.Reference.Gas = reference.Gas;
                    }

                    var dateUsages = usages.Where(x => x.Timestamp >= date && x.Timestamp < date.AddDays(1)).OrderBy(x=> x.Timestamp).ToList();
                    newDailyUsage.Standings = new HourlyStanding[25];
                    for (int i = 0; i <= 24; i++) {
                        var hourlyUsage = dateUsages.SingleOrDefault(x => x.Timestamp == date.AddHours(i));
                        if (hourlyUsage != null)
                            lastUsage = hourlyUsage;
                        newDailyUsage.Standings[i] = new HourlyStanding() {
                            Hour = i,
                            E1 = i == 24 ? lastUsage.E1Current : lastUsage.E1Start,
                            E2 = i == 24 ? lastUsage.E2Current: lastUsage.E2Start,
                            E1Retour = i == 24 ? lastUsage.E1RetourCurrent : lastUsage.E1RetourStart,
                            E2Retour = i == 24 ? lastUsage.E2RetourCurrent : lastUsage.E2RetourStart,
                            PvProduction = 0,
                            Gas = i == 24 ? lastUsage.GasCurrent : lastUsage.GasStart
                        };
                    }

                    newUsages.Add(newDailyUsage);
                }

                string json = JsonConvert.SerializeObject(newUsages.ToArray());

                System.IO.File.WriteAllText(@"C:\temp\mongousages.txt", json);
            }
        }
    }
}
