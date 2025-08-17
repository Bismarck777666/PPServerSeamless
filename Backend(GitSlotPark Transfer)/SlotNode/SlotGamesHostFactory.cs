using Akka.Actor;
using Akka.Configuration;

namespace SlotGamesNode
{
    public class SlotGamesHostFactory
    {
        public static ActorSystem LaunchSlotGamesNode(Config clusterConfig)
        {
            string str = "gitigaming";
            Config config = clusterConfig.GetConfig("slotgames");
            if (config != null)
                str = config.GetString("actorsystem", str);
            
            return ActorSystem.Create(str, clusterConfig);
        }
    }
}
