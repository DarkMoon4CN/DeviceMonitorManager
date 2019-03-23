using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace MonitorManager.Api
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {

            config.EnableCors();
            // Web API 配置和服务

            // Web API 路由
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{action}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );

             var formatters = GlobalConfiguration.Configuration.Formatters;
             var jsonFormatter = formatters.JsonFormatter;
             var settings = jsonFormatter.SerializerSettings;
             settings.Formatting = Newtonsoft.Json.Formatting.Indented;
             settings.DateFormatString = "yyyy-MM-dd HH:mm:ss";
             //settings.ContractResolver = new CamelCasePropertyNamesContractResolver();

        }
    }
}
