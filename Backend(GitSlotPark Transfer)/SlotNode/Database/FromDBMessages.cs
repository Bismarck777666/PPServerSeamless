using GITProtocol;

namespace SlotGamesNode.Database
{
    public class PayoutConfigUpdated
    {
        public PayoutConfigUpdated(GAMEID gameID)
        {
            GameID = gameID;
        }
        public GAMEID GameID { get; private set; }
    }
}
