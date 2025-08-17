using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Akka.Actor;
using SlotGamesNode.GameLogics;

namespace SlotGamesNode
{
    class JackpotPoolConfig
    {
        private static JackpotPoolConfig _sInstance = new JackpotPoolConfig();
        public static JackpotPoolConfig Instance => _sInstance;
        public IActorRef PoolActor { get; set; }

        public JackpotPoolConfig()
        {
        }
    }
}
