using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Core.Persistence;
using Core.Model;
using System.Diagnostics;
using System.Threading;
using System.Net;
using System.IO;
using Core.Migrations;


namespace TestApp {
    class Program {
        static void Main(string[] args) {           

            //TestRepository(); 

            TestAppHarbor();
        }

        private static void TestAppHarbor() {
            var appharborurl = "http://netduinop1logging.apphb.com/api/logentries";

            var E1 = 100.240;
            
            for (int i = 0; i < 1000; i++) {
                E1 = E1 + 0.01;
                StringBuilder content = new StringBuilder();
                content.AppendLine("{");
                content.AppendLine("\"ApiKey\": \"sadsada232132131231\",");
                content.AppendLine("\"Timestamp\": \"" + DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss") + "\",");
                content.AppendLine(string.Format("\"E1\": \"{0}\",", E1.ToString()));
                content.AppendLine("\"E2\": \"0.456\",");
                content.AppendLine("\"E1Retour\": \"0\",");
                content.AppendLine("\"E2Retour\": \"0\",");
                content.AppendLine("\"CurrentTariff\": \"2\",");
                content.AppendLine("\"CurrentUsage\": \"0.10\",");
                content.AppendLine("\"CurrentRetour\": \"0\",");
                content.AppendLine(string.Format("\"GasMeasurementMoment\": \"{0}\",", DateTime.Now.AddHours(-1).ToString("yyMMddHH0000")));
                content.AppendLine("\"GasMeasurementValue\": \"0.123\"");
                content.AppendLine("}");

                byte[] bytes =  Encoding.UTF8.GetBytes(content.ToString());

                try {
                    WebRequest request = WebRequest.Create(appharborurl);
                    request.Timeout = 2000;
                    request.Method = "POST";
                    request.ContentType = "application/json";
                    request.ContentLength = bytes.Length;

                    Stream dataStream = request.GetRequestStream();
                    dataStream.Write(bytes, 0, bytes.Length);
                    dataStream.Close();
                    WebResponse response = request.GetResponse();

                    Console.WriteLine(string.Format("{0}: {1}", DateTime.Now.ToString("HH:mm:ss"), ((HttpWebResponse)response).StatusDescription));
                } catch (Exception) {
                    Console.WriteLine(string.Format("{0}: Request timed out", DateTime.Now.ToString("HH:mm:ss")));
                }

                Thread.Sleep(60000);
            }
        }

        private static void TestRepository() {


            ILoggingRepository repo = Core.Factory.GetILoggingRepository();

            var dateTimeStart = new DateTime(2011, 10, 4, 0, 3, 12);
            var user = repo.GetUserByApiKey("bWFpbEBwZXRlcmdlcnJpdHNlbi5ubA");

            Random rand = new Random();

            var e1Offset = rand.NextDouble();
            var e2Offset = rand.NextDouble();
            var gasOffset = rand.NextDouble();

            for (int i = 0; i < 100; i++) {
                var logEntry = new LogEntry();
                logEntry.Timestamp = dateTimeStart.AddMinutes(90 * i);
                logEntry.UserId = user.UserId;
                logEntry.E1 = Convert.ToDecimal(e1Offset);
                logEntry.E2 = Convert.ToDecimal(e2Offset);
                logEntry.GasMeasurementMoment = logEntry.Timestamp;
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
