﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Core.Helper;
using Core.Model;

namespace Core.Persistence {
    public class LoggingRepository : ILoggingRepository {
        Context context = new Context();

        public IEnumerable<Model.LogEntry> GetEntries() {
            return context.LogEntries.OrderByDescending(x => x.LogEntryId).Take(10).ToList();
        }

        public Model.LogEntry GetEntry(int id) {
            return context.LogEntries.Find(id);
        }

        public Model.LogEntry AddEntry(Model.LogEntry entry) {
            context.LogEntries.Add(entry);

            // Process Usage values
            var hourlyUsage = GetUsage(Model.UsageType.Hourly, entry);
            PopulateUsage(hourlyUsage, entry);
            var dailyUsage = GetUsage(Model.UsageType.Daily, entry);
            PopulateUsage(dailyUsage, entry);
            var weeklyUsage = GetUsage(Model.UsageType.Weekly, entry);
            PopulateUsage(weeklyUsage, entry);
            var monthlyUsage = GetUsage(Model.UsageType.Monthly, entry);
            PopulateUsage(monthlyUsage, entry);

            // Delete old logentries
            context.Database.ExecuteSqlCommand("DELETE FROM LogEntries WHERE TimeStamp < @DateOffset", new System.Data.SqlClient.SqlParameter("DateOffset", entry.Timestamp.AddDays(-1)));

            context.SaveChanges();

            // Process gas usage
            var hourlyGasUsage = GetGasUsage(Model.UsageType.Hourly, entry);
            var dailyGasUsage = GetGasUsage(Model.UsageType.Daily, entry);
            var weeklyGasUsage = GetGasUsage(Model.UsageType.Weekly, entry);
            var monthlyGasUsage = GetGasUsage(Model.UsageType.Monthly, entry);

            if (hourlyGasUsage != null) {
                hourlyGasUsage.GasCurrent = entry.GasMeasurementValue;
            }
            if (dailyGasUsage != null) {
                dailyGasUsage.GasCurrent = entry.GasMeasurementValue;
            }
            if (weeklyGasUsage != null) {
                weeklyGasUsage.GasCurrent = entry.GasMeasurementValue;
            }
            if (monthlyGasUsage != null) {
                monthlyGasUsage.GasCurrent = entry.GasMeasurementValue;
            }

            context.SaveChanges();

            return entry;
        }

        public Model.User GetUserById(int id) {
            return context.Users.Find(id);
        }

        public Model.User GetUserByApiKey(string apiKey) {
            return context.Users.FirstOrDefault(x => x.ApiKey == apiKey);
        }

        public Model.Usage GetUsage(Core.Model.UsageType type, LogEntry logEntry) {
            var baseTimestamp = GetUsageTimestamp(type, logEntry.Timestamp);

            var usage = context.Usages.FirstOrDefault(x => x.UsageType == (int)type && x.Timestamp == baseTimestamp);

            if (usage == null) {
                usage = new Model.Usage() { UsageType = (int)type, Timestamp = baseTimestamp, UserId = logEntry.UserId };

                var prevUsage = context.Usages.Where(x => x.UsageType == (int)type && x.Timestamp < baseTimestamp).OrderByDescending(x => x.Timestamp).FirstOrDefault();
                if (prevUsage != null) {
                    usage.E1Start = prevUsage.E1Current;
                    usage.E2Start = prevUsage.E2Current;
                    usage.E1RetourStart = prevUsage.E1RetourCurrent;
                    usage.E2RetourStart = prevUsage.E2RetourCurrent;
  
                } else {
                    usage.E1Start = logEntry.E1;
                    usage.E2Start = logEntry.E2;
                    usage.E1RetourStart = logEntry.E1Retour;
                    usage.E2RetourStart = logEntry.E2Retour;

                }

                context.Usages.Add(usage);
            }

            return usage;
        }

        private Model.Usage GetGasUsage(Core.Model.UsageType type, LogEntry logEntry) {
            var baseTimestamp = GetUsageTimestamp(type, logEntry.GasMeasurementMoment);

            var usage = context.Usages.FirstOrDefault(x => x.UsageType == (int)type && x.Timestamp == baseTimestamp);

            if (usage != null && usage.GasStart == 0) {
                var prevUsage = context.Usages.Where(x => x.UsageType == (int)type && x.UsageId < usage.UsageId).OrderByDescending(x => x.UsageId).FirstOrDefault();
                if (prevUsage != null)
                    usage.GasStart = prevUsage.GasCurrent;
                else
                    usage.GasStart = logEntry.GasMeasurementValue;
            }

            return usage;
        }

        private static DateTime GetUsageTimestamp(Core.Model.UsageType type, DateTime refTimestamp) {
            var baseTimestamp = DateTime.MinValue;
            if (type == Model.UsageType.Hourly) {
                baseTimestamp = new DateTime(refTimestamp.Year, refTimestamp.Month, refTimestamp.Day, refTimestamp.Hour, 0, 0);
            }
            if (type == Model.UsageType.Daily) {
                baseTimestamp = new DateTime(refTimestamp.Year, refTimestamp.Month, refTimestamp.Day, 0, 0, 0);
            }
            if (type == Model.UsageType.Weekly) {
                baseTimestamp = DateCalculations.GetFirstDayOfWeek(new DateTime(refTimestamp.Year, refTimestamp.Month, refTimestamp.Day, 0, 0, 0));
            }
            if (type == Model.UsageType.Monthly) {
                baseTimestamp = new DateTime(refTimestamp.Year, refTimestamp.Month, 1, 0, 0, 0);
            }
            return baseTimestamp;
        }

        private void PopulateUsage(Model.Usage usage, Model.LogEntry entry) {
            usage.E1Current = entry.E1;
            usage.E2Current = entry.E2;
            usage.E1RetourCurrent = entry.E1Retour;
            usage.E2RetourCurrent = entry.E2Retour;
        }
    }
}
