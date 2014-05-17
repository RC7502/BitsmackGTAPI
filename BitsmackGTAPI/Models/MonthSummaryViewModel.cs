using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BitsmackGTAPI.Models
{
    public class MonthSummaryViewModel
    {
        public string Name;
        public int Reverse;
        public int BestMonth;
        public int MonthlyAverage;
        public int CurrentMonth;
        public int Expected;
        public int GoalPerDay;
        public int CurrentDay;
        public int Remaining;
    }
}