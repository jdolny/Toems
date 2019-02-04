using System.IO;
using System.Web;
using log4net;
using log4net.Config;
using GlobalConfiguration = System.Web.Http.GlobalConfiguration;

namespace Toems_ApplicationApi
{
    public class WebApiApplication : HttpApplication
    {
        protected void Application_Start()
        {
            GlobalConfiguration.Configure(WebApiConfig.Register);
            var logPath = HttpContext.Current.Server.MapPath("~") + Path.DirectorySeparatorChar + "private" +
                          Path.DirectorySeparatorChar + "logs" + Path.DirectorySeparatorChar +
                          "Application.log";
            GlobalContext.Properties["LogFile"] = logPath;
            XmlConfigurator.Configure();
        }
    }
}