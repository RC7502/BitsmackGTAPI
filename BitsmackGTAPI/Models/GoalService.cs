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
        private readonly IGTRepository<Goals> _goalsRepo;
        private readonly IGTRepository<GoalTran> _goalTranRepo; 
        private readonly ICommonService _commonService;

        public GoalService(IGTRepository<Goals> goals, 
            IGTRepository<GoalTran> goaltran,
            ICommonService commonService)
        {
            _goalsRepo = goals;
            _goalTranRepo = goaltran;
            _commonService = commonService;
        }

        public GoalSummaryViewModel GetSummary()
        {
            var model = new GoalSummaryViewModel();
            RefreshToggl(true, DateTime.Now.AddYears(-1), DateTime.Now.AddDays(-1));

            return model;
        }

        private void RefreshToggl(bool overwrite, DateTime start, DateTime end)
        {
            var key = _commonService.GetAPIKeyByName("Toggl");
            if (key == null || ((DateTime.UtcNow - key.last_update).TotalMinutes < 60)) return;
            var list = GetTogglData(key, start, end);

        }

        private Task[] GetTogglData(APIKeys key, DateTime start, DateTime end)
        {
            var tasks = new Task[] {};
            var auth = new TogglAuthRequest() {ApiToken = key.user_token};
            var api = new TogglApi.TogglApi(auth);
            var user = api.Users.GetCurrentDetailed();
            var projects = user.projects;
            var entries = user.time_entries;



            return tasks;
        }
    }
}