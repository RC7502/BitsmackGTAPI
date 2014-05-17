using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using BitsmackGTAPI.Constants;
using BitsmackGTAPI.Helpers;
using BitsmackGTAPI.Interfaces;

namespace BitsmackGTAPI.Models
{
    public class DashboardService : IDashboardService
    {
        private readonly IPedometerService _pedometerService;
        private readonly IDAL _dal;

        public DashboardService(IPedometerService pedometerService, IDAL dal)
        {
            _pedometerService = pedometerService;
            _dal = dal;
        }


        public DashboardViewModel GetDashboard()
        {
            var model = new DashboardViewModel();

            //Pedometer
            //var pedoCategory = new DashboardCategoryViewModel
            //    {
            //        Name = "Exercise and Diet"
            //    };
            //var pedometerSummary = _pedometerService.GetMonthSummary().ToList();
            //var steps = pedometerSummary.FirstOrDefault(x => x.Name == "Steps");
            //if (steps != null)
            //{
            //    pedoCategory.Items.Add(new DashboardItemViewModel
            //        {
            //            Text = steps.Name,
            //            ItemType = "ProgressBar",
            //            BarMin = 0,
            //            BarMax = steps.GoalPerDay,
            //            BarActual = steps.CurrentDay,
            //            Status = steps.CurrentMonth >= steps.Expected ? "Positive" : "Negative"
            //        });
            //}
            //var cals = pedometerSummary.FirstOrDefault(x => x.Name == "Calories");
            //if (cals != null)
            //{
            //    pedoCategory.Items.Add(new DashboardItemViewModel
            //    {
            //        Text = cals.Name,
            //        ItemType = "ProgressBar",
            //        BarMin = 0,
            //        BarMax = cals.GoalPerDay,
            //        BarActual = cals.CurrentDay,
            //        Status = cals.CurrentMonth <= cals.Expected ? "Positive" : "Negative"
            //    });
            //}

            //model.Categories.Add(pedoCategory);

            return model;
        }

        public DashboardCategoryViewModel GetSteps()
        {
            var model = new DashboardCategoryViewModel();

            var localNow = TimeHelper.ConvertUtcToLocal(DateTime.UtcNow);
            _pedometerService.RefreshData(true, localNow.Date.AddDays(-14), DateTime.Now);
            var pedometerRecs = _dal.GetPedometerRecords().OrderBy(x => x.trandate);
           
            var currentMonth = new DateTime(localNow.Year, localNow.Month, 1);
            var dayInCurrentMonth = DateTime.DaysInMonth(currentMonth.Year, currentMonth.Month);
            var remainingDays = dayInCurrentMonth - localNow.Day + 1;
            var todaysRecord = pedometerRecs.FirstOrDefault(x => x.trandate == localNow.Date);

            //pedometer 
            var stepModel = new MonthSummaryViewModel();
            var stepList = new List<int>();
            for (var monthCounter = new DateTime(2013, 3, 1);
                 monthCounter < currentMonth;
                 monthCounter = monthCounter.AddMonths(1))
            {
                stepList.Add(pedometerRecs.Where(x=>x.trandate.Year == monthCounter.Year && x.trandate.Month == monthCounter.Month).Sum(x=>x.steps));
            }

            stepModel.Name = "Steps";
            stepModel.Reverse = 0;
            stepModel.BestMonth = stepList.Max();
            stepModel.MonthlyAverage = (int) stepList.Average();
            stepModel.CurrentMonth = pedometerRecs.Where(x=>x.trandate.Year == currentMonth.Year && x.trandate.Month == currentMonth.Month).Sum(x=>x.steps);
            stepModel.Expected = (int) (stepModel.MonthlyAverage*(Decimal.Divide(localNow.Day,dayInCurrentMonth)));
            stepModel.GoalPerDay = (stepModel.MonthlyAverage - stepModel.CurrentMonth)/remainingDays;           
            stepModel.CurrentDay = todaysRecord != null ? todaysRecord.steps : 0;
            if (stepModel.Expected > stepModel.CurrentMonth)
            {
                stepModel.Remaining = stepModel.MonthlyAverage - stepModel.CurrentMonth;
            }
            else if (stepModel.CurrentMonth > stepModel.BestMonth)
            {
                stepModel.Remaining = 0;
            }
            else
            {
                stepModel.Remaining = stepModel.BestMonth - stepModel.CurrentMonth;
            }
            var dayGoal = Math.Max(3000, stepModel.Remaining/remainingDays);


            _pedometerService.SetFitbitNewGoal(dayGoal);

            model.ID = "steps";
            model.Title = "Steps";
            model.Text = string.Format("Goal {0} | ", dayGoal);
            model.Text += string.Format("Actual {0} | ", stepModel.CurrentDay);
            model.Text += string.Format("Remaining {0}", (dayGoal - stepModel.CurrentDay));


            var monthly = new DashboardItemViewModel
                {
                    Title = localNow.ToString("MMM", CultureInfo.InvariantCulture),
                    Text = string.Format("Avg {0} | ", stepModel.MonthlyAverage)
                };
            monthly.Text += string.Format("Best {0} | ",stepModel.BestMonth);
            monthly.Text += string.Format("Actual {0} | ",stepModel.CurrentMonth);
            monthly.Text += string.Format("Remaining {0}",stepModel.Remaining);
            model.Items.Add(monthly);

            return model;
        }
    }
}