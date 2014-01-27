using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Configuration;
using System.Data.SqlClient;
using Dapper;
using System.Data;
using System.Globalization;

namespace Web.Controllers {
    public class UsagesController : ApiController {
        private string connectionString = "";

        public UsagesController() {
            connectionString = ConfigurationManager.ConnectionStrings["context"].ConnectionString;
        }

        [HttpGet]
        public IEnumerable<HourlyUsage> Hourly(string key, int offset) {
            var date = DateTime.Today.AddDays(-1 * offset);
            using (var conn = new SqlConnection(connectionString)) {
                conn.Open();
                IEnumerable<HourlyUsage> results = conn.Query<HourlyUsage>("GetHourlyUsage", new { Key = key, Date = date }, commandType: CommandType.StoredProcedure);

                return results;
            }
        }

        [HttpGet]
        public IEnumerable<DailyUsage> Daily(string key, int offset) {
            var diff = DateTime.Today.DayOfWeek - DayOfWeek.Monday;
            diff = (7 + diff) % 7;
            var date = DateTime.Today.AddDays(-1 * diff).AddDays(-7 * offset);
            using (var conn = new SqlConnection(connectionString)) {
                conn.Open();
                return conn.Query<DailyUsage>("GetDailyUsage", new { Key = key, StartDate = date, EndDate = date.AddDays(6) }, commandType: CommandType.StoredProcedure);
            }
        }

        [HttpGet]
        public IEnumerable<WeeklyUsage> Weekly(string key, int offset, int count) {
            // last day of current week
            var diff = DateTime.Today.DayOfWeek - DayOfWeek.Monday;
            diff = (7 + diff) % 7;
            var endDate = DateTime.Today.AddDays(-1 * diff).AddDays(6 + (-7 * offset));
            var startDate = endDate.AddDays((-7 * count) + 1);
            endDate = endDate.AddDays(1).AddSeconds(-1);

            using (var conn = new SqlConnection(connectionString)) {
                conn.Open();
                return conn.Query<WeeklyUsage>("GetWeeklyUsage", new { Key = key, StartDate = startDate, EndDate = endDate }, commandType: CommandType.StoredProcedure);
            }
        }

        [HttpGet]
        public IEnumerable<MonthlyUsage> Monthly(string key, int offset) {
             List<MonthlyUsage> returnValues = null;
            using (var conn = new SqlConnection(connectionString)) {
                conn.Open();
                returnValues = conn.Query<MonthlyUsage>("GetMonthlyUsage", new { Key = key, Year = DateTime.Today.Year - offset }, commandType: CommandType.StoredProcedure).ToList();
            }
            
            foreach (var usage in returnValues) {
                usage.MonthName =  new DateTime(2011, usage.Month, 1).ToString("MMM"); 
            }
            returnValues.Add(new MonthlyUsage() {
                Month = 13,
                MonthName = "Total",
                E1 = returnValues.Sum(x => x.E1),
                E1Retour = returnValues.Sum(x => x.E1Retour),
                E2 = returnValues.Sum(x => x.E2),
                E2Retour = returnValues.Sum(x => x.E2Retour),
                EleRef = returnValues.Sum(x => x.EleRef),
                ERetourTotal = returnValues.Sum(x => x.ERetourTotal),
                ETotal = returnValues.Sum(x => x.ETotal),
                Gas = returnValues.Sum(x => x.Gas),
                GasRef = returnValues.Sum(x => x.GasRef)
            });

            return returnValues;
        }

        [HttpGet]
        public IEnumerable<RecentData> Recent(string key) {
            IEnumerable<RecentData> vals;
            using (var conn = new SqlConnection(connectionString)) {
                conn.Open();
                vals = conn.Query<RecentData>("SELECT TOP 20 [Timestamp], CurrentUsage FROM LogEntries INNER JOIN Users ON Users.UserId = LogEntries.UserId WHERE ApiKey = @Key ORDER BY [Timestamp] DESC", new { Key = key }, commandType: CommandType.Text);
            }
            return vals.Reverse();
        }

        [HttpGet]
        public EstimatedUsage Estimated(string key) {
            var startdate = new DateTime(DateTime.Today.Year, 1, 1);
            var enddate = new DateTime(DateTime.Today.Year, 12, 31);
            using (var conn = new SqlConnection(connectionString)) {
                conn.Open();
                return conn.Query<EstimatedUsage>("GetTotalUsage", new { Key = key, StartDate = startdate, EndDate = enddate, Year = DateTime.Today.Year }, commandType: CommandType.StoredProcedure).FirstOrDefault();
            }
        }
    }


