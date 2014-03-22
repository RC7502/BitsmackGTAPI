using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Web;
using BitsmackGTAPI.Constants;
using BitsmackGTAPI.Helpers;
using BitsmackGTAPI.Interfaces;
using HabitRPG.NET;
using HabitRPG.NET.Models;
using Newtonsoft.Json;
using TogglApi.Models;

namespace BitsmackGTAPI.Models
{
    public class GoalService : IGoalService
    {
        private readonly IDAL _dal;
        private readonly ICommonService _commonService;
        private readonly IHabitDAL _habitDAL;

        public GoalService(IDAL dal, ICommonService commonService, IHabitDAL habitDAL)
        {
            _dal = dal;
            _commonService = commonService;
            _habitDAL = habitDAL;
        }

        public GoalsSummaryViewModel GetSummary()
        {
            var model = new GoalsSummaryViewModel();
            //RefreshToggl(false, DateTime.Now.Date.AddDays(-14), DateTime.Now.Date);
            //model.Items.Add(CalcStandingDeskGoal());
            //model.Items.Add(CalcWeightGoal());
            
            return model;
        }

        public IEnumerable<WeightCalDetailModel> GetWeightCalDetail()
        {
            var list = new List<WeightCalDetailModel>();
            var pedoRecs = _dal.GetPedometerRecords().OrderBy(x => x.trandate).ToList();
            decimal trendAvg = 0;
            for (var i = 0; i < pedoRecs.Count; i++)
            {
                var rec = pedoRecs[i];
                if (i == 0)
                {
                    trendAvg = (decimal) rec.weight;
                }
                else
                {
                    if (rec.weight > 0)
                        trendAvg = trendAvg + (0.1m*(decimal) (rec.weight - (double) trendAvg));
                }

                list.Add(new WeightCalDetailModel
                    {
                        TranDate = rec.trandate,
                        Weight = Math.Round(rec.weight, 1),
                        Trend = Math.Round(trendAvg, 1),
                        CalConsumed = rec.calconsumed ?? 0
                    });

            }

            return list;
        }

        public WeightCalSummaryModel GetWeightCalSummary()
        {
            var model = new WeightCalSummaryModel();
            var detail = GetWeightCalDetail().ToList();
            var firstRec = detail.OrderBy(x => x.TranDate).First(x => x.CalConsumed > 0);
            var lastRec = detail.OrderBy(x => x.TranDate).Last();
            var newDetail = detail.Where(x => x.TranDate >= firstRec.TranDate).ToList();

            var numDays = newDetail.Count()-1;
            model.DateRange = string.Format("{0} - {1} ({2})", firstRec.TranDate.ToShortDateString(),
                                            lastRec.TranDate.ToShortDateString(), numDays);
            var weightLoss = firstRec.Trend - lastRec.Trend;
            model.WeightTrendChange = weightLoss + " lbs.";
            var totalCal = newDetail.Sum(x => x.CalConsumed) - lastRec.CalConsumed;
            model.CalConsumedPerDay = (totalCal/numDays).ToString();
            var totalBurned = (weightLoss*3500) + totalCal;
            var burnPerDay = (int)totalBurned/numDays;
            model.CalBurnedPerDay = burnPerDay.ToString();
            model.NewDailyCalorieGoal = (burnPerDay - 500).ToString();

            return model;
        }

        public List<HabitDetailViewModel> GetHabitDetail()
        {
            RefreshHabitRPG();
            return _habitDAL.TaskHistoryView();
        }

        private void RefreshHabitRPG()
        {
            try
            {
                var key = _commonService.GetAPIKeyByName(APINames.HABITRPG);
                if (key == null || ((DateTime.UtcNow - key.last_update).TotalMinutes < 60)) return;
                var list = GetHabitRPGData(key);
                foreach (var task in list.Tasks)
                {
                    var existingTask = _habitDAL.GetHabitTasks().FirstOrDefault(x => x.id == task.Id);
                    if (existingTask != null)
                    {
                        CopyHabitTask(task, existingTask);
                        _habitDAL.Update(existingTask);
                    }
                    else
                    {
                        var newTask = new HabitTasks
                            {
                                attribute = task.Attribute,
                                dateCreated = task.DateCreated,
                                down = (short?) (task.Down ? 1 : 0),
                                up = (short?) (task.Up ? 1 : 0),
                                id = task.Id,
                                notes = task.Notes,
                                priority = task.Priority,
                                text = task.Text,
                                type = task.Type,
                                value = task.Value
                            };
                        _habitDAL.Insert(newTask);
                    }
                    var existingHistory = _habitDAL.GetHabitTaskHistory().Where(x => x.taskID == task.Id).ToList();
                    if (task.History != null)
                    {
                        foreach (var hist in task.History)
                        {
                            var histDate = TimeHelper.MillisecondsToDate(hist.Date);
                            if (existingHistory.All(x => x.historyDate != histDate))
                            {
                                var newHist = new HabitTaskHistory
                                    {
                                        taskID = task.Id,
                                        historyDate = histDate,
                                        value = hist.Value
                                    };
                                _habitDAL.Insert(newHist);

                            }
                        }
                    }

                    //merge new and old tags
                }
                _habitDAL.Save();



                key.last_update = DateTime.UtcNow;
                _commonService.UpdateAPIKey(key);
            }
            catch (DbEntityValidationException dbEx)
            {
                foreach (var validationErrors in dbEx.EntityValidationErrors)
                {
                    foreach (var validationError in validationErrors.ValidationErrors)
                    {
                        _commonService.WriteLog(EventLogSeverity.Error, MethodBase.GetCurrentMethod().Name, validationError.ErrorMessage);
                    }
                }
            }
        }

