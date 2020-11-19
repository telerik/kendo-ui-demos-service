using Microsoft.AspNet.SignalR;
using Microsoft.Owin;
using Microsoft.Owin.Cors;
using Owin;

[assembly: OwinStartup(typeof(KendoCRUDService.Startup))]

namespace KendoCRUDService
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            app.UseCors(CorsOptions.AllowAll);

            app.MapSignalR("/signalr/hubs", new HubConfiguration
            {
                EnableJSONP = true
            });
        }
    }
}