﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Core.Model;
using Core.Persistence;
using System.Diagnostics;
using System.Globalization;

namespace Web.Controllers
{
    public class LogEntriesController : ApiController
    {
        ILoggingRepository repo = Core.Factory.GetILoggingRepository();

        // GET api/logentries
        public IEnumerable<LogEntry> GetAll()
        {
            return repo.GetEntries();
        }

        // GET api/logentries/5
        public LogEntry Get(int id)
        {
            return repo.GetEntry(id);
        }

        //// POST api/logentries
        public LogEntry Post([FromBody]PostEntry value) {
            if (ModelState.IsValid) {
                var user = repo.GetUserByApiKey(value.ApiKey);
                if (user == null)
                    throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.NotFound) { Content = new StringContent(string.Format("Invalid API Key: {0}", value.ApiKey)) });

                DateTime gasMeasurementMoment = value.Timestamp;
                DateTime.TryParseExact("20" + value.GasMeasurementMoment, "yyyyMMddHHmmss", CultureInfo.InvariantCulture, DateTimeStyles.None, out gasMeasurementMoment);

                var logEntry = new LogEntry() {
                    Timestamp = value.Timestamp,
                    E1 = value.E1,
                    E2 = value.E2,
                    E1Retour = value.E1Retour,
                    E2Retour = value.E2Retour,
                    CurrentTariff = value.CurrentTariff,
                    CurrentUsage = value.CurrentUsage,
                    CurrentRetour = value.CurrentRetour,
                    GasMeasurementMoment = gasMeasurementMoment,
                    GasMeasurementValue = value.GasMeasurementValue,
                    User = user
                };

                return repo.AddEntry(logEntry);
            } else {
                throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.BadRequest) { Content = new StringContent("Invalid data") });
            }
        }        
    }   

    public class PostEntry
    {
        public string ApiKey { get; set; }
        public DateTime Timestamp { get; set; }
        public decimal E1 { get; set; }
        public decimal E2 { get; set; }
        public decimal E1Retour { get; set; }
        public decimal E2Retour { get; set; }
        public int CurrentTariff { get; set; }
        public decimal CurrentUsage { get; set; }
        public decimal CurrentRetour { get; set; }
        public string GasMeasurementMoment { get; set; }
        public decimal GasMeasurementValue { get; set; }
    }
}