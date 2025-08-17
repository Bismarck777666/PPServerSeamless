using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GITProtocol;

namespace SlotGamesNode.Database
{
    public class PayoutConfigUpdated
    {
        public PayoutConfigUpdated(GAMEID gameID)
        {
            this.GameID = gameID;
        }
        public GAMEID GameID { get; private set; }
    }    
}
