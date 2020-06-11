using Microsoft.Owin;
using Owin;

[assembly: OwinStartup(typeof(KendoCRUDService.Startup))]

namespace KendoCRUDService
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
        }
    }
}