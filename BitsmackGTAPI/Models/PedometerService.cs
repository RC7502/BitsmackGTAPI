using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Web;
using BitsmackGTAPI.Constants;
using BitsmackGTAPI.Helpers;
using BitsmackGTAPI.Interfaces;
using Fitbit.Api;
using Fitbit.Models;

namespace BitsmackGTAPI.Models
{
    public class PedometerService : IPedometerService
    {
        private readonly ICommonService _commonService;
        private readonly IDAL _dal;

        public PedometerService(ICommonService commonService, IDAL dal)
        {
            _commonService = commonService;
            _dal = dal;
        }

        public PedometerSummaryViewModel GetSummary()
        {
            RefreshData(true, DateTime.Today.Date.AddDays(-14), DateTime.Today.Date);
            var pedometerRecs = _dal.GetPedometerRecords().ToList();       
            var stepList = pedometerRecs.Where(x=>x.trandate < DateTime.Today.Date).Select(x => x.steps).ToList();

            var wakeuptime = GetNextAlarm();

            var model = new PedometerSummaryViewModel()
                {
                    AverageSteps = MathHelper.Average(stepList),
                    NumOfDays = pedometerRecs.Count,
                    TrendSteps = MathHelper.TrendAverage(stepList),                 
                    AvgSleep = MathHelper.Average(pedometerRecs.Select(x=>x.sleep).ToList())
                };
            var stepsToBeat = Math.Max(model.AverageSteps, model.TrendSteps);
            model.NewStepGoal = (int) Math.Round(stepsToBeat*1.02, 0);
            model.SleepStartTime = wakeuptime.AddHours(-8).ToShortTimeString();
            model.SleepEndTime = wakeuptime.AddMinutes(-1*model.AvgSleep*1.02).ToShortTimeString();

            SetFitbitNewGoal(model.NewStepGoal);
            
            return model;
        }

        public DateTime GetNextAlarm()
        {
            try
            {
                var key = _commonService.GetAPIKeyByName(APINames.FITBIT);
                var fbClient = GetFitbitClient(key);
                var device = fbClient.GetDevices().FirstOrDefault();
                TextInfo myTI = new CultureInfo("en-US", false).TextInfo;
                if (device != null)
                {
                    var alarms = fbClient.GetAlarms(device.Id).Where(x=>x.Enabled);
                    //build a list of all upcoming alarms then sort it to find the next alarm
                    var upcomingalarms = new List<DateTime>();
                    var currentDay = TimeHelper.ConvertUtcToLocal(DateTime.UtcNow);
                    foreach (var alarm in alarms)
                    {
                        var alarmTime = TimeHelper.StringPlusTZToTimeSpan(alarm.Time);
                        if (!alarm.Recurring)
                        {      
                            upcomingalarms.Add(alarmTime > currentDay.TimeOfDay
                                                   ? currentDay.Date.AddDays(1).AddSeconds(alarmTime.TotalSeconds)
                                                   : currentDay.Date.AddSeconds(alarmTime.TotalSeconds));
                        }
                        else
                        {
                            foreach (var weekDay in alarm.WeekDays)
                            {

                                var titleDay = myTI.ToTitleCase(weekDay.ToLower());
                                //adding the next 2 weeks of alarms to be safe
                                var nextOccurence = TimeHelper.GetNextDayOfWeek(currentDay.Date,
                                                                                (DayOfWeek) Enum.Parse(typeof (DayOfWeek), titleDay)).AddSeconds(alarmTime.TotalSeconds);
                                upcomingalarms.Add(nextOccurence);
                                upcomingalarms.Add(nextOccurence.AddDays(7));                    
                            }

                        }
                    }

                    upcomingalarms.RemoveAll(x => x < currentDay);
                    if(upcomingalarms.Any())
                        return upcomingalarms.OrderBy(x => x).FirstOrDefault();       
                }


            }
            catch (Exception ex)
            {
                _commonService.WriteLog(EventLogSeverity.Error,MethodBase.GetCurrentMethod().Name, ex.Message);
            }

            return TimeHelper.ConvertUtcToLocal(DateTime.UtcNow.Date).Date.AddDays(1).AddHours(5);
        }

        public PedometerDetailViewModel GetDetail(DateTime start, DateTime end)
        {
            var model = new PedometerDetailViewModel();
            RefreshData(true, start, end);
            var pedometerRecs = _dal.GetPedometerRecords(start, end);
            foreach (var rec in pedometerRecs)
            {
                model.Details.Add(new PedometerViewModel(rec));
            }
            return model;
        }

