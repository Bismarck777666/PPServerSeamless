using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Akka.Actor;
using Akka.Configuration;
using System.IO;

namespace PromoNode
{
    public class PromoHostFactory
    {
        public static ActorSystem LaunchSlotGamesNode(Config clusterConfig)
        {
            string systemName = "gitigaming";
            var connectConfig = clusterConfig.GetConfig("promo");
            if (connectConfig != null)
            {
                systemName = connectConfig.GetString("actorsystem", systemName);
            }

            return ActorSystem.Create(systemName, clusterConfig);
        }
    }
}
