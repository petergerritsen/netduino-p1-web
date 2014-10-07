using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Model
{
    public class LogEntry
    {
        public long LogEntryId { get; set; }        
        public DateTime Timestamp { get; set; }
        public decimal E1 { get; set; }
        public decimal E2 { get; set; }
        public decimal E1Retour { get; set; }
        public decimal E2Retour { get; set; }
        public int CurrentTariff { get; set; }
        public decimal CurrentUsage { get; set; }
        public decimal CurrentRetour { get; set; }
        public DateTime GasMeasurementMoment { get; set; }
        public decimal GasMeasurementValue { get; set; }
        public decimal PvCounter { get; set; }

        public int UserId { get; set; }
        [ForeignKey("UserId")]
        public virtual User User { get; set; }
    }
}
