using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Web;
using BitsmackGTAPI.Constants;
using BitsmackGTAPI.Helpers;
using BitsmackGTAPI.Interfaces;
using Newtonsoft.Json;
using TogglApi.Models;

namespace BitsmackGTAPI.Models
{
    public class GoalService : IGoalService
    {
        private readonly IDAL _dal;
        private readonly ICommonService _commonService;

        public GoalService(IDAL dal, ICommonService commonService)
        {
            _dal = dal;
            _commonService = commonService;
        }

        public GoalsSummaryViewModel GetSummary()
        {
            var model = new GoalsSummaryViewModel();
            //RefreshToggl(false, DateTime.Now.Date.AddDays(-14), DateTime.Now.Date);
            //model.Items.Add(CalcStandingDeskGoal());
            //model.Items.Add(CalcWeightGoal());
            
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