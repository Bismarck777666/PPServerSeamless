using System;
using Topshelf;
using Topshelf.HostConfigurators;
using Topshelf.Runtime;
using Topshelf.ServiceConfigurators;

namespace FrontNode
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            HostFactory.Run(x =>
            {
                x.Service<FrontService>(serviceConfig =>
                {
                    serviceConfig.ConstructUsing(name => new FrontService());
                    serviceConfig.WhenStarted(os => os.Start());
                    serviceConfig.WhenStopped(os => os.StopAsync().Wait());
                });

                x.RunAsLocalSystem();
                x.SetDescription("FrontNode for GIT IGaming Solution");
                x.SetServiceName("FrontNode");
                x.SetDisplayName("FrontNode");
                x.StartAutomatically();
            });
        }
    }
}
