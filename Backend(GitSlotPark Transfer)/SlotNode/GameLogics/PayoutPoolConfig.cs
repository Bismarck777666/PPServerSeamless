using Akka.Actor;
using System.Collections.Generic;

namespace SlotGamesNode.GameLogics
{
    internal class PayoutPoolConfig
    {
        private static  PayoutPoolConfig _sInstance = new PayoutPoolConfig();
        public static   PayoutPoolConfig Instance => _sInstance;
        public IActorRef PoolActor { get; set; }
        public Dictionary<int, double> WebsitePayoutRedundency { get; set; }
        public PayoutPoolConfig()
        {
            WebsitePayoutRedundency = new Dictionary<int, double>();
        }
    }
}
