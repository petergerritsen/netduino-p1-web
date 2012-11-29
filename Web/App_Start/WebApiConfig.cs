using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace Web
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            config.Routes.MapHttpRoute(
               name: "UsageApi",
               routeTemplate: "api/usages/{key}/{action}/{offset}/{count}",
               defaults: new { controller = "Usages", key = "", action = "Hourly", offset = 0, count = RouteParameter.Optional }
            );

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );

           
        }
    }
}
