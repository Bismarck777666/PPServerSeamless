using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Topshelf;

namespace PPPromoBot
{
    class Program
    {
        static void Main(string[] args)
        {
            HostFactory.Run(x => {
                x.Service<PPPromoBotService>(serviceConfig => {
                    serviceConfig.ConstructUsing(name => new PPPromoBotService());
                    serviceConfig.WhenStarted(os => os.Start());
                    serviceConfig.WhenStopped(os => os.StopAsync().Wait());
                });

                x.RunAsLocalSystem();
                x.SetDescription("PPPromoBot Service for GIT IGaming Solution");
                x.SetServiceName("PPPromoBot");
                x.SetDisplayName("PPPromoBot");
                x.StartAutomatically();
            });
        }
    }
}
