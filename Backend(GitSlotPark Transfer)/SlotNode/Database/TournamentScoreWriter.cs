using Akka.Actor;
using Akka.Routing;
using GITProtocol;
using GITProtocol.Utils;
using Newtonsoft.Json.Linq;
using StackExchange.Redis;
using System;
using System.Threading.Tasks;

namespace SlotGamesNode.Database
{
    public class TournamentScoreWriter : ReceiveActor
    {
        private int                     _activeTourID   = 0;
        private double                  _tourMinBet     = 0.0;
        private RealExtensions.Epsilon  _epsilion       = new RealExtensions.Epsilon(0.001);

        public TournamentScoreWriter()
        {
            ReceiveAsync<string>        (onCommand);
            ReceiveAsync<TourScoreWrite>(onScoreWrite);
        }

        private async Task onCommand(string strCommand)
        {
            if (strCommand == "start")
            {
                Self.Tell("tick");
            }
            else
            {
                try
                {
                    string strActivePromos = await RedisDatabase.RedisCache.StringGetAsync("ActivePromos");
                    if (string.IsNullOrEmpty(strActivePromos))
                        return;

                    _activeTourID = 0;
                    JToken activePromos = JToken.Parse(strActivePromos);
                    JArray tournaments  = activePromos["tournaments"] as JArray;
                    for (int i = 0; i < tournaments.Count; i++)
                    {
                        if ((string)tournaments[i]["status"] == "O")
                            _activeTourID = (int)tournaments[i]["id"];
                    }

                    if (_activeTourID == 0)
                        return;

                    string strTourDetails = await RedisDatabase.RedisCache.StringGetAsync("TournamentDetails");
                    if (string.IsNullOrEmpty(strTourDetails))
                        return;

                    JToken tourDetails = JToken.Parse(strTourDetails);
                    tournaments = tourDetails["details"] as JArray;

                    _tourMinBet = 0.0;
                    for (int i = 0; i < tournaments.Count; i++)
                    {
                        if ((int)tournaments[i]["id"] == _activeTourID)
                            _tourMinBet = (double)tournaments[i]["prizePool"]["minBetLimit"] / 100.0;
                    }
                }
                catch
                {
                }
                finally
                {
                    Context.System.Scheduler.ScheduleTellOnce(TimeSpan.FromSeconds(60.0), Self, "tick", Self);
                }
            }
        }

        private async Task onScoreWrite(TourScoreWrite write)
        {
            try
            {
                if (_activeTourID == 0 || _tourMinBet == 0.0 || write.BaseBet.LT(_tourMinBet, _epsilion))
                    return;

                string strKey           = string.Format("tournament_{0}_scores", _activeTourID);
                string strGlobalUserID  = string.Format("{0}_{1}", write.AgentID, write.UserID);
                
                long score = (long)(write.Win / write.BaseBet * 1000.0);
                if (score == 0L)
                    return;
                
                RedisValue value = await RedisDatabase.RedisCache.HashGetAsync(strKey, strGlobalUserID);
                if (value.IsNullOrEmpty || (long)value < score)
                {
                    await RedisDatabase.RedisCache.HashSetAsync(strKey, strGlobalUserID, score);
                }
            }
            catch
            {
            }
        }
    }

    public class TourScoreWrite : IConsistentHashable
    {
        public int      AgentID { get; private set; }
        public string   UserID  { get; private set; }
        public double   BaseBet { get; private set; }
        public double   Win     { get; private set; }
        public GAMEID   GameID  { get; private set; }

        public object ConsistentHashKey => string.Format("{0}_{1}", AgentID, UserID);

        public TourScoreWrite(int agentID, string userID, double baseBet, double win, GAMEID gameID)
        {
            AgentID     = agentID;
            UserID      = userID;
            BaseBet     = baseBet;
            Win         = win;
            GameID      = gameID;
        }
    }
}
