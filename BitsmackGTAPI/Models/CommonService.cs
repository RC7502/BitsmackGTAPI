using System;
using System.Collections.Generic;
using System.Linq;
using BitsmackGTAPI.Constants;
using BitsmackGTAPI.Helpers;
using BitsmackGTAPI.Interfaces;

namespace BitsmackGTAPI.Models
{
    public class CommonService : ICommonService
    {
        private readonly IDAL _dal;

        public CommonService(IDAL dal)
        {
            _dal = dal;
        }

        public IEnumerable<APIKeys> GetAPIKeys()
        {
            return _dal.GetAPIKeys();
        }

        public void UpdateAPIKey(APIKeys key)
        {
            _dal.Update(key);
            _dal.SaveAPIKey();
        }

        public APIKeys GetAPIKeyByName(string name)
        {
            return GetAPIKeys().FirstOrDefault(x => x.service_name == name);
        }

        public void WriteLog(EventLogSeverity severity, string area, string message)
        {
            var log = new EventLog
            {
                severity = (int)severity,
                message = message,
                area = area,
                eventdate = DateTime.UtcNow
            };
            _dal.Insert(log);
            _dal.SaveLog();
        }

        public void LogActivity(Pedometer pedometer)
        {
            //Steps
            WriteLog(EventLogSeverity.Activity, EventLogAreas.PEDOMETER, string.Format("Walked {0} steps on {1}", pedometer.steps, pedometer.trandate.ToShortDateString()));

            //Sleep
            WriteLog(EventLogSeverity.Activity, EventLogAreas.PEDOMETER, string.Format("Slept for {0} on {1}",
                TimeHelper.MinutesToHours(pedometer.sleep), pedometer.trandate.ToShortDateString()));

            //Weight
            WriteLog(EventLogSeverity.Activity, EventLogAreas.PEDOMETER, string.Format("Weigh in of {0} on {1}", pedometer.weight, pedometer.trandate.ToShortDateString()));
        }

        public void LogActivity(Todos todoDay)
        {
            WriteLog(EventLogSeverity.Activity, EventLogAreas.TODO, "Message goes here.");
        }

        public void LogActivity(Cardio cardio)
        {
            switch (cardio.activity)
            {
                case "Running":
                    var miles = MathHelper.MetersToMiles(cardio.distance);
                    WriteLog(EventLogSeverity.Activity, EventLogAreas.CARDIO,
                         string.Format("Ran for {0} miles in {1} on {2}. Average Pace was {3}. Adjusted 5K Pace was {4}.", miles,
                         TimeHelper.SecondsToTime(cardio.time), cardio.trandate, TimeHelper.SecondsToTime(cardio.time / miles),
                         MathHelper.Adj5KPace(cardio.time, cardio.distance)));
                    break;
            }
        }

        public void LogActivity(TimedActivities newRec)
        {
            WriteLog(EventLogSeverity.Activity, EventLogAreas.GOALS, string.Format("{0} for {1} on {2}", newRec.description,
                TimeHelper.SecondsToTime(newRec.duration), newRec.enddate.ToShortDateString()));
        }
    }
}