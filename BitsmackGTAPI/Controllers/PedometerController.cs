using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BitsmackGTAPI.Interfaces;
using BitsmackGTAPI.Models;

namespace BitsmackGTAPI.Controllers
{
    public class PedometerController : Controller
    {
        private readonly IPedometerService _service;

        public PedometerController()
        {
            _service = StructureMap.ObjectFactory.GetInstance<IPedometerService>();
        }


        //
        // GET: /Pedometer/

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Summary()
        {
            return Json(_service.GetSummary(), JsonRequestBehavior.AllowGet);
        }

        public ActionResult Detail()
        {
            var start = (DateTime)SqlDateTime.MinValue;
            var end = (DateTime)SqlDateTime.MaxValue;

            var model = _service.GetDetail(start, end);
            var list = model.Details.Select(item => new[] { item.trandate,item.steps.ToString(), item.sleep.ToString(), item.createddate, item.lastupdateddate  }).ToList();

            return Json(new {aaData = list}, JsonRequestBehavior.AllowGet);
        }

        public ActionResult MonthAverages()
        {
            return Json(_service.GetMonthAverages(), JsonRequestBehavior.AllowGet);
        }

    }
}
