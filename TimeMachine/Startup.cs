using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(TimeMachine.Startup))]
namespace TimeMachine
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
