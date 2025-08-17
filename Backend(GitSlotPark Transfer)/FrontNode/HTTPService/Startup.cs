using Owin;
using Microsoft.Owin;
using System.Web.Http;

[assembly: OwinStartup(typeof(FrontNode.HTTPService.Startup))]
namespace FrontNode.HTTPService
{
    public class Startup
    {
        public void Configuration(IAppBuilder appBuilder)
        {
            appBuilder.UseCors(Microsoft.Owin.Cors.CorsOptions.AllowAll);
            HttpConfiguration config = new HttpConfiguration();
            config.MapHttpAttributeRoutes();
            appBuilder.UseWebApi(config);
        }
    }
}
