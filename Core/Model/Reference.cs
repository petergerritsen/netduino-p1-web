using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Core.Model {
    public class Reference {
        public int ReferenceId {get;set;}
        public int UserId { get; set; }
        public DateTime Date { get; set; }
        public decimal Electricity { get; set; }
        public decimal Gas { get; set; }
    }
}
