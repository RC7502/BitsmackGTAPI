using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BitsmackGTAPI.Interfaces;

namespace BitsmackGTAPI.Controllers
{
    public class GoalController : Controller
    {
        private readonly IGoalService _service;

        public GoalController()
        {
            _service = StructureMap.ObjectFactory.GetInstance<IGoalService>();
        }

        public ActionResult Summary()
        {
            var model = _service.GetSummary();
            var list = model.Items.Select(item => new[]
                {
                    item.Name,
                    item.AvgValue.ToString(),
                    item.TrendAvg.ToString(),
                    item.NewGoalValue.ToString(),             
                });

            return Json(new {aaData = list}, JsonRequestBehavior.AllowGet);
        }

        public ActionResult WeightDetail()
        {
            return Json(new {aaData = _service.GetWeightCalDetail()}, JsonRequestBehavior.AllowGet);
        }

        public ActionResult HabitDetail()
        {
            var modelList = _service.GetHabitDetail();
            
            return Json(new {aaData = modelList}, JsonRequestBehavior.AllowGet);
        }

    }
}
