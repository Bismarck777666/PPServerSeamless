﻿using System;
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
                x.Service<SlotGamesService>(serviceConfig => {
                    serviceConfig.ConstructUsing(name => new SlotGamesService());
                    serviceConfig.WhenStarted(os => os.Start());
                    serviceConfig.WhenStopped(os => os.StopAsync().Wait());
                });

                x.RunAsLocalSystem();
                x.SetDescription("Greentube SlotGamesNode Service for GIT IGaming Solution");
                x.SetServiceName("Greentube SlotGamesNode");
                x.SetDisplayName("Greentube SlotGamesNode");
                x.StartAutomatically();
            });
        }
    }
}
