using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
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
        private readonly IGTRepository<TimedActivities> _timedRepo; 
        private readonly ICommonService _commonService;

        public GoalService(IGTRepository<TimedActivities> timedActvities,
            ICommonService commonService)
        {
            _timedRepo = timedActvities;
            _commonService = commonService;
        }

        public GoalsSummaryViewModel GetSummary()
        {
            var model = new GoalsSummaryViewModel();
            RefreshToggl(false, DateTime.Now.AddDays(-14), DateTime.Now.AddDays(-1));
            model.Items.Add(CalcStandingDeskGoal());
            
            return model;
        }

        private GoalSummaryViewModel CalcStandingDeskGoal()
        {
            var recs = _timedRepo.AllForRead().Where(x => x.description == "Standing Desk").OrderBy(x=>x.startdate);
            var firstRec = recs.FirstOrDefault();
            var model = new GoalSummaryViewModel()
                {
                    Name = "Standing Desk"
                };
            if (firstRec != null)
            {
                model.AvgValue = recs.Sum(x => x.duration)/
                                 TimeHelper.GetBusinessDays(firstRec.startdate, DateTime.UtcNow.AddDays(-1));
                model.NewGoalValue = model.AvgValue*1.01;
            }

            return model;
        }

        private void RefreshToggl(bool overwrite, DateTime start, DateTime end)
        {
            var key = _commonService.GetAPIKeyByName("Toggl");
            if (key == null || ((DateTime.UtcNow - key.last_update).TotalMinutes < 60)) return;           
            var list = GetTogglData(key, start, end);
            var existingRecs = _timedRepo.AllForRead().ToList();
            foreach (var entry in list)
            {
                var existingAct = existingRecs.FirstOrDefault(x => x.description == entry.description
                                                                              && x.duration == entry.duration
                                                                              && x.startdate.Date == DateTime.Parse(entry.start).Date);
                if (existingAct == null)
                {
                    var newRec = new TimedActivities()
                        {
                            description = entry.description,
                            duration = entry.duration,
                            startdate = DateTime.Parse(entry.start),
                            enddate = DateTime.Parse(entry.stop)
                        };
                    _timedRepo.Insert(newRec);
                }
            }

            _timedRepo.Save();
            key.last_update = DateTime.UtcNow;
            _commonService.UpdateAPIKey(key);

        }

        private IEnumerable<TimeEntry> GetTogglData(APIKeys key, DateTime start, DateTime end)
        {
            var auth = new TogglAuthRequest() {ApiToken = key.user_token};
            var api = new TogglApi.TogglApi(auth);
            var entries = api.Users.GetAllTimeEntries(start, end);
            return entries;
        }
    }
}