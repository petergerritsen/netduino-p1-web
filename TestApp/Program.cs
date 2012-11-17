using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Core.Persistence;
using Core.Model;
using System.Diagnostics;

namespace TestApp {
    class Program {
        static void Main(string[] args) {
            ILoggingRepository repo = Core.Factory.GetILoggingRepository();

            var dateTimeStart = new DateTime(2011, 10, 4, 0, 3, 12);
            var user = repo.GetUserByApiKey("bWFpbEBwZXRlcmdlcnJpdHNlbi5ubA");

            Random rand = new Random();

            var e1Offset = rand.NextDouble();
            var e2Offset = rand.NextDouble();
            var gasOffset = rand.NextDouble();

            for(int i = 0; i < 60000; i++){
                var logEntry = new LogEntry();
                logEntry.Timestamp = dateTimeStart.AddMinutes(10 * i);
                logEntry.UserId = user.UserId;
                logEntry.E1 = Convert.ToDecimal(e1Offset);
                logEntry.E2 = Convert.ToDecimal(e2Offset);
                logEntry.GasMeasurementMoment = dateTimeStart;
                logEntry.GasMeasurementValue = Convert.ToDecimal(gasOffset);

                repo.AddEntry(logEntry);
                
                e1Offset += rand.NextDouble();
                e2Offset += rand.NextDouble();
                gasOffset += rand.NextDouble();

                if (i % 600 == 0)
                    Debug.WriteLine(string.Format("{0} %", i / 600));
            }            
        }
    }
}
