using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Exporter.Db
{
    public class Usage {
        public int UsageId { get; set; }
        public DateTime Timestamp { get; set; }

        public decimal E1Start { get; set; }
        public decimal E1Current { get; set; }

        public decimal E2Start { get; set; }
        public decimal E2Current { get; set; }

        public decimal E1RetourStart { get; set; }
        public decimal E1RetourCurrent { get; set; }

        public decimal E2RetourStart { get; set; }
        public decimal E2RetourCurrent { get; set; }

        public decimal GasStart { get; set; }
        public decimal GasCurrent { get; set; }

        public int UserId { get; set; }
    }

    public class Reference {
        public int ReferenceId { get; set; }
        public int UserId { get; set; }
        public DateTime Date { get; set; }
        public decimal Electricity { get; set; }
        public decimal Gas { get; set; }
    }
}
