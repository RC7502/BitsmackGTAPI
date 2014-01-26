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
            var model = new PedometerSummaryViewModel();
            var key = _commonService.GetAPIKeys().FirstOrDefault(x => x.service_name == "Fitbit");
            if (key != null)
            {
                if((DateTime.UtcNow - key.last_update).TotalMinutes > 20)
                {
                    var existingRecs = GetPedometerRecords().ToList();
                    var startDate = existingRecs.Any() ? existingRecs.Max(x => x.trandate) : key.start_date;
                    RefreshData(false, startDate, DateTime.Today.AddDays(-1));
                    key.last_update = DateTime.UtcNow;
                    _commonService.UpdateAPIKey(key);
                }


                var pedometerRecs = GetPedometerRecords().ToList();
                var stepList = pedometerRecs.Select(x => x.steps).ToList();
                model = new PedometerSummaryViewModel()
                    {
                        AverageSteps = MathHelper.Average(stepList),
                        NumOfDays = pedometerRecs.Count,
                        TrendSteps = MathHelper.TrendAverage(stepList),
                        NextUpdate = Convert.ToInt32((key.last_update.AddMinutes(20) - DateTime.UtcNow).TotalMinutes)
                    };
                var stepsToBeat = Math.Max(model.AverageSteps, model.TrendSteps);
                model.NewStepGoal = (int) Math.Round(stepsToBeat*1.02, 0);

                //SetFitbitNewGoal(key, model.NewStepGoal);
            }
            return model;
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
            var list = GetFitBitData(overwrite, startDate, endDate);
            foreach (var pedometer in list)
            {
                var existingTran = _pedometerRepo.AllForRead().FirstOrDefault(x => x.trandate == pedometer.trandate);
                if (existingTran != null)
                    _pedometerRepo.Delete(existingTran);
                _pedometerRepo.Insert(pedometer);
            }
            _pedometerRepo.Save();
        }

        private IEnumerable<Pedometer> GetFitBitData(bool overwrite, DateTime startDate, DateTime endDate)
        {
            var key = _commonService.GetAPIKeys().FirstOrDefault(x => x.service_name == "Fitbit");
            var list = new List<Pedometer>();
            try
            {           
                var fbClient = GetFitbitClient(key);
                var existingRecs = !overwrite ? GetPedometerRecords().ToList() : new List<Pedometer>();
                var weightlog = fbClient.GetWeight(startDate, startDate.AddDays(30));
                for (var d = startDate; d <= endDate && list.Count < 30; d = d.AddDays(1))
                {
                    if (existingRecs.Any(x => x.trandate == d)) continue;
                    var newrec = new Pedometer();
                    var activity = fbClient.GetDayActivitySummary(d);
                    var sleep = fbClient.GetSleep(d);
                    newrec.trandate = d;
                    newrec.steps = activity.Steps;
                    newrec.sleep = sleep.Summary.TotalMinutesAsleep;
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