using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Live_Quiz.Startup))]
namespace Live_Quiz
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
