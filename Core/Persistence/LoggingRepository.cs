using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Core.Helper;
using Core.Model;

namespace Core.Persistence {
    public class LoggingRepository : ILoggingRepository {
        Context context = new Context();

        List<User> users;
        DateTime lastCleanupRun = DateTime.MinValue;

        public IEnumerable<Model.LogEntry> GetEntries() {
            return context.LogEntries.OrderByDescending(x => x.LogEntryId).Take(10).ToList();
        }

        public Model.LogEntry GetEntry(int id) {
            return context.LogEntries.Find(id);
        }

        public Model.LogEntry AddEntry(Model.LogEntry entry) {
            context.LogEntries.Add(entry);

            if (lastCleanupRun.AddHours(3) < System.DateTime.Now) {
                // Delete old logentries
                context.Database.ExecuteSqlCommand("DELETE FROM LogEntries WHERE TimeStamp < @DateOffset", new System.Data.SqlClient.SqlParameter("DateOffset", entry.Timestamp.AddDays(-1)));

                lastCleanupRun = System.DateTime.Now;
            }

            context.SaveChanges();

            ProcessUsages(entry);

            return entry;
        }

        private void ProcessUsages(Model.LogEntry entry) {
            // Process Usage values
            var hourlyUsage = GetUsage(entry);
            PopulateUsage(hourlyUsage, entry);

            context.SaveChanges();

            // Process gas usage
            var hourlyGasUsage = GetGasUsage(entry);

            if (hourlyGasUsage != null) {
                hourlyGasUsage.GasCurrent = entry.GasMeasurementValue;
            }

            context.SaveChanges();
        }

        public Model.User GetUserById(int id) {
            if (users == null)
                users = context.Users.ToList();

            return users.FirstOrDefault(x => x.UserId == id);
        }

        public Model.User GetUserByApiKey(string apiKey) {
            if (users == null)
                users = context.Users.ToList();

            return users.FirstOrDefault(x => x.ApiKey == apiKey);
        }

        Dictionary<int, Usage> lastHourlyUsage = new Dictionary<int, Usage>();

        public Model.Usage GetUsage(LogEntry logEntry) {
            var baseTimestamp = GetUsageTimestamp(logEntry.Timestamp);

            Usage usage = null;
            bool updateUsage = false;

            if (lastHourlyUsage.ContainsKey(logEntry.UserId) && lastHourlyUsage[logEntry.UserId].Timestamp == baseTimestamp) {
                usage = lastHourlyUsage[logEntry.UserId];
            }

            if (usage == null) {
                updateUsage = true;
                usage = context.Usages.FirstOrDefault(x => x.UserId == logEntry.UserId  && x.Timestamp == baseTimestamp);
            }

            if (usage == null) {
                usage = new Model.Usage() { Timestamp = baseTimestamp, UserId = logEntry.UserId };

                var prevUsage = context.Usages.Where(x => x.UserId == logEntry.UserId && x.Timestamp < baseTimestamp).OrderByDescending(x => x.Timestamp).FirstOrDefault();
                if (prevUsage != null) {
                    usage.E1Start = prevUsage.E1Current;
                    usage.E2Start = prevUsage.E2Current;
                    usage.E1RetourStart = prevUsage.E1RetourCurrent;
                    usage.E2RetourStart = prevUsage.E2RetourCurrent;
                    usage.PvProductionStart = prevUsage.PvProductionCurrent;
                } else {
                    usage.E1Start = logEntry.E1;
                    usage.E2Start = logEntry.E2;
                    usage.E1RetourStart = logEntry.E1Retour;
                    usage.E2RetourStart = logEntry.E2Retour;
                    usage.PvProductionStart = 0;
                }

                context.Usages.Add(usage);                
            }

            if (updateUsage) {
                lastHourlyUsage[logEntry.UserId] = usage;
            }

            return usage;
        }

        private Model.Usage GetGasUsage(LogEntry logEntry) {
            var baseTimestamp = GetUsageTimestamp(logEntry.GasMeasurementMoment).AddHours(-1);

            Usage usage = null;

            if (lastHourlyUsage.ContainsKey(logEntry.UserId) && lastHourlyUsage[logEntry.UserId].Timestamp == baseTimestamp) {
                usage = lastHourlyUsage[logEntry.UserId];
            }

            if (usage == null)
                usage = context.Usages.FirstOrDefault(x => x.UserId == logEntry.UserId && x.Timestamp == baseTimestamp);

            if (usage != null && usage.GasStart == 0) {
                var prevUsage = context.Usages.Where(x => x.UserId == logEntry.UserId && x.UsageId < usage.UsageId).OrderByDescending(x => x.UsageId).FirstOrDefault();
                if (prevUsage != null && prevUsage.GasCurrent > 0)
                    usage.GasStart = prevUsage.GasCurrent;
                else
                    usage.GasStart = context.Usages.Where(x => x.UserId == logEntry.UserId && x.UsageId < usage.UsageId).Max(x => x.GasCurrent);
            }

            return usage;
        }

        private static DateTime GetUsageTimestamp(DateTime refTimestamp) {
           return new DateTime(refTimestamp.Year, refTimestamp.Month, refTimestamp.Day, refTimestamp.Hour, 0, 0);            
        }

        private void PopulateUsage(Model.Usage usage, Model.LogEntry entry) {
            usage.E1Current = entry.E1;
            usage.E2Current = entry.E2;
            usage.E1RetourCurrent = entry.E1Retour;
            usage.E2RetourCurrent = entry.E2Retour;

            if (entry.PvCounter > 0 && entry.Timestamp.Hour >= 3) {
                var refDate = entry.Timestamp.Date;
                var minDailyProduction = context.Usages.Where(x => x.Timestamp >= refDate).Min(x => x.PvProductionStart);
                usage.PvProductionCurrent = minDailyProduction + entry.PvCounter;
            } else {
                usage.PvProductionCurrent = usage.PvProductionStart;
            }
        }
    }
}
