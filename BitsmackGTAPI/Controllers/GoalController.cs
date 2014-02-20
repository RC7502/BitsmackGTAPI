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
            return Json(_service.GetSummary(), JsonRequestBehavior.AllowGet);
        }

    }
}