        public ColumnChartModel GetMonthAverages()
        {
            var model = new ColumnChartModel();
            var allRecords = _dal.GetPedometerRecords().ToList();
            var key = _commonService.GetAPIKeyByName(APINames.FITBIT);
            if (key != null)
            {
                var categories = new List<string>();
                var values = new List<double>();
                var iDate = new DateTime(key.start_date.Year, key.start_date.Month, 1);
                var now = DateTime.UtcNow;
                var currentMonth = new DateTime(now.Year, now.Month, 1);
                while (iDate < currentMonth)
                {
                    categories.Add(iDate.Month + "-" + iDate.Year);
                    values.Add((int) allRecords.Where(x=>x.trandate.Month == iDate.Month && x.trandate.Year == iDate.Year).Average(x=>x.steps));
                    iDate = iDate.AddMonths(1);
                }
                model.PlotLine = (int)values.Average();
                //add this months value
                categories.Add(currentMonth.Month + "-" + currentMonth.Year);
                values.Add((int)allRecords.Where(x => x.trandate.Month == currentMonth.Month && x.trandate.Year == currentMonth.Year).Average(x => x.steps));

                model.Categories = categories.ToArray();
                model.SeriesData = values.ToArray();
            }
            return model;
        }

        public void SetFitbitNewGoal(int newStepGoal)
        {
            try
            {
                var key = _commonService.GetAPIKeyByName(APINames.FITBIT);
                var fbClient = GetFitbitClient(key);
                fbClient.SetStepGoal(newStepGoal);
            }
            catch (Exception ex)
            {
                _commonService.WriteLog(EventLogSeverity.Error, MethodBase.GetCurrentMethod().Name, ex.Message);
            }
        }

        public void RefreshData(bool overwrite, DateTime startDate, DateTime endDate)
        {
            var key = _commonService.GetAPIKeyByName(APINames.FITBIT);
            if (key == null || ((DateTime.UtcNow - key.last_update).TotalMinutes < 60)) return;
            var list = GetFitBitData(key, startDate, endDate);
            foreach (var pedometer in list)
            {
                var existingTran = _dal.GetPedometerRecords().FirstOrDefault(x => x.trandate == pedometer.trandate);
                if (existingTran != null && overwrite)
                {
                    Copy(pedometer, existingTran);
                    _dal.Update(existingTran);
                }
                else if (existingTran == null)
                {
                    _dal.Insert(pedometer);
                    _commonService.LogActivity(pedometer);
                }
            }
            _dal.SavePedometer();

            key.last_update = DateTime.UtcNow;
            _commonService.UpdateAPIKey(key);
        }

        public void Copy(Pedometer from, Pedometer to)
        {
            to.bodyfat = from.bodyfat;
            to.sleep = from.sleep;
            to.steps = from.steps;
            to.trandate = from.trandate;
            to.weight = from.weight;
            to.lastupdateddate = DateTime.UtcNow;
            to.calconsumed = from.calconsumed;
        }

        public IEnumerable<Pedometer> GetFitBitData(APIKeys key, DateTime startDate, DateTime endDate)
        {          
            var list = new List<Pedometer>();
            try
            {           
                var fbClient = GetFitbitClient(key);               
                var weightlog = fbClient.GetWeight(startDate, startDate.AddDays(30));
                for (var d = startDate; d <= endDate && list.Count < 30; d = d.AddDays(1))
                {                                       
                    var activity = fbClient.GetDayActivitySummary(d);
                    var sleep = fbClient.GetSleep(d);
                    var calLog = fbClient.GetFood(d);
                    var newrec = new Pedometer
                        {
                            trandate = d,
                            steps = activity.Steps,
                            sleep = sleep.Summary.TotalMinutesAsleep,
                            createddate = DateTime.UtcNow,
                            calconsumed = (int?) calLog.Summary.Calories
                        };
                    var dayWeight = weightlog.Weights.FirstOrDefault(x => x.Date == d);
                    newrec.weight = dayWeight != null ? dayWeight.Weight*2.20462 : 0;
                    
                    list.Add(newrec);
                }
            }
            catch (Exception ex)
            {
                //_commonService
                _commonService.WriteLog(EventLogSeverity.Error,MethodBase.GetCurrentMethod().Name, ex.Message);

                //Update Last Modified Date
                if (key != null)
                {
                    key.last_update = DateTime.UtcNow;
                    _commonService.UpdateAPIKey(key);
                }
            }
            return list;
        }

        public IFitbitClient GetFitbitClient(APIKeys key)
        {
            return new FitbitClient(key.consumer_key, key.consumer_secret, key.user_token, key.user_secret);
        }

        public double GetWeight(FitbitClient client, DateTime parmdate)
        {
            try
            {
                var weightlog = client.GetWeight(parmdate, parmdate);
                var todayWeight = weightlog.Weights.FirstOrDefault();
                if(todayWeight != null)
                    return todayWeight.Weight * 2.20462;

                return 0;
            }
            catch (Exception ex)
            {
                var messsage = ex.Message;
                return 0;
            }
        }
    }

 
}