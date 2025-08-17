using Microsoft.Owin.Cors;
using Owin;
using System.Web.Http;

namespace QueenApiNode.HttpService
{
    public class Startup
    {
        public void Configuration(IAppBuilder appBuilder)
        {
            appBuilder.UseCors(CorsOptions.AllowAll);
            HttpConfiguration configuration = new HttpConfiguration();
            configuration.MapHttpAttributeRoutes();
            appBuilder.UseWebApi(configuration);
        }
    }
}
