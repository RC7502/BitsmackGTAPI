using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BitsmackGTAPI.Models;

namespace BitsmackGTAPI.Controllers
{
    public class PedometerController : Controller
    {
        //
        // GET: /Pedometer/

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Summary()
        {
            var service = StructureMap.ObjectFactory.GetInstance<IPedometerService>();

            //refresh data
            return Json(service.GetSummary(), JsonRequestBehavior.AllowGet);
        }

    }
}
