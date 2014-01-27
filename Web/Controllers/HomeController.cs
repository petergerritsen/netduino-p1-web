using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Globalization;
using Microsoft.AspNet.SignalR;

namespace Web.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public ActionResult Dashboard(string key) {
            ViewData.Add("ApiKey", key);

            // Calculate current week            
            DateTimeFormatInfo dfi = new CultureInfo("nl-NL").DateTimeFormat;
            var week = dfi.Calendar.GetWeekOfYear(DateTime.Today, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday);
            ViewData.Add("CurrentWeek", week);

            return View();
        }

        [HttpGet]
        public ActionResult TestSignalR(string key) {
            Random rand = new Random();
            var hubContext = GlobalHost.ConnectionManager.GetHubContext<UsageHub>();
            hubContext.Clients.Group(key).newCurrentUsage(DateTime.Now, rand.NextDouble() * 8);

            return new HttpStatusCodeResult(200);
        }
    }
}
