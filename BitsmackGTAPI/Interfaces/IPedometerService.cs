using System;
using System.Collections.Generic;
using BitsmackGTAPI.Models;

namespace BitsmackGTAPI.Interfaces
{
    public interface IPedometerService
    {
        PedometerSummaryViewModel GetSummary();
        PedometerDetailViewModel GetDetail(DateTime start, DateTime end);
        ColumnChartModel GetMonthAverages();
        IEnumerable<MonthSummaryViewModel> GetMonthSummary();
        void RefreshData(bool b, DateTime addDays, DateTime date);
        void SetFitbitNewGoal(int goalPerDay);
    }
}