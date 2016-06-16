using Microsoft.Owin;
using Owin;

[assembly: OwinStartup(typeof(SnapAndSaveService.Startup))]

namespace SnapAndSaveService
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureMobileApp(app);
        }
    }
}