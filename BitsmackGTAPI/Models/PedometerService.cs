using System;
using System.Collections.Generic;
using System.Linq;
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
        private readonly IGTRepository<Pedometer> _pedometerRepo;
        private readonly ICommonService _commonService;

        public PedometerService(IGTRepository<Pedometer> db, ICommonService commonService)
        {
            _pedometerRepo = db;
            _commonService = commonService;
        }

        public PedometerSummaryViewModel GetSummary()
        {
            RefreshData(true, DateTime.Today.AddDays(-14), DateTime.Today.AddDays(-1));
            var pedometerRecs = GetPedometerRecords().ToList();
            var stepList = pedometerRecs.Select(x => x.steps).ToList();
            //store this in a table
            var wakeuptime = TimeHelper.ConvertUtcToLocal(DateTime.UtcNow.Date).Date.AddDays(1).AddHours(5).AddMinutes(50);

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

            //SetFitbitNewGoal(key, model.NewStepGoal);
            
            return model;
        }

        public PedometerDetailViewModel GetDetail(DateTime start, DateTime end)
        {
            var model = new PedometerDetailViewModel();
            RefreshData(true, start, end);
            var pedometerRecs = GetPedometerRecords(start, end);
            foreach (var rec in pedometerRecs)
            {
                model.Details.Add(new PedometerViewModel(rec));
            }
            return model;
        }

        private IEnumerable<Pedometer> GetPedometerRecords(DateTime start, DateTime end)
        {
            return _pedometerRepo.AllForRead().Where(x => x.trandate >= start && x.trandate <= end);
        }

        private void SetFitbitNewGoal(APIKeys key, int newStepGoal)
        {
            try
            {

                var fbClient = GetFitbitClient(key);
                fbClient.SetStepGoal(newStepGoal);


            }
            catch (Exception ex)
            {      
                Logger.WriteLog(EventLogSeverity.Error, ex.Message);
            }
        }

        private void RefreshData(bool overwrite, DateTime startDate, DateTime endDate)
        {
            var key = _commonService.GetAPIKeyByName("Fitbit");
            if (key == null || ((DateTime.UtcNow - key.last_update).TotalMinutes < 60)) return;
            var list = GetFitBitData(key, startDate, endDate);
            foreach (var pedometer in list)
            {
                var existingTran =
                    _pedometerRepo.AllForRead().FirstOrDefault(x => x.trandate == pedometer.trandate);
                if (existingTran != null && overwrite)
                {
                    Copy(pedometer, existingTran);
                    _pedometerRepo.Update(existingTran);
                }
                else if (existingTran == null)
                {
                    _pedometerRepo.Insert(pedometer);
                }
            }
            _pedometerRepo.Save();

            key.last_update = DateTime.UtcNow;
            _commonService.UpdateAPIKey(key);
        }

        private void Copy(Pedometer from, Pedometer to)
        {
            to.bodyfat = from.bodyfat;
            to.sleep = from.sleep;
            to.steps = from.steps;
            to.trandate = from.trandate;
            to.weight = from.weight;
            to.lastupdateddate = DateTime.UtcNow;
        }

        private IEnumerable<Pedometer> GetFitBitData(APIKeys key, DateTime startDate, DateTime endDate)
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
                    var newrec = new Pedometer
                        {
                            trandate = d,
                            steps = activity.Steps,
                            sleep = sleep.Summary.TotalMinutesAsleep,
                            createddate = DateTime.UtcNow
                        };
                    var dayWeight = weightlog.Weights.FirstOrDefault(x => x.Date == d);
                    newrec.weight = dayWeight != null ? dayWeight.Weight*2.20462 : 0;

                    list.Add(newrec);
                }
            }
            catch (Exception ex)
            {
                //Logger
                Logger.WriteLog(EventLogSeverity.Error, ex.Message);

                //Update Last Modified Date
                if (key != null)
                {
                    key.last_update = DateTime.UtcNow;
                    _commonService.UpdateAPIKey(key);
                }
            }
            return list;
        }

        private IEnumerable<Pedometer> GetPedometerRecords()
        {
            return _pedometerRepo.AllForRead();
        }

        private FitbitClient GetFitbitClient(APIKeys key)
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