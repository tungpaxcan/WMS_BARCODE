using Microsoft.Owin;
using Owin;

[assembly: OwinStartup(typeof(WMS.Startup))]

namespace WMS
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            // Kích hoạt middleware SignalR
            app.MapSignalR();
        }
    }
}