using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Akka.Actor;
using Akka.Configuration;
using System.IO;

namespace PPPromoBot
{
    public class PPPromoBotHostFactory
    {
        public static ActorSystem LaunchPPPromoBot(Config clusterConfig)
        {
            //首先从配置文件中获取参与者系统的名称。
            string systemName = "gitigamingbot";
            var connectConfig = clusterConfig.GetConfig("pppromobot");
            if (connectConfig != null)
            {
                systemName = connectConfig.GetString("actorsystem", systemName);
            }

            //创建参与者系统。
            return ActorSystem.Create(systemName, clusterConfig);
        }
    }
}
