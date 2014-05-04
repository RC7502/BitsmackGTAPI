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
    }
}