using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Topshelf;

namespace FrontNode
{
    class Program
    {
        static void Main(string[] args)
        {
            HostFactory.Run(x => {

                x.Service<FrontService>(serviceConfig => {
                    serviceConfig.ConstructUsing(name => new FrontService());
                    serviceConfig.WhenStarted(os => os.Start());
                    serviceConfig.WhenStopped(os => os.StopAsync().Wait());
                });

                x.RunAsLocalSystem();

                x.SetDescription("EGT FrontNode for GIT IGaming Solution");
                x.SetServiceName("EGT FrontNode");
                x.SetDisplayName("EGT FrontNode");
                x.StartAutomatically();
            });
        }
        

    }
}
