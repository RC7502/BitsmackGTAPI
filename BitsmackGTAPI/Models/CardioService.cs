using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BitsmackGTAPI.Interfaces;

namespace BitsmackGTAPI.Models
{
    public class CardioService
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
            return model;
        }
    }


}