using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Exporter {
    public class DailyUsage {
        public DateTime Date { get; set; }
        
        public int Year {
            get { return Date.Year; }
        }

        public int Month {
            get { return Date.Month; }
        }

        public int Week {
            get {
                CultureInfo ci = new CultureInfo(1043);
                return ci.Calendar.GetWeekOfYear(Date, ci.DateTimeFormat.CalendarWeekRule, DayOfWeek.Monday);
            } 
        }

        public Reference Reference { get; set; }

        public HourlyStanding[] Standings { get; set; }
    }

    public class Reference {
        public decimal Gas { get; set; }
        public decimal Electricity { get; set; }
    }

    public class HourlyStanding {
        public int Hour { get; set; }
        public decimal E1 { get; set; }
        public decimal E2 { get; set; }
        public decimal E1Retour { get; set; }
        public decimal E2Retour { get; set; }
        public decimal PvProduction { get; set; }
        public decimal Gas { get; set; }
    }
}
