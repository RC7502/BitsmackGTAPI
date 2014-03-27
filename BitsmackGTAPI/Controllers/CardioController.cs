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
        private readonly ICardioService _service = StructureMap.ObjectFactory.GetInstance<ICardioService>();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Summary()
        {
            //refresh data
            return Json(_service.GetSummary(), JsonRequestBehavior.AllowGet);
        }

        public ActionResult WeatherForecast()
        {
            return Json(_service.GetWeatherForecast(), JsonRequestBehavior.AllowGet);
        }

    }
}
