using PCGSharp;
using System;

namespace SlotGamesNode.GameLogics
{
    public class GameUtils
    {
        private static readonly DateTime UnixEpoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        public static int selectFromProbs(int[] probs)
        {
            int maxExclusive = 0;
            for (int index = 0; index < probs.Length; ++index)
                maxExclusive += probs[index];
            int num1 = Pcg.Default.Next(0, maxExclusive);
            int num2 = 0;
            for (int index = 0; index < probs.Length; ++index)
            {
                num2 += probs[index];
                if (num1 < num2)
                    return index;
            }
            return 0;
        }

        public static long GetCurrentUnixTimestampMillis()
        {
            return (long)(DateTime.UtcNow - GameUtils.UnixEpoch).TotalMilliseconds;
        }

        public static long GetCurrentUnixTimestamp()
        {
            return (long)(DateTime.UtcNow - GameUtils.UnixEpoch).TotalSeconds;
        }
    }
}