        private void CopyHabitTask(Task from, HabitTasks to)
        {
            to.attribute = from.Attribute;
            to.dateCreated = from.DateCreated;
            to.down = (short?) (from.Down ? 1 : 0);
            to.up = (short?) (from.Up ? 1 : 0);
            to.notes = from.Notes;
            to.priority = from.Priority;
            to.text = from.Text;
            to.type = from.Type;
            to.value = from.Value;
        }

        private HabitRPGDataModel GetHabitRPGData(APIKeys key)
        {
            var model = new HabitRPGDataModel();
            try
            {
                var client = new HabitRPGClient("https://habitrpg.com/api/v2", key.user_secret, key.user_token);
                model.Tasks = client.GetTasks();

            }
            catch (Exception ex)
            {
                //_commonService
                _commonService.WriteLog(EventLogSeverity.Error, MethodBase.GetCurrentMethod().Name, ex.Message);

            }


            return model;
        }

        private GoalSummaryViewModel CalcWeightGoal()
        {
            var recs = _dal.GetPedometerRecords().OrderBy(x => x.trandate).Select(x=>(decimal)x.weight).ToList();
            var model = new GoalSummaryViewModel
                {
                    Name = "Weight",
                    AvgValue = Math.Round(recs.Where(x => x > 0).Average(), 1),
                    TrendAvg = MathHelper.TrendAverage(recs, 1)
                };
            model.NewGoalValue = Math.Round(Math.Max(model.AvgValue, model.TrendAvg) - 0.2m, 1);
            return model;
        }

        private GoalSummaryViewModel CalcStandingDeskGoal()
        {
            var recs = _dal.GetTimedActivityRecords().Where(x => x.description == "Standing Desk").OrderBy(x=>x.startdate);
            var firstRec = recs.FirstOrDefault();
                 
            var model = new GoalSummaryViewModel()
                {
                    Name = "Standing Desk"
                };
            if (firstRec != null)
            {
                var durationPerDay = new Dictionary<DateTime, int>();
                for (var counterDay = firstRec.startdate.Date;
                     counterDay < DateTime.UtcNow.Date;
                     counterDay = counterDay.AddDays(1))
                {
                    if (counterDay.DayOfWeek == DayOfWeek.Saturday || counterDay.DayOfWeek == DayOfWeek.Sunday)
                        continue;
                    var sumDuration = recs.Where(x => x.startdate.Date == counterDay.Date).Sum(x => x.duration);
                    durationPerDay.Add(counterDay.Date, sumDuration);
                }

                model.AvgValue = new decimal(recs.Sum(x => x.duration) /
                                                 TimeHelper.GetBusinessDays(firstRec.startdate, DateTime.UtcNow.AddDays(-1)));
                model.TrendAvg = MathHelper.TrendAverage(durationPerDay.Select(x => x.Value).ToList(), true);
                model.NewGoalValue = Math.Max(model.AvgValue, model.TrendAvg)*1.01m;
            }

            return model;
        }

        private void RefreshToggl(bool overwrite, DateTime start, DateTime end)
        {
            try
            {
                var key = _commonService.GetAPIKeyByName(APINames.TOGGL);
                if (key == null || ((DateTime.UtcNow - key.last_update).TotalMinutes < 60)) return;
                var list = GetTogglData(key, start, end);
                var existingRecs = _dal.GetTimedActivityRecords().ToList();
                foreach (var entry in list)
                {
                    var existingAct = existingRecs.FirstOrDefault(x => x.description == entry.description
                                                                       && x.duration == entry.duration
                                                                       && x.startdate.Date == DateTime.Parse(entry.start).Date);
                    if (existingAct != null) continue;
                    var newRec = new TimedActivities()
                        {
                            description = entry.description,
                            duration = entry.duration,
                            startdate = DateTime.Parse(entry.start),
                            enddate = DateTime.Parse(entry.stop)
                        };
                    _dal.Insert(newRec);
                    _commonService.LogActivity(newRec);
                }

                _dal.SaveTimedActivities();
                key.last_update = DateTime.UtcNow;
                _commonService.UpdateAPIKey(key);
            }
            catch (Exception ex)
            {
                _commonService.WriteLog(EventLogSeverity.Error, MethodBase.GetCurrentMethod().Name, ex.Message);
            }

        }

        private IEnumerable<TimeEntry> GetTogglData(APIKeys key, DateTime start, DateTime end)
        {
            try
            {
                var auth = new TogglAuthRequest() {ApiToken = key.user_token};
                var api = new TogglApi.TogglApi(auth);
                var entries = api.Users.GetAllTimeEntries(start, end);
                return entries;
            }
            catch (Exception ex)
            {
                _commonService.WriteLog(EventLogSeverity.Error, MethodBase.GetCurrentMethod().Name, ex.Message);
            }
            return new List<TimeEntry>();
        }
    }
}