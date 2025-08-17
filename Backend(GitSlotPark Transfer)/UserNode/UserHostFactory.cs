using Akka.Actor;
using Akka.Configuration;

namespace UserNode
{
    public class UserHostFactory
    {
        public static ActorSystem LauchUserNode(Config clusterConfig)
        {
            string str      = "gitigaming";
            Config config   = clusterConfig.GetConfig("user");
            if (config != null)
                str = config.GetString("actorsystem", str);

            return ActorSystem.Create(str, clusterConfig);
        }
    }
}
