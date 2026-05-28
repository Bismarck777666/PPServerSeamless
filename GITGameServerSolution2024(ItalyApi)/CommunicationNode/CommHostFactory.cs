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
            //首先从配置文件中获取参与者系统的名称。
            string systemName = "gitigaming";
            var connectConfig = clusterConfig.GetConfig("comm");
            if (connectConfig != null)
            {
                systemName = connectConfig.GetString("actorsystem", systemName);
            }

            //创建参与者系统。
            return ActorSystem.Create(systemName, clusterConfig);
        }

    }
}
