using System;
using BitsmackGTAPI.Models;

namespace BitsmackGTAPI.Interfaces
{
    public interface IPedometerService
    {
        PedometerSummaryViewModel GetSummary();
        PedometerDetailViewModel GetDetail(DateTime start, DateTime end);
    }
}