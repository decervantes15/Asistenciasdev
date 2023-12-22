using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace Asistencias
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                name: "Administracion",
                url: "Administracion",
                defaults: new { controller = "Administracion", action = "Login", id = UrlParameter.Optional }
            );
            
            routes.MapRoute(
                name: "Administracion_Login",
                url: "Administracion/Login",
                defaults: new { controller = "Administracion", action = "Login", id = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
