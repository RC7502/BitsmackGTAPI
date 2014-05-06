using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BitsmackGTAPI.Constants;
using BitsmackGTAPI.Interfaces;

namespace BitsmackGTAPI.Models
{
    public class DashboardService : IDashboardService
    {
        private readonly IPedometerService _pedometerService;

        public DashboardService(IPedometerService pedometerService)
        {
            _pedometerService = pedometerService;
        }


        public DashboardViewModel GetDashboard()
        {
            var model = new DashboardViewModel();

            //Pedometer
            var pedoCategory = new DashboardCategoryViewModel
                {
                    Name = "Pedometer"
                };
            var pedometerSummary = _pedometerService.GetMonthSummary().ToList();
            var steps = pedometerSummary.FirstOrDefault(x => x.Name == "Steps");
            if (steps != null)
            {
                pedoCategory.Items.Add(new DashboardItemViewModel
                    {
                        Text = steps.Name,
                        ItemType = "ProgressBar",
                        BarMin = 0,
                        BarMax = steps.GoalPerDay,
                        BarActual = steps.CurrentDay,
                        Status = steps.CurrentMonth >= steps.Expected ? "Positive" : "Negative"
                    });
            }
            var cals = pedometerSummary.FirstOrDefault(x => x.Name == "Calories");
            if (cals != null)
            {
                pedoCategory.Items.Add(new DashboardItemViewModel
                {
                    Text = cals.Name,
                    ItemType = "ProgressBar",
                    BarMin = 0,
                    BarMax = cals.GoalPerDay,
                    BarActual = cals.CurrentDay,
                    Status = cals.CurrentMonth <= cals.Expected ? "Positive" : "Negative"
                });
            }

            model.Categories.Add(pedoCategory);

            return model;
        }
    }
}