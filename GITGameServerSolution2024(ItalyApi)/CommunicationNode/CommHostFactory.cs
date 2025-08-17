using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Akka.Actor;
using Akka.Configuration;
using System.IO;

namespace CommNode
{
    public class CommHostFactory
    {
        public static ActorSystem LauchCommNode(Config clusterConfig)
        {
            //먼저 설정파일에서 액터시스템의 이름을 얻는다.
            string systemName = "gitigaming";
            var connectConfig = clusterConfig.GetConfig("comm");
            if (connectConfig != null)
            {
                systemName = connectConfig.GetString("actorsystem", systemName);
            }

            //액터시스템을 창조한다.
            return ActorSystem.Create(systemName, clusterConfig);
        }

    }
}
