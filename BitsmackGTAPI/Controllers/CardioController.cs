using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BitsmackGTAPI.Models;

namespace BitsmackGTAPI.Controllers
{
    public class CardioController : Controller
    {
        //
        // GET: /Cardio/

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Summary()
        {
            var service = StructureMap.ObjectFactory.GetInstance<ICardioService>();

            //refresh data
            return Json(service.GetSummary(), JsonRequestBehavior.AllowGet);
        }

    }
}
