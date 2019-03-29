using Microsoft.AspNet.SignalR;
using Microsoft.Owin;
using Owin;
using Toems_ClientApi;

[assembly: OwinStartup(typeof(Startup))]

namespace Toems_ClientApi
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            var hubConfiguration = new HubConfiguration();
            hubConfiguration.EnableDetailedErrors = true;
            app.MapSignalR(hubConfiguration);
        }
    }
}
