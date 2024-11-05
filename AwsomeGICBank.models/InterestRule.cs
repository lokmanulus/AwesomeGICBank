using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AwsomeGICBank.Models
{
    public class InterestRule
    {
        public DateTime Date { get; set; }
        public string RuleId { get; set; }
        public decimal Rate { get; set; } // Stored as a percentage (e.g., 2.20 for 2.20%)
    }

}
