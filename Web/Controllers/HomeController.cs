using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Globalization;

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
    }
}
