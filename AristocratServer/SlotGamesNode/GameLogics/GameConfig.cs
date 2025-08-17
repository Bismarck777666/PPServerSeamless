using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SlotGamesNode.GameLogics
{
    public class GameConfig
    {
        public double   PayoutRate          { get; set; }
        public double   EventRate           { get; set; }
        public double   PoolRedundency      { get; set; }
        public bool     HasRandomJackpot    { get; set; }
        public GameConfig(double payoutRate, double eventRate, double redundency, bool hasRandomJackpot)
        {
            this.PayoutRate = payoutRate;
            this.EventRate = eventRate;
            this.PoolRedundency = redundency;
            this.HasRandomJackpot = hasRandomJackpot;
        }
    }

}
