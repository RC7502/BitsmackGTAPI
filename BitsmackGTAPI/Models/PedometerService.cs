﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BitsmackGTAPI.Constants;
using BitsmackGTAPI.Helpers;
using BitsmackGTAPI.Interfaces;
using Fitbit.Api;

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
            var key = _commonService.GetAPIKeys().FirstOrDefault(x => x.service_name == "Fitbit");
            if (key != null && (DateTime.Now - key.last_update).Hours > 1)
            {
                var existingRecs = GetPedometerRecords().ToList();
                var startDate = existingRecs.Any() ? existingRecs.Max(x => x.trandate) : key.start_date;
                RefreshData(false, startDate, DateTime.Today.AddDays(-1));
                key.last_update = DateTime.Now;
                _commonService.UpdateAPIKey(key);
            }

            var pedometerRecs = GetPedometerRecords().ToList();
            var stepList = pedometerRecs.Select(x => x.steps).ToList();
            var model = new PedometerSummaryViewModel()
                {
                    AverageSteps = MathHelper.Average(stepList),
                    NumOfDays = pedometerRecs.Count,
                    TrendSteps = MathHelper.TrendAverage(stepList),
                };
            model.NewStepGoal = (int) Math.Round(model.AverageSteps*1.05, 0);         
            return model;
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
                for (var d = startDate; d <= endDate; d = d.AddDays(1))
                {
                    if (existingRecs.Any(x => x.trandate == d)) continue;
                    var newrec = new Pedometer();
                    var activity = fbClient.GetDayActivitySummary(d);
                    var sleep = fbClient.GetSleep(d);
                    newrec.trandate = d;
                    newrec.steps = activity.Steps;
                    newrec.sleep = sleep.Summary.TotalMinutesAsleep;
                    newrec.weight = GetWeight(fbClient, d);
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
                    key.last_update = DateTime.Now;
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