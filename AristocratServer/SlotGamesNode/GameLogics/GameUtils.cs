using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PCGSharp;

namespace SlotGamesNode.GameLogics
{
    public class GameUtils
    {
        private static readonly DateTime UnixEpoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        public static long GetCurrentUnixTimestampMillis()
        {
            return (long)(DateTime.UtcNow - UnixEpoch).TotalMilliseconds;
        }
    }
}