    public class HourlyUsage {
        public int Hour { get; set; }
        public decimal E1 { get; set; }
        public decimal E2 { get; set; }
        public decimal ETotal { get; set; }
        public decimal E1Retour { get; set; }
        public decimal E2Retour { get; set; }
        public decimal ERetourTotal { get; set; }
        public decimal Gas { get; set; }
    }

    public class DailyUsage {
        public DateTime Day { get; set; }
        public string DayString { get { return Day.ToString("ddd dd MMM"); } }
        public decimal E1 { get; set; }
        public decimal E2 { get; set; }
        public decimal ETotal { get; set; }
        public decimal E1Retour { get; set; }
        public decimal E2Retour { get; set; }
        public decimal ERetourTotal { get; set; }
        public decimal Gas { get; set; }
        public decimal EleRef { get; set; }
        public decimal GasRef { get; set; }
        public decimal EleRefDiff {
            get {
                return ETotal - EleRef;
            }
        }
        public decimal GasRefDiff {
            get {
                return Gas - GasRef;
            }
        }
        public int EleRefPerc {
            get {
                if (EleRef == 0)
                    return 100;

                return Convert.ToInt32((ETotal / EleRef) * 100);
            }
        }
        public int GasRefPerc {
            get {
                if (GasRef == 0)
                    return 100;

                return Convert.ToInt32((Gas / GasRef) * 100);
            }
        }
    }

    public class WeeklyUsage {
        public int Week { get; set; }
        public decimal E1 { get; set; }
        public decimal E2 { get; set; }
        public decimal ETotal { get; set; }
        public decimal E1Retour { get; set; }
        public decimal E2Retour { get; set; }
        public decimal ERetourTotal { get; set; }
        public decimal Gas { get; set; }
        public decimal EleRef { get; set; }
        public decimal GasRef { get; set; }
        public decimal EleRefDiff {
            get {
                return ETotal - EleRef;
            }
        }
        public decimal GasRefDiff {
            get {
                return Gas - GasRef;
            }
        }
        public int EleRefPerc {
            get {
                if (EleRef == 0)
                    return 100;

                return Convert.ToInt32((ETotal / EleRef) * 100);
            }
        }
        public int GasRefPerc {
            get {
                if (GasRef == 0)
                    return 100;

                return Convert.ToInt32((Gas / GasRef) * 100);
            }
        }
    }

    public class MonthlyUsage {
        public int Month { get; set; }
        public string MonthName { get; set; }
        public decimal E1 { get; set; }
        public decimal E2 { get; set; }
        public decimal ETotal { get; set; }
        public decimal E1Retour { get; set; }
        public decimal E2Retour { get; set; }
        public decimal ERetourTotal { get; set; }
        public decimal Gas { get; set; }
        public decimal EleRef { get; set; }
        public decimal GasRef { get; set; }
        public decimal EleRefDiff {
            get {
                return ETotal - EleRef;
            }
        }
        public decimal GasRefDiff {
            get {
                return Gas - GasRef;
            }
        }
        public int EleRefPerc {
            get {
                if (EleRef == 0)
                    return 100;

                return Convert.ToInt32((ETotal / EleRef) * 100);
            }
        }
        public int GasRefPerc {
            get {
                if (GasRef == 0)
                    return 100;

                return Convert.ToInt32((Gas / GasRef) * 100);
            }
        }
    }

    public class RecentData {
        public DateTime Timestamp { get; set; }
        public decimal CurrentUsage { get; set; }
    }

    public class EstimatedUsage {
        public int NumberOfDays { get; set; }
        public decimal E1Meter { get; set; }
        public decimal E2Meter { get; set; }
        public decimal E1 { get; set; }
        public decimal E2 { get; set; }
        public decimal ETotal { get; set; }
        public decimal E1Retour { get; set; }
        public decimal E2Retour { get; set; }
        public decimal ERetourTotal { get; set; }
        public decimal GasMeter { get; set; }
        public decimal Gas { get; set; }
        public decimal EleRef { get; set; }
        public decimal GasRef { get; set; }
        public decimal ERefYear { get; set; }
        public decimal GasRefYear { get; set; }
        public decimal EPercentage { get; set; }
        public decimal EEstimated { get; set; }
        public decimal GasPercentage { get; set; }
        public decimal GasEstimated { get; set; }
    }
}
