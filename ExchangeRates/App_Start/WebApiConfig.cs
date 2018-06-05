using System.Web.Http;
using System.Web.Http.Cors;

namespace ExchangeRates
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Добавляем поддержку CORS
            var cors = new EnableCorsAttribute("*", "*", "*");
            config.EnableCors(cors);

            // Маршруты веб-API
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "RatesUniq",
                routeTemplate: "api/{controller}/{id}/{dtStart}"
            );

            config.Routes.MapHttpRoute(
                name: "RatesPeriod",
                routeTemplate: "api/{controller}/{id}/{dtStart}/{dtEnd}"
            );

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );

            config.Formatters.Remove(config.Formatters.XmlFormatter);
        }
    }
}
