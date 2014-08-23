using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BitsmackGTAPI.Models;

namespace BitsmackGTAPI.Interfaces
{
    public interface IDashboardService
    {
        DashboardViewModel GetDashboard();
        GoalMonthlyViewModel GetSteps();
        GoalMonthlyViewModel GetCalories();
        GoalMonthlyViewModel GetTodos();
        BurnRateViewModel GetBurnRate();
        double CalBurnedPerMinute(Pedometer startRec, Pedometer mostCurrent, int? calConsumed);
    }
}