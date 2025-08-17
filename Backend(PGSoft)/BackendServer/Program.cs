using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Topshelf;

namespace SlotGamesNode
{
    class Program
    {
        static void Main(string[] args)
        {
            HostFactory.Run(x => {

                x.Service<CommService>(serviceConfig => {
                    serviceConfig.ConstructUsing(name => new CommService());
                    serviceConfig.WhenStarted(os => os.Start());
                    serviceConfig.WhenStopped(os => os.StopAsync().Wait());
                });

                x.RunAsLocalSystem();

                x.SetDescription("BackendNode for GIT IGaming Solution");
                x.SetServiceName("BackendNode");
                x.SetDisplayName("BackendNode");
                x.StartAutomatically();
            });
        }
    }
}
