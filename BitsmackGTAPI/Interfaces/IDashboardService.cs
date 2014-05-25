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
        DashboardCategoryViewModel GetSteps();
        DashboardCategoryViewModel GetCalories();
    }
}