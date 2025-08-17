namespace SlotGamesNode.GameLogics
{
    public class GameConfig
    {
        public double PayoutRate { get; set; }

        public GameConfig(double payoutRate)
        {
            PayoutRate = payoutRate;
        }
    }
}
