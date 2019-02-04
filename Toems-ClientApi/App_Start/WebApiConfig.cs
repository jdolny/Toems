using System.Web.Http;
using System.Web.Http.Dispatcher;
using Newtonsoft.Json;
using Toems_ClientApi.MessageHandlers;

namespace Toems_ClientApi
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Web API configuration and services
            config.Formatters.XmlFormatter.SupportedMediaTypes.Clear();
            // Web API routes
            config.MapHttpAttributeRoutes();

            config.EnableCors();
            config.Routes.MapHttpRoute("DefaultApiProvisioned", "ProvisionedComm/{controller}/{action}/{id}",
               defaults: new { id = RouteParameter.Optional },
               constraints: null,
               handler: new SymmKeyHandler() { InnerHandler = new HttpControllerDispatcher(config) }
               );
            config.Routes.MapHttpRoute("DefaultApi", "{controller}/{action}/{id}", new {id = RouteParameter.Optional}
                );

           

            var json = config.Formatters.JsonFormatter;
            json.SerializerSettings.DateTimeZoneHandling = DateTimeZoneHandling.Unspecified;

            //config.MessageHandlers.Add(new EncryptHandler());
        }
    }
}
