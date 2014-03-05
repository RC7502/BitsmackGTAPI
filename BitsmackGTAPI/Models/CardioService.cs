using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using BitsmackGTAPI.Constants;
using BitsmackGTAPI.Helpers;
using BitsmackGTAPI.Interfaces;
using HealthGraphNet;

namespace BitsmackGTAPI.Models
{
    public class CardioService : ICardioService
    {
        private readonly IDAL _dal;
        private readonly ICommonService _commonService;

        public CardioService(IDAL dal, ICommonService commonService)
        {
            _dal = dal;
            _commonService = commonService;
        }

        public CardioSummaryViewModel GetSummary()
        {
            var model = new CardioSummaryViewModel();
            try
            {
                RefreshData(false, DateTime.UtcNow.AddDays(-7), DateTime.UtcNow);
                var cardioRecs = _dal.GetCardioRecords().Where(x => x.activity == "Running").ToList();
                model = new CardioSummaryViewModel
                    {
                        TotalRuns = cardioRecs.Count,
                        AvgMilesPerRun = MathHelper.MetersToMiles(cardioRecs.Average(x => x.distance)),
                        AvgAdj5KPace = CalcAvgAdj5KPace(cardioRecs)
                    };
            }
            catch (Exception ex)
            {
                _commonService.WriteLog(EventLogSeverity.Error, MethodBase.GetCurrentMethod().Name,ex.Message);
            }
            return model;
        }

        private int CalcAvgAdj5KPace(IEnumerable<Cardio> cardioRecs)
        {
            var adj5KPaces = cardioRecs.Where(x=>x.distance > 0).ToList().Select(cardio => MathHelper.Adj5KPace(cardio.time, cardio.distance));
            return (int) Math.Round(adj5KPaces.Average(), 0);
        }

        private void RefreshData(bool overwrite, DateTime startDate, DateTime endDate)
        {
            var key = _commonService.GetAPIKeyByName(APINames.RUNKEEPER);
            if (key == null || ((DateTime.UtcNow - key.last_update).TotalMinutes < 60)) return;
            var list = GetRunKeeperData(key, startDate, endDate);
            foreach (var cardio in list)
            {
                var existingTran = _dal.GetCardioRecords().FirstOrDefault(x => x.trandate == cardio.trandate);
                if (existingTran != null && overwrite)
                {
                    Copy(cardio, existingTran);
                    _dal.Update(existingTran);
                }
                else if (existingTran == null)
                {
                    _dal.Insert(cardio);
                    _commonService.LogActivity(cardio);
                }
                
            }
            _dal.SaveCardio();
            key.last_update = DateTime.UtcNow;
            _commonService.UpdateAPIKey(key);
        }

        private void Copy(Cardio cardio, Cardio existingTran)
        {
            existingTran.activity = cardio.activity;
            existingTran.distance = cardio.distance;
            existingTran.time = cardio.time;
            existingTran.trandate = cardio.trandate;
            
        }

        private IEnumerable<Cardio> GetRunKeeperData(APIKeys key, DateTime startDate, DateTime endDate)
        {
            var list = new List<Cardio>();
            if (key != null)
            {
                try
                {
                    var tokenManager = new AccessTokenManager(key.consumer_key, key.consumer_secret,
                                                              "http://runkeeper.com",
                                                              key.user_token);
                    var userRequest = new UsersEndpoint(tokenManager);
                    var user = userRequest.GetUser();

                    var activityRequest = new FitnessActivitiesEndpoint(tokenManager, user);
                    var activitiesPage = activityRequest.GetFeedPage(pageSize: 1000, modifiedNoEarlierThan: startDate, modifiedNoLaterThan: endDate);

                    list.AddRange(activitiesPage.Items.Select(item => new Cardio
                        {
                            trandate = TimeHelper.CopyDateTime(item.StartTime), 
                            activity = item.Type, 
                            distance = (float) item.TotalDistance, 
                            time = (float) item.Duration
                        }));
                }
                catch (Exception ex)
                {
                    //_commonService
                    _commonService.WriteLog(EventLogSeverity.Error,MethodBase.GetCurrentMethod().Name, ex.Message);

                    //Update Last Modified Date
                    key.last_update = DateTime.UtcNow;
                    _commonService.UpdateAPIKey(key);
                }
            }
            return list;
        }

    }


}