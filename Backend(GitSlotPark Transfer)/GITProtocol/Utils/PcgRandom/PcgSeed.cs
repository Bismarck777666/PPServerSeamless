using System;

namespace PCGSharp
{
    public static class PcgSeed
    {
        /// <summary>
        /// Provides a time-dependent seed value, matching the default behavior of System.Random.
        /// </summary>
        public static ulong TimeBasedSeed()
        {
            return (ulong)(Environment.TickCount);
        }

        /// <summary>
        /// Provides a seed based on time and unique GUIDs.
        /// </summary>
        public static ulong GuidBasedSeed()
        {
            ulong upper = (ulong)(Environment.TickCount ^ Guid.NewGuid().GetHashCode()) << 32;
            ulong lower = (ulong)(Environment.TickCount ^ Guid.NewGuid().GetHashCode());
            return (upper | lower);
        }
    }
}
