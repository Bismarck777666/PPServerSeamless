using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Akka.Actor;
using Akka.Configuration;
using System.IO;

namespace SlotGamesNode
{
    public class SlotGamesHostFactory
    {
        public static ActorSystem LaunchSlotGamesNode(Config clusterConfig)
        {
            string systemName = "gitigaming";
            var connectConfig = clusterConfig.GetConfig("slotgames");
            if (connectConfig != null)
            {
                systemName = connectConfig.GetString("actorsystem", systemName);
            }

            return ActorSystem.Create(systemName, clusterConfig);
        }
    }
}
