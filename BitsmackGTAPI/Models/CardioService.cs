using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BitsmackGTAPI.Constants;
using BitsmackGTAPI.Helpers;
using BitsmackGTAPI.Interfaces;
using HealthGraphNet;
using HealthGraphNet.Models;

namespace BitsmackGTAPI.Models
{
    public class CardioService : ICardioService
    {
        private readonly IGTRepository<Cardio> _cardioRepo;
        private readonly ICommonService _commonService;

        public CardioService(IGTRepository<Cardio> cardioRepo, ICommonService commonService)
        {
            _cardioRepo = cardioRepo;
            _commonService = commonService;
        }

        public CardioSummaryViewModel GetSummary()
        {
            var model = new CardioSummaryViewModel();
            var key = _commonService.GetAPIKeys().FirstOrDefault(x => x.service_name == "RunKeeper");
            if (key != null)
            {
                if ((DateTime.UtcNow - key.last_update).TotalMinutes > 20)
                {
                    var existingRecs = GetCardioRecords().ToList();
                    var startDate = existingRecs.Any() ? existingRecs.Max(x => x.trandate) : key.start_date;
                    RefreshData(false, startDate, DateTime.UtcNow);
                    key.last_update = DateTime.UtcNow;
                    _commonService.UpdateAPIKey(key);
                }

                var cardioRecs = GetCardioRecords().Where(x=>x.activity == "Running").ToList();
                model = new CardioSummaryViewModel()
                    {
                        TotalRuns = cardioRecs.Count,
                        AvgMilesPerRun = MathHelper.MetersToMiles(cardioRecs.Average(x=>x.distance)),
                        AvgAdj5KPace = CalcAvgAdj5KPace(cardioRecs)
                    };


            }
            return model;
        }

        private int CalcAvgAdj5KPace(IEnumerable<Cardio> cardioRecs)
        {
            var adj5KPaces = cardioRecs.Select(cardio => cardio.time*(Math.Pow(3.1/MathHelper.MetersToMiles(cardio.distance), 1.06))).ToList();
            return (int) Math.Round(adj5KPaces.Average(), 0);
        }

        private void RefreshData(bool overwrite, DateTime startDate, DateTime endDate)
        {
            var list = GetRunKeeperData(overwrite, startDate, endDate);
            foreach (var cardio in list)
            {
                var existingTran = _cardioRepo.AllForRead().FirstOrDefault(x => x.trandate == cardio.trandate);
                if (existingTran != null)
                    _cardioRepo.Delete(existingTran);
                _cardioRepo.Insert(cardio);
            }
            _cardioRepo.Save();
        }

        private IEnumerable<Cardio> GetRunKeeperData(bool overwrite, DateTime startDate, DateTime endDate)
        {
            var key = _commonService.GetAPIKeys().FirstOrDefault(x => x.service_name == "RunKeeper");
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
                    //Logger
                    Logger.WriteLog(EventLogSeverity.Error, ex.Message);

                    //Update Last Modified Date
                    key.last_update = DateTime.UtcNow;
                    _commonService.UpdateAPIKey(key);
                }
            }
            return list;
        }

        private IEnumerable<Cardio> GetCardioRecords()
        {
            return _cardioRepo.AllForRead();
        }
    }


}