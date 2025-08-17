using System;
using Topshelf;
using Topshelf.HostConfigurators;
using Topshelf.Runtime;
using Topshelf.ServiceConfigurators;

namespace SlotGamesNode
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            HostFactory.Run(x =>
            {
                x.Service<SlotGamesService>(serviceConfig =>
                {
                    serviceConfig.ConstructUsing(name => new SlotGamesService());
                    serviceConfig.WhenStarted(os => os.Start());
                    serviceConfig.WhenStopped(os => os.StopAsync().Wait());
                });

                x.RunAsLocalSystem();
                x.SetDescription("SlotGamesNode Service for GIT IGaming Solution");
                x.SetServiceName("SlotGamesNode");
                x.SetDisplayName("SlotGamesNode");
                x.StartAutomatically();
            });
        }
    }
}
