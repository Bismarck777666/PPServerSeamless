
namespace QueenApiNode
{
    public class ApiWithdrawRequest
    {
        public int      AgentID { get; private set; }
        public string   UserID  { get; private set; }
        public double   Amount  { get; private set; }

        public ApiWithdrawRequest(int agentID, string strUserID, double amount)
        {
            AgentID = agentID;
            UserID  = strUserID;
            Amount  = amount;
        }
    }

    public class ApiWithdrawResponse
    {
        public int      Result      { get; private set; }
        public double   BeforeScore { get; private set; }
        public double   AfterScore  { get; private set; }

        public ApiWithdrawResponse(int result, double beforeScore, double afterScore)
        {
            Result      = result;
            BeforeScore = beforeScore;
            AfterScore  = afterScore;
        }
    }
}
