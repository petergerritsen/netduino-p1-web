using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Core.Model;
using Core.Persistence;
using System.Diagnostics;
using System.Globalization;
using System.Web.Management;
using Microsoft.AspNet.SignalR;

namespace Web.Controllers {
    public class LogEntriesController : ApiController {
        ILoggingRepository repo;

        public LogEntriesController() {
            repo = Core.Factory.GetILoggingRepository();
        }

        //// POST api/logentries
        public void Post([FromBody]PostEntry value) {
            
            if (ModelState.IsValid) {
                try {
                    var user = repo.GetUserByApiKey(value.ApiKey);
                    if (user == null)
                        throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.NotFound) { Content = new StringContent(string.Format("Invalid API Key: {0}", value.ApiKey)) });

                    DateTime gasMeasurementMoment = value.Timestamp;

                    var hubContext = GlobalHost.ConnectionManager.GetHubContext<UsageHub>();
                    hubContext.Clients.Group(value.ApiKey).newCurrentUsage(value.Timestamp, value.CurrentUsage, value.CurrentRetour);

                    if (!DateTime.TryParseExact("20" + value.GasMeasurementMoment, "yyyyMMddHHmmss", CultureInfo.InvariantCulture, DateTimeStyles.None, out gasMeasurementMoment))
                        gasMeasurementMoment = value.Timestamp;


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
                        PvCounter = Convert.ToDecimal(value.PvProductionCounter) / 1000,
                        UserId = user.UserId
                    };

                    repo.AddEntry(logEntry);

                    return;
                } catch (Exception ex) {
                    repo = Core.Factory.ResetILoggingRepository();

                    new LogEvent(ex.Message).Raise();

                    throw new HttpResponseException(HttpStatusCode.InternalServerError);
                }
            } else {
                throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.BadRequest) { Content = new StringContent("Invalid data") });
            }
        }

        //public void ResetRepository() {
        //    repo = Core.Factory.ResetILoggingRepository();
        //    return;
        //}
    }

    public class LogEvent : WebRequestErrorEvent {
        public LogEvent(string message)
            : base(null, null, 100001, new Exception(message)) {
        }
    }

    public class PostEntry {
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
        public int PvProductionCounter { get; set; }
    }
}