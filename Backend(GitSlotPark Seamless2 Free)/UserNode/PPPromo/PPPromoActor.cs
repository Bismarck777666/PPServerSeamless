using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Akka.Actor;
using Akka.Event;
using UserNode.Database;
using GITProtocol;
using Newtonsoft.Json.Linq;

namespace UserNode.PPPromo
{
    public class PPPromoActor : ReceiveActor
    {
        protected string                    _activePromos               = "";
        protected string                    _tournamentDetails          = "";
        protected string                    _raceDetails                = "";
        protected string                    _racePrizes                 = "";
        protected string                    _raceWinners                = "";
        protected string                    _tournamentLeaderboard      = "";
        protected int                       _activeTournamentID         = 0;
        protected double                    _activeTournamentMinBet     = 0.0;
        protected string                    _miniLobbyGames             = "";
        private readonly ILoggingAdapter    _logger                     = Context.GetLogger();

        public PPPromoActor()
        {
            ReceiveAsync<string>        (onCommand);
            Receive<PPPromoGetStatus>   (onPPPromoGetStatus);
        }

        protected async Task onCommand(string strCommand)
        {
            if(strCommand == "start")
            {
                Self.Tell("tick");
            }
            else if(strCommand == "tick")
            {
                await onTick();
                Context.System.Scheduler.ScheduleTellOnce(TimeSpan.FromSeconds(60.0), Self, "tick", Self);
            }
        }

        protected async Task onTick()
        {
            try
            {
                _activePromos           = await RedisDatabase.RedisCache.StringGetAsync("ActivePromos");
                _tournamentDetails      = await RedisDatabase.RedisCache.StringGetAsync("TournamentDetails");
                _raceDetails            = await RedisDatabase.RedisCache.StringGetAsync("RaceDetails");
                _racePrizes             = await RedisDatabase.RedisCache.StringGetAsync("RacePrizes");
                _raceWinners            = await RedisDatabase.RedisCache.StringGetAsync("RaceWinners");
                _tournamentLeaderboard  = await RedisDatabase.RedisCache.StringGetAsync("TournamentLeaderboard");
                _miniLobbyGames         = await RedisDatabase.RedisCache.StringGetAsync("PPMiniLobby");
                
                Context.System.EventStream.Publish(new PPPromoStatus(_activePromos, _tournamentDetails, _raceDetails, _raceWinners, _racePrizes, _activeTournamentID, _activeTournamentMinBet, _tournamentLeaderboard, _miniLobbyGames));
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occurred in PPPromoActor::onTick {0}", ex);
            }
        }

        private void onPPPromoGetStatus(PPPromoGetStatus _)
        { 
            Sender.Tell(new PPPromoStatus(_activePromos, _tournamentDetails, _raceDetails, _raceWinners, _racePrizes, _activeTournamentID, _activeTournamentMinBet, _tournamentLeaderboard, _miniLobbyGames));
        } 

        private List<int> getActiveTournamentIDs()
        {
            try
            {
                _activeTournamentID = 0;
                List<int>   activeTournamentIds = new List<int>();
                JArray      tournamentArray     = JToken.Parse(_activePromos)["tournaments"] as JArray;
                for (int i = 0; i < tournamentArray.Count; i++)
                {
                    if ((string)tournamentArray[i]["status"] == "O")
                        _activeTournamentID = (int)tournamentArray[i]["id"];

                    if ((string)tournamentArray[i]["status"] != "S")
                        activeTournamentIds.Add((int)tournamentArray[i]["id"]);
                }

                _activeTournamentMinBet         = 1000.0;
                JArray tournamentDetailArray    = JToken.Parse(_tournamentDetails)["details"] as JArray;
                for (int i = 0; i < tournamentDetailArray.Count; i++)
                {
                    if ((int)tournamentDetailArray[i]["id"] == _activeTournamentID)
                    {
                        _activeTournamentMinBet = (double)tournamentDetailArray[i]["prizePool"]["minBetLimit"] / 100.0;
                        break;
                    }
                }

                return activeTournamentIds;
            }
            catch
            {
                return new List<int>();
            }
        }
    }

    public class PPPromoGetStatus
    {
    }
}
