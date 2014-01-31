using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BitsmackGTAPI.Helpers;

namespace BitsmackGTAPI.Models
{
    public class PedometerDetailViewModel
    {
        public List<PedometerViewModel> Details;

        public PedometerDetailViewModel()
        {
            Details = new List<PedometerViewModel>();
        }
    }

    public class PedometerViewModel
    {
        public int steps;
        public int sleep;
        public string trandate;
        public string createddate;
        public string lastupdateddate;

        public PedometerViewModel(Pedometer rec)
        {
            steps = rec.steps;
            sleep = rec.sleep;
            trandate = rec.trandate.ToShortDateString();
            createddate = rec.createddate.HasValue ? TimeHelper.ConvertUtcToLocal(rec.createddate.Value).ToShortDateString() : string.Empty;
            lastupdateddate = rec.lastupdateddate.HasValue ? TimeHelper.ConvertUtcToLocal(rec.lastupdateddate.Value).ToShortDateString() : string.Empty;
        }
    }
}