using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BitsmackGTAPI.Interfaces;

namespace BitsmackGTAPI
{
    public class DashboardController : Controller
    {
        private readonly IDashboardService _service;

        public DashboardController()
        {
            _service = StructureMap.ObjectFactory.GetInstance<IDashboardService>();
        }

        public ActionResult Index()
        {
            return Json(_service.GetDashboard(), JsonRequestBehavior.AllowGet);
        }

    }
}
