using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Topshelf;

namespace ApiIntegration
{
    class Program
    {
        static void Main(string[] args)
        {
            HostFactory.Run(x => {

                x.Service<ApiInteService>(serviceConfig => {
                    serviceConfig.ConstructUsing(name => new ApiInteService());
                    serviceConfig.WhenStarted(os => os.Start());
                    serviceConfig.WhenStopped(os => os.StopAsync().Wait());
                });

                x.RunAsLocalSystem();

                x.SetDescription("Greentube ApiIntegration for GIT IGaming Solution");
                x.SetServiceName("Greentube ApiIntegration");
                x.SetDisplayName("Greentube ApiIntegration");
                x.StartAutomatically();
            });
        }
        

    }
}
