using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FrontNode
{
    public class Constants
    {
        public static readonly string   CheckConnectionCommand  = "checkConnections";
        public static readonly TimeSpan HeartbeatTimeout        = TimeSpan.FromSeconds(300);
    }

    public class Command
    {
        public static readonly string LOGIN = "login";
        public static readonly string SUBSCRIBE = "subscribe";
        public static readonly string SETTINGS = "settings";
        public static readonly string PING = "ping";
        public static readonly string BET = "bet";
    }

    public class GameCommand
    {
        public static readonly string BET = "bet";
        public static readonly string COLLECT = "collect";
        public static readonly string GAMBLE = "gamble";
    }
}
