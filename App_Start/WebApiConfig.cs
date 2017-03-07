using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace EnouFlowWebApi
{
  public static class WebApiConfig
  {
    public static void Register(HttpConfiguration config)
    {
      // Web API configuration and services
      config.EnableCors();

      // Web API routes
      config.MapHttpAttributeRoutes();

      config.Routes.MapHttpRoute(
          name: "DefaultApi",
          routeTemplate: "api/{controller}/{id}",
          defaults: new { id = RouteParameter.Optional }
      );

      //Not support XML
      config.Formatters.Remove(config.Formatters.XmlFormatter);

      //Avoid ojbects reference looping for 
      GlobalConfiguration.Configuration.Formatters.
        JsonFormatter.SerializerSettings.ReferenceLoopHandling = 
        Newtonsoft.Json.ReferenceLoopHandling.Ignore;
    }
  }
}
