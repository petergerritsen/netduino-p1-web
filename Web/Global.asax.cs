﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using Core.Persistence;
using System.Data.Entity.Migrations;
using Core.Migrations;

namespace Web
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801

    public class WebApiApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            //var webconfiguration = System.Web.Configuration.WebConfigurationManager.OpenWebConfiguration("~");
            //var connectionString = webconfiguration.ConnectionStrings.ConnectionStrings["NetduinoP1Logging"].ConnectionString;
            //throw new ArgumentException("Connstring: " + connectionString);

            RouteTable.Routes.MapHubs();

            AreaRegistration.RegisterAllAreas();

            WebApiConfig.Register(GlobalConfiguration.Configuration);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            // WebApi Configuration to hook up formatters and message handlers
            RegisterApis(GlobalConfiguration.Configuration);

            //// Run migrations
            //var configuration = new Configuration();
            //var migrator = new DbMigrator(configuration);
            //migrator.Update();
            
        }

        public static void RegisterApis(HttpConfiguration config) {
            // remove default Xml handler
            var matches = config.Formatters
                                .Where(f => f.SupportedMediaTypes
                                             .Where(m => m.MediaType.ToString() == "application/xml" ||
                                                         m.MediaType.ToString() == "text/xml")
                                             .Count() > 0)
                                .ToList();
            foreach (var match in matches)
                config.Formatters.Remove(match);
        }
    }
}