using System;
using Topshelf;
using Topshelf.HostConfigurators;
using Topshelf.Runtime;
using Topshelf.ServiceConfigurators;

namespace QueenApiNode
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            HostFactory.Run(x =>
            {
                x.Service<APIService>(serviceConfig =>
                {
                    serviceConfig.ConstructUsing(name => new APIService());
                    serviceConfig.WhenStarted(os => os.Start());
                    serviceConfig.WhenStopped(os => os.StopAsync().Wait());
                });

                x.RunAsLocalSystem();
                
                x.SetDescription("QueenApiNode Service for GodGaming Solution");
                x.SetServiceName("QueenApiNode");
                x.SetDisplayName("QueenApiNode");
                x.StartAutomatically();
            });
        }
    }
}
