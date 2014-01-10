using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BitsmackGTAPI.Interfaces;

namespace BitsmackGTAPI.Models
{
    public class PedometerService : IPedometerService
    {
        private readonly IPedometerRepository _repo;

        public PedometerService(IPedometerRepository db)
        {
            _repo = db;
        }


        public PedometerSummaryViewModel GetSummary()
        {
            var model = new PedometerSummaryViewModel();
            var pedometerRec = _repo.All;
            if (pedometerRec != null && pedometerRec.Any())
            {
                model.AverageSteps = (int)Math.Round(pedometerRec.Average(x=>x.steps), 0);
                model.NumOfDays = pedometerRec.Count();
            }

            return model;
        }
    }

 
}