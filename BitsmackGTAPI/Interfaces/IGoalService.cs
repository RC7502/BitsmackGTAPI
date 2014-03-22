using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BitsmackGTAPI.Models;

namespace BitsmackGTAPI.Interfaces
{
    public interface IGoalService
    {
        GoalsSummaryViewModel GetSummary();
        List<HabitDetailViewModel> GetHabitDetail();
        IEnumerable<WeightCalDetailModel> GetWeightCalDetail();
        WeightCalSummaryModel GetWeightCalSummary();
    }
}