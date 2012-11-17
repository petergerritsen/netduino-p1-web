using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Model {
    public class Usage {
        public int UsageId { get; set; }
        public int UsageType { get; set; }
        public DateTime Timestamp { get; set; }

        public decimal E1Start { get; set; }
        public decimal E1Current { get; set; }
        [NotMapped]
        public decimal E1Usage {
            get {
                return E1Current - E1Start;
            }
        }

        public decimal E2Start { get; set; }
        public decimal E2Current { get; set; }
        [NotMapped]
        public decimal E2Usage {
            get {
                return E2Current - E2Start;
            }
        }

        public decimal E1RetourStart { get; set; }
        public decimal E1RetourCurrent { get; set; }
        [NotMapped]
        public decimal E1RetourUsage {
            get {
                return E1RetourCurrent - E1RetourStart;
            }
        }

        public decimal E2RetourStart { get; set; }
        public decimal E2RetourCurrent { get; set; }
        [NotMapped]
        public decimal E2RetourUsage {
            get {
                return E2RetourCurrent - E2RetourStart;
            }
        }

        public decimal GasStart { get; set; }
        public decimal GasCurrent { get; set; }
        [NotMapped]
        public decimal GasUsage {
            get {
                return GasCurrent - GasStart;
            }
        }

        public int UserId { get; set; }
        [ForeignKey("UserId")]
        public virtual User User { get; set; }

        [NotMapped]
        public bool IsNew {
            get { return UsageId == 0; }
        }
    }

    public enum UsageType {
        Hourly = 1,
        Daily = 2,
        Weekly = 3,
        Monthly = 4
    }
}
