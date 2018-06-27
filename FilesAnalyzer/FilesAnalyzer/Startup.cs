using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(FilesAnalyzer.Startup))]
namespace FilesAnalyzer
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
