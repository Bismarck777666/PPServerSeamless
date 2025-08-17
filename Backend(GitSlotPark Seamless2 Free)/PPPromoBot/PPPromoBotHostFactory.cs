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
            //먼저 설정파일에서 액터시스템의 이름을 얻는다.
            string systemName = "gitigamingbot";
            var connectConfig = clusterConfig.GetConfig("pppromobot");
            if (connectConfig != null)
            {
                systemName = connectConfig.GetString("actorsystem", systemName);
            }

            //액터시스템을 창조한다.
            return ActorSystem.Create(systemName, clusterConfig);
        }
    }
}
