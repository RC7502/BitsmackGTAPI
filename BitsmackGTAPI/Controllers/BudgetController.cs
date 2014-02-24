using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BitsmackGTAPI.Models;

namespace BitsmackGTAPI.Controllers
{
    public class BudgetController : Controller
    {
        private readonly IBudgetService _service;

        public BudgetController()
        {
            _service = StructureMap.ObjectFactory.GetInstance<IBudgetService>();
        }

        public ActionResult MonthDetail()
        {
            return Json(_service.GetMonthCategories(), JsonRequestBehavior.AllowGet);
        }

    }
}
