using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Penates.Startup))]
namespace Penates
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
