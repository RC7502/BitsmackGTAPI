using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BitsmackGTAPI.Models
{
    public class PedometerSummaryViewModel
    {
        public int NumOfDays;
        public int TrendSteps;
        public int NewStepGoal;
        public double NextUpdate;
        public int AvgSleep;
        public string SleepStartTime;
        public string SleepEndTime;
        public int AverageSteps { get; set; }
    }
}