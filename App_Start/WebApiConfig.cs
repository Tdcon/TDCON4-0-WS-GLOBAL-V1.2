using System;
using System.Collections.Generic;
using System.Linq;
//using System.Net.Http.Headers;
using System.Web.Http;
using WSGlobal.App_Start;
using WSGlobal.Security;

namespace WSGlobal
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Configuración y servicios de API web
            // Web API configuration and services
            config.EnableCors(new AccessPolicyCors());

            // Rutas de API web
            config.MapHttpAttributeRoutes();

            config.MessageHandlers.Add(new TokenValidationHandler());
            //config.Formatters.JsonFormatter.SupportedMediaTypes.Add(new MediaTypeHeaderValue("text/plain"));
            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );
        }
    }
}
