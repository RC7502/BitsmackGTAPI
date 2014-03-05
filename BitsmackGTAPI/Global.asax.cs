﻿using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using BitsmackGTAPI.Interfaces;
using BitsmackGTAPI.Models;
using Fitbit.Api;
using StructureMap;

namespace BitsmackGTAPI
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801

    public class MvcApplication : System.Web.HttpApplication
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }

        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                "Default", // Route name
                "{controller}/{action}/{id}", // URL with parameters
                new { controller = "Home", action = "Index", id = UrlParameter.Optional } // Parameter defaults
            );

        }

        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();

            // Use LocalDB for Entity Framework by default
            Database.DefaultConnectionFactory = new SqlConnectionFactory(@"Data Source=(localdb)\v11.0; Integrated Security=True; MultipleActiveResultSets=True");

            RegisterGlobalFilters(GlobalFilters.Filters);
            RegisterRoutes(RouteTable.Routes);
            ObjectFactory.Initialize(x =>
            {
                x.For<IPedometerService>().Use<PedometerService>();
                x.For<ICardioService>().Use<CardioService>();
                x.For<ICommonService>().Use<CommonService>();
                x.For<IGoalService>().Use<GoalService>();
                x.For<IBudgetService>().Use<BudgetService>();
                x.For<IDAL>().Use<DAL>();
                
                x.For(typeof(IGTRepository<>)).Use(typeof(GTRepository<>));
            });

        }
    }
}