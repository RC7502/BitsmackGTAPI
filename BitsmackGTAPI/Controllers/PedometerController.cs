using System;
using System.Collections.Generic;
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
            //refresh data
            return Json(_service.GetSummary(), JsonRequestBehavior.AllowGet);
        }

        public ActionResult Detail(string startDate, string endDate)
        {
            var start = DateTime.Parse(startDate);
            var end = DateTime.Parse(endDate);

            return Json(_service.GetDetail(start, end), JsonRequestBehavior.AllowGet);
        }

    }
}
