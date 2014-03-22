using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BitsmackGTAPI.Models
{
    public class WeightCalSummaryModel
    {
        public string DateRange { get; set; }
        public string WeightTrendChange { get; set; }
        public string CalConsumedPerDay { get; set; }
        public string CalBurnedPerDay { get; set; }
        public string NewDailyCalorieGoal { get; set; }
    }
}