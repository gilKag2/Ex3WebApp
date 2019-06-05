using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace FinalWork
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute("Display", "display/{ip}/{port}",
                defaults: new { controller = "Main", action = "Display" });

            routes.MapRoute("DisplayWithRoute", "display/{ip}/{port}/{refreshRate}",
                defaults: new { controller = "Main", action = "DisplayWithRoute" });

            routes.MapRoute("SaveToFile", "save/{ip}/{port}/{refreshRate}/{duration}/{fileName}",
                defaults: new { controller = "Main", action = "SaveToFile" });

            //routes.MapRoute("Default", "{controller}/{action}/{id}",
            //    defaults: new { controller = "Main", action = "Index", id = UrlParameter.Optional });

        }
    }
}
