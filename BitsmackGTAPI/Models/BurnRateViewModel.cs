using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BitsmackGTAPI.Models
{
    public class BurnRateViewModel
    {
        public double CalBurnedPerMinute { get; set; }
        public int CalGoalPerDay { get; set; }
        public int CalAvailable { get; set; }
        public string TimeUntilGoalRate { get; set; }
    }
}