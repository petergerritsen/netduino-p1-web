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

        public decimal E2Start { get; set; }
        public decimal E2Current { get; set; }        

        public decimal E1RetourStart { get; set; }
        public decimal E1RetourCurrent { get; set; }        

        public decimal E2RetourStart { get; set; }
        public decimal E2RetourCurrent { get; set; }        

        public decimal GasStart { get; set; }
        public decimal GasCurrent { get; set; }
        
        public int UserId { get; set; }
        [ForeignKey("UserId")]
        public virtual User User { get; set; }             

        #region Extra Properties

        [NotMapped]
        public decimal E1Usage {
            get {
                return E1Current - E1Start;
            }
        }

        [NotMapped]
        public decimal E2Usage {
            get {
                return E2Current - E2Start;
            }
        }

        [NotMapped]
        public decimal EUsageTotal {
            get {
                return E1Usage + E2Usage;
            }
        }

        [NotMapped]
        public decimal E1Retour {
            get {
                return E1RetourCurrent - E1RetourStart;
            }
        }

        [NotMapped]
        public decimal E2Retour {
            get {
                return E2RetourCurrent - E2RetourStart;
            }
        }

        [NotMapped]
        public decimal ERetourTotal {
            get {
                return E1Retour + E2Retour;
            }
        }

        [NotMapped]
        public decimal GasUsage {
            get {
                return GasCurrent - GasStart;
            }
        }

        [NotMapped]
        public bool IsNew {
            get { return UsageId == 0; }
        }

        #endregion
    }

    public enum UsageType {
        Hourly = 1,
        Daily = 2,
        Weekly = 3,
        Monthly = 4
    }
}
