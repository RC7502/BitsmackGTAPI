using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using BitsmackGTAPI.Constants;
using BitsmackGTAPI.Helpers;
using BitsmackGTAPI.Interfaces;
using TogglApi.Entities;

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

        public DashboardCategoryViewModel GetStepsOld()
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
                stepModel.Remaining = stepModel.MonthlyAverage - stepModel.CurrentMonth - stepModel.CurrentDay;
                model.Positive = 0;
            }
            else if (stepModel.CurrentMonth > stepModel.BestMonth)
            {
                stepModel.Remaining = 0;
                model.Positive = 1;
            }
            else
            {
                stepModel.Remaining = stepModel.BestMonth - stepModel.CurrentMonth - stepModel.CurrentDay;
                model.Positive = 1;
            }
            var dayGoal = Math.Max(3000, stepModel.Remaining/remainingDays);

            _pedometerService.SetFitbitNewGoal(dayGoal);
            var remaining = (dayGoal - stepModel.CurrentDay);

            model.ID = "steps";
            model.Title = string.Format("Steps - {0}", remaining);
            model.Texts.Add(string.Format("Day Goal {0}", dayGoal));
            model.Texts.Add(string.Format("Day Actual {0}", stepModel.CurrentDay));
            model.Texts.Add(string.Format("Day Remaining {0}", remaining));
            var currentMonthName = localNow.ToString("MMM", CultureInfo.InvariantCulture);

            model.Texts.Add(string.Format("Month Avg {0}", stepModel.MonthlyAverage));
            model.Texts.Add(string.Format("Month Best {0}", stepModel.BestMonth));
            model.Texts.Add(string.Format("{1} Actual {0}", stepModel.CurrentMonth, currentMonthName));
            model.Texts.Add(string.Format("{1} Remaining {0}", stepModel.Remaining, currentMonthName));

            return model;
        }

        public GoalMonthlyViewModel GetSteps()
        {
            var model = new GoalMonthlyViewModel();

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
                stepList.Add(pedometerRecs.Where(x => x.trandate.Year == monthCounter.Year && x.trandate.Month == monthCounter.Month).Sum(x => x.steps));
            }

            stepModel.Name = "Steps";
            stepModel.Reverse = 0;
            stepModel.BestMonth = stepList.Max();
            stepModel.MonthlyAverage = (int)stepList.Average();
            stepModel.CurrentMonth = pedometerRecs.Where(x => x.trandate.Year == currentMonth.Year && x.trandate.Month == currentMonth.Month).Sum(x => x.steps);
            stepModel.Expected = (int)(stepModel.MonthlyAverage * (Decimal.Divide(localNow.Day, dayInCurrentMonth)));
            stepModel.GoalPerDay = (stepModel.MonthlyAverage - stepModel.CurrentMonth) / remainingDays;
            stepModel.CurrentDay = todaysRecord != null ? todaysRecord.steps : 0;
            if (stepModel.Expected > stepModel.CurrentMonth)
            {
                stepModel.Remaining = stepModel.MonthlyAverage - stepModel.CurrentMonth - stepModel.CurrentDay;
            }
            else if (stepModel.CurrentMonth > stepModel.BestMonth)
            {
                stepModel.Remaining = 0;
            }
            else
            {
                stepModel.Remaining = stepModel.BestMonth - stepModel.CurrentMonth - stepModel.CurrentDay;
            }

            var dayGoal = Math.Max(3000, stepModel.Remaining / remainingDays);

            //_pedometerService.SetFitbitNewGoal(dayGoal);
            var remaining = (dayGoal - stepModel.CurrentDay);

            model.Name = "Steps";
            model.GoalPerDay = dayGoal.ToString();
            model.MonthlyAvg = stepModel.MonthlyAverage.ToString();
            model.BestMonth = stepModel.BestMonth.ToString();
            model.Expected = stepModel.Expected.ToString();
            model.Actual = stepModel.CurrentMonth.ToString();

            return model;
        }

        public DashboardCategoryViewModel GetCalories()
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
            var calModel = new MonthSummaryViewModel();
            var calList = new List<int>();
            for (var monthCounter = new DateTime(2014, 4, 1);
                 monthCounter < currentMonth;
                 monthCounter = monthCounter.AddMonths(1))
            {
                calList.Add(pedometerRecs.Where(x => x.trandate.Year == monthCounter.Year && x.trandate.Month == monthCounter.Month).Sum(x => x.calconsumed ?? 0));
            }

            calModel.Name = "Calories";
            calModel.Reverse = 1;
            calModel.BestMonth = calList.Min();
            calModel.MonthlyAverage = (int)calList.Average();
            calModel.CurrentMonth = pedometerRecs.Where(x => x.trandate.Year == currentMonth.Year && x.trandate.Month == currentMonth.Month).Sum(x => x.calconsumed ?? 0);
            calModel.Expected = (int)(calModel.MonthlyAverage * (Decimal.Divide(localNow.Day, dayInCurrentMonth)));
            calModel.GoalPerDay = (calModel.MonthlyAverage - calModel.CurrentMonth) / remainingDays;
            calModel.CurrentDay = todaysRecord != null && todaysRecord.calconsumed.HasValue ? todaysRecord.calconsumed.Value : 0;
            if (calModel.Expected < calModel.CurrentMonth)
            {
                calModel.Remaining = calModel.MonthlyAverage - calModel.CurrentMonth - calModel.CurrentDay;
                model.Positive = 0;
            }
            else
            {
                calModel.Remaining = calModel.BestMonth - calModel.CurrentMonth - calModel.CurrentDay;
                model.Positive = 1;
            }
            var dayGoal = Math.Max(1500, calModel.Remaining / remainingDays);

            var remaining = (dayGoal - calModel.CurrentDay);

            model.ID = "calories";
            model.Title = string.Format("Calories - {0}", remaining);
            model.Texts.Add(string.Format("Day Goal {0}", dayGoal));
            model.Texts.Add(string.Format("Day Actual {0}", calModel.CurrentDay));
            model.Texts.Add(string.Format("Day Remaining {0}", remaining));
            var currentMonthName = localNow.ToString("MMM", CultureInfo.InvariantCulture);

            model.Texts.Add(string.Format("Month Avg {0}", calModel.MonthlyAverage));
            model.Texts.Add(string.Format("Month Best {0}", calModel.BestMonth));
            model.Texts.Add(string.Format("{1} Actual {0}", calModel.CurrentMonth, currentMonthName));
            model.Texts.Add(string.Format("{1} Remaining {0}", calModel.Remaining, currentMonthName));

            return model;
        }

        public DashboardCategoryViewModel GetTodos()
        {
            var model = new DashboardCategoryViewModel();

            

            return model;
        }
    }
}