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
            var date = DateTime.Today.AddDays(DayOfWeek.Monday - DateTime.Today.DayOfWeek).AddDays(-7 * offset);
            using (var conn = new SqlConnection(connectionString)) {
                conn.Open();
                return conn.Query<DailyUsage>("GetDailyUsage", new { Key = key, StartDate = date, EndDate = date.AddDays(7) }, commandType: CommandType.StoredProcedure);
            }
        }

        [HttpGet]
        public IEnumerable<WeeklyUsage> Weekly(string key, int offset, int count) {
            // first day of current week
            var date = DateTime.Today.AddDays(DayOfWeek.Monday - DateTime.Today.DayOfWeek);
            DateTimeFormatInfo dfi = new CultureInfo("nl-NL").DateTimeFormat;
            var week = dfi.Calendar.GetWeekOfYear(date, dfi.CalendarWeekRule, dfi.FirstDayOfWeek);
            
            using (var conn = new SqlConnection(connectionString)) {
                conn.Open();
                return conn.Query<WeeklyUsage>("GetWeeklyUsage", new { Key = key, StartWeek = week - offset, EndWeek = week - offset + count, Year = date.AddDays(-1 * offset).Year }, commandType: CommandType.StoredProcedure);
            }
        }

        [HttpGet]
        public IEnumerable<MonthlyUsage> Monthly(string key, int offset) {
            using (var conn = new SqlConnection(connectionString)) {
                conn.Open();
                return conn.Query<MonthlyUsage>("GetMonthlyUsage", new { Key = key, Year = DateTime.Today.Year - offset }, commandType: CommandType.StoredProcedure);
            }
        }

        [HttpGet]
        public IEnumerable<RecentData> Recent(string key) {
            using (var conn = new SqlConnection(connectionString)) {
                conn.Open();
                return conn.Query<RecentData>("SELECT TOP 100 [Timestamp], CurrentUsage As Usage, CurrentRetour As Retour FROM LogEntries INNER JOIN Users ON Users.UserId = LogEntries.UserId WHERE ApiKey = @Key ORDER BY [Timestamp] DESC", new { Key = key }, commandType: CommandType.Text);
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
        public decimal E1 { get; set; }
        public decimal E2 { get; set; }
        public decimal ETotal { get; set; }
        public decimal E1Retour { get; set; }
        public decimal E2Retour { get; set; }
        public decimal ERetourTotal { get; set; }
        public decimal Gas { get; set; }
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
    }

    public class MonthlyUsage {
        public int Month { get; set; }
        public decimal E1 { get; set; }
        public decimal E2 { get; set; }
        public decimal ETotal { get; set; }
        public decimal E1Retour { get; set; }
        public decimal E2Retour { get; set; }
        public decimal ERetourTotal { get; set; }
        public decimal Gas { get; set; }
    }

    public class RecentData {
        public DateTime Timestamp { get; set; }
        public decimal Usage { get; set; }
        public decimal Retour { get; set; }
    }
}
