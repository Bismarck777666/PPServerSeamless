using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GITProtocol;
using Newtonsoft.Json;
using CommNode.Database;
using StackExchange.Redis;
using System.Diagnostics;

namespace CommNode.PPPromo
{
    public class PPPromoSnapshot
    {
        private static  PPPromoSnapshot     _sInstance      = new PPPromoSnapshot();
        public static   PPPromoSnapshot     Instance        => _sInstance;

        private const int                   MAX_RACEWINNERHISTORY   = 10;


        public Dictionary<string, double>               CurrencyMap             { get; set; }
        public List<PPTournament> TournamentList { get; set; } = new List<PPTournament>();
        public List<PPRace>                             RaceList                { get; set; }
        public List<PPCashback>                         CashbackList            { get; set; }
        public PPActivePromos                           ActivePromos            { get; set; }
        public PPTournamentDetails                      ActiveTournamentDetails { get; set; }
        public PPRaceDetails                            ActiveRaceDetails       { get; set; }
        public PPRacePrizes                             ActiveRacePrizes        { get; set; }

        public List<PPFreespin> ActiveFreespins { get; set; } = new List<PPFreespin> { };

        private Dictionary<int, Dictionary<string, PPRaceWinnerInfo>>    _raceWinnersInfoHistory      { get; set; }
        private Dictionary<int, List<string>>                            _raceWinnersInfoIdentities   { get; set; }
        public Dictionary<int, Dictionary<string, string>>               DicRaceWinnerStrings         { get; set; }
        public Dictionary<int, string>                                   DicActiveRaceWinners         { get; set; }
        public List<int>                                                 ActiveTournamentIDs          { get; set; }
        public List<int>                                                 ActivePrizeDropIDs           { get; set; }
        public List<int>                                                 ActiveCashbackIDs            { get; set; }
        public int                                                       OpenTournamentID             { get; set; }
        public double                                                    OpenTournamentMinBet         { get; set; }

        public void generateActivePromos()
        {                   
            PPActivePromos activePromos = new PPActivePromos();
            activePromos.error          = 0;
            activePromos.description    = "OK";
            activePromos.serverTime     = (int) (DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalSeconds);
            activePromos.races          = new List<PPActiveRace>();

            ActivePrizeDropIDs = new List<int>();
            ActiveCashbackIDs = new List<int>();
            ActiveTournamentIDs         = new List<int>();
            OpenTournamentID            = 0;

            if (RaceList != null)
            {
                for (int i = 0; i < RaceList.Count; i++)
                {
                    activePromos.races.Add(new PPActiveRace(RaceList[i]));
                    ActivePrizeDropIDs.Add(RaceList[i].id);
                }
            }

            if(CashbackList != null)
            {
                for(int i = 0; i < CashbackList.Count;i++)
                {
                    activePromos.races.Add(new PPActiveRace(CashbackList[i]));
                    ActiveCashbackIDs.Add(CashbackList[i].id);
                }
            }

            activePromos.tournaments = new List<PPActiveTournament>();
            if (TournamentList != null)
            {
                for (int i = 0; i < TournamentList.Count; i++)
                {
                    activePromos.tournaments.Add(new PPActiveTournament(TournamentList[i]));

                    if (TournamentList[i].status == "O")
                    {
                        OpenTournamentID     = TournamentList[i].id;
                        OpenTournamentMinBet = TournamentList[i].prizePool.minBetLimit;
                    }
                    ActiveTournamentIDs.Add(TournamentList[i].id);
                }
            }
            this.ActivePromos = activePromos;
        }
        public void generateTournamentDetails()
        {
            PPTournamentDetails tournamentDetails = new PPTournamentDetails();
            tournamentDetails.error         = 0;
            tournamentDetails.description   = "OK";
            tournamentDetails.details = new List<PPTournamentDetail>();

            if(TournamentList != null)
            {
                for(int i = 0; i < TournamentList.Count; i++)
                    tournamentDetails.details.Add(new PPTournamentDetail(TournamentList[i], CurrencyMap));
            }
            ActiveTournamentDetails = tournamentDetails;
        }
        public void generateRaceDetails()
        {
            PPRaceDetails raceDetails = new PPRaceDetails();
            raceDetails.error = 0;
            raceDetails.description = "OK";
            raceDetails.details = new List<PPRaceDetail>();

            if(RaceList != null)
            {
                for (int i = 0; i < RaceList.Count; i++)
                    raceDetails.details.Add(
                    new PPRaceDetail()
                    {
                        id = RaceList[i].id, htmlRules = RaceList[i].htmlRules,
                        shortHtmlRules = RaceList[i].shortHtmlRules,
                        prizePool = RaceList[i].prizePool, currencyRateMap = CurrencyMap
                    });
            }
            if(CashbackList != null)
            {
                for (int i = 0; i < CashbackList.Count; i++)
                    raceDetails.details.Add(
                    new PPRaceDetail()
                    {
                        id = CashbackList[i].id,
                        htmlRules = CashbackList[i].htmlRules,
                        shortHtmlRules = CashbackList[i].shortHtmlRules,
                        prizePool = CashbackList[i].prizePool                        
                    });
            }
            ActiveRaceDetails = raceDetails;
        }       

        private void calculateHistory(int raceID, PPRaceWinnerInfo currentWinnersInfo)
        {
            DicRaceWinnerStrings[raceID] = new Dictionary<string, string>();
            foreach(KeyValuePair<string, PPRaceWinnerInfo> pair in _raceWinnersInfoHistory[raceID])
            {
                PPRaceWinnerInfo lastWinnersInfo = pair.Value;
                DicRaceWinnerStrings[raceID][lastWinnersInfo.lastIdentity] = calculateWinnersDiff(lastWinnersInfo, currentWinnersInfo);
            }
        }
        private string calculateWinnersDiff(PPRaceWinnerInfo lastWinnerInfo, PPRaceWinnerInfo currentWinnerInfo)
        {
            PPRaceWinnerInfo winnerInfo = new PPRaceWinnerInfo();
            winnerInfo.lastIdentity     = currentWinnerInfo.lastIdentity;
            winnerInfo.raceID           = currentWinnerInfo.raceID;
            winnerInfo.action           = "A";
            winnerInfo.items            = new List<PPRaceWinner>();

            if(lastWinnerInfo.lastIdentity != currentWinnerInfo.lastIdentity)
            {
                foreach (PPRaceWinner winner in currentWinnerInfo.items)
                {
                    if (!lastWinnerInfo.dicItems.ContainsKey(winner.id))
                        winnerInfo.items.Add(winner);
                }
            }
            return JsonConvert.SerializeObject(winnerInfo);
        }
        public void generateRaceWinners()
        {
            PPRacePrizes raceRemainPrizes   = new PPRacePrizes();
            raceRemainPrizes.error          = 0;
            raceRemainPrizes.description    = "OK";
            raceRemainPrizes.prizes         = new List<PPRaceRemainPrize>();

            if (this.RaceList != null)
            {
                for(int i = 0; i < RaceList.Count; i++)
                {

                    int raceID = RaceList[i].id;

                    PPRaceWinnerInfo winnerInfo = new PPRaceWinnerInfo();
                    winnerInfo.action       = "R";
                    winnerInfo.raceID       = RaceList[i].id;
                    winnerInfo.lastIdentity = RaceList[i].lbGuid;
                    winnerInfo.items        = new List<PPRaceWinner>(RaceList[i].winners);
                    if (RaceList[i].dicWinners != null)
                        winnerInfo.dicItems = new Dictionary<long, bool>(RaceList[i].dicWinners);
                    else
                        winnerInfo.dicItems = new Dictionary<long, bool>();

                    if (RaceList[i].status == "C")
                        continue;

                    Dictionary<int, int> wonPrizeCount = new Dictionary<int, int>();
                    foreach(PPRaceWinner raceWinner in winnerInfo.items)
                    {
                        if (!wonPrizeCount.ContainsKey(raceWinner.prizeID))
                            wonPrizeCount[raceWinner.prizeID] = 1;
                        else
                            wonPrizeCount[raceWinner.prizeID]++;

                        //raceWinner.playerID = obfusticateUserID(raceWinner.playerID);
                    }

                    if (!_raceWinnersInfoHistory.ContainsKey(raceID))
                    {
                        _raceWinnersInfoHistory.Add(raceID, new Dictionary<string, PPRaceWinnerInfo>());
                        _raceWinnersInfoIdentities.Add(raceID, new List<string>());
                    }

                    if(!_raceWinnersInfoHistory[raceID].ContainsKey(winnerInfo.lastIdentity))
                    {
                        _raceWinnersInfoHistory[raceID].Add(winnerInfo.lastIdentity, winnerInfo);
                        _raceWinnersInfoIdentities[raceID].Add(winnerInfo.lastIdentity);

                        if (_raceWinnersInfoIdentities[raceID].Count > MAX_RACEWINNERHISTORY)
                        {
                            string guid = _raceWinnersInfoIdentities[raceID][0];
                            _raceWinnersInfoHistory[raceID].Remove(guid);
                            _raceWinnersInfoIdentities[raceID].RemoveAt(0);
                        }
                        calculateHistory(raceID, winnerInfo);
                    }

                    DicActiveRaceWinners[raceID]            = JsonConvert.SerializeObject(winnerInfo);


                    PPRaceRemainPrize remainPrizes          = new PPRaceRemainPrize();
                    remainPrizes.id                         = RaceList[i].id;
                    remainPrizes.currency                   = RaceList[i].prizePool.currency;
                    remainPrizes.maxBetLimitByMultiplier    = RaceList[i].prizePool.maxBetLimitByMultiplier;
                    remainPrizes.prizeRemains               = new List<PPRacePrize>();

                    for (int j = 0; j < RaceList[i].prizePool.prizesList.Count; j++)
                    {
                        PPRacePrize remainPrize = new PPRacePrize();
                        remainPrize.prizeID             = RaceList[i].prizePool.prizesList[j].prizeID;
                        remainPrize.count               = RaceList[i].prizePool.prizesList[j].count;
                        if (wonPrizeCount.ContainsKey(remainPrize.prizeID))
                            remainPrize.count -= wonPrizeCount[remainPrize.prizeID];

                        remainPrize.type                = RaceList[i].prizePool.prizesList[j].type;
                        remainPrize.betMultiplier       = RaceList[i].prizePool.prizesList[j].betMultiplier;
                        remainPrize.amount              = RaceList[i].prizePool.prizesList[j].amount;

                        if (remainPrize.count > 0)
                            remainPrizes.prizeRemains.Add(remainPrize);
                    }
                    raceRemainPrizes.prizes.Add(remainPrizes);
                }
            }
            ActiveRacePrizes  = raceRemainPrizes;
        }
        public static string obfusticateUserID(string strUserID)
        {
            int realPartLength = 4;
            if (strUserID.Length <= 4)
                realPartLength = 2;

            if (realPartLength > strUserID.Length)
                realPartLength = strUserID.Length;

            string strObfusticated = "*****";
            for (int i = 0; i < 4 - realPartLength; i++)
                strObfusticated += "*";

            strObfusticated += strUserID.Substring(strUserID.Length - realPartLength);
            return strObfusticated;
        }

        public PPPromoSnapshot()
        {
            _raceWinnersInfoHistory      = new Dictionary<int, Dictionary<string, PPRaceWinnerInfo>>();
            _raceWinnersInfoIdentities   = new Dictionary<int, List<string>>();

            DicActiveRaceWinners         = new Dictionary<int, string>();
            DicRaceWinnerStrings         = new Dictionary<int, Dictionary<string, string>>();
        }
    }

    public class PPActivePromos
    {
        public int                          error       { get; set; }
        public string                       description { get; set; }
        public int                          serverTime  { get; set; }
        public List<PPActiveRace>           races       { get; set; }
        public List<PPActiveTournament>     tournaments { get; set; }
    }
    public class PPActiveTournament
    {
        public int              id              { get; set; }
        public string           name            { get; set; }
        public List<string>     optJurisdiction { get; set; }
        public bool             optin           { get; set; }
        public int              startDate       { get; set; }
        public int              endDate         { get; set; }
        public string           status          { get; set; }
        public string           clientMode      { get; set; }
        [JsonIgnore]
        public string           agentid         { get; set; }
        [JsonIgnore]
        public int              type            { get; set; }
        [JsonIgnore]
        public int              currency        { get; set; }
        [JsonIgnore]
        public int              prizeslimit     { get; set; }
        [JsonIgnore]
        public string           games           { get; set; }
        [JsonIgnore]
        public string           playersinc      { get; set; }
        [JsonIgnore]
        public string           playersexc      { get; set; }
        

        public PPActiveTournament(PPTournament tournament)
        {
            this.id                 = tournament.id;
            this.name               = tournament.name;
            this.optJurisdiction    = tournament.optJurisdiction;
            this.optin              = true;
            this.startDate          = tournament.startDate;
            this.endDate            = tournament.endDate;
            this.status             = tournament.status;
            this.clientMode         = tournament.clientMode;
            this.agentid            = tournament.agentid;
            this.type               = tournament.type;
            this.currency           = tournament.currency;
            this.games              = tournament.games;
            this.playersinc         = tournament.playersinc;
            this.playersexc         = tournament.playersexc;
        }
        public PPActiveTournament()
        {

        }
    }
    public class PPActiveRace
    {
        public int id { get; set; }
        public string name { get; set; }
        public bool optin { get; set; }
        public bool showWinnersList { get; set; }
        public int startDate { get; set; }
        public int endDate { get; set; }
        public string clientMode { get; set; }
        public string clientStyle { get; set; }
        public string status { get; set; }

        [JsonIgnore]
        public string agentid { get; set; }
        [JsonIgnore]
        public int prizeslimit { get; set; }
        [JsonIgnore]
        public string games { get; set; }
        [JsonIgnore]
        public string playersinc { get; set; }
        [JsonIgnore]
        public string playersexc { get; set; }

        [JsonIgnore]
        public int type { get; set; } // 0: PrizeDrop, 1: Cashback

        [JsonIgnore]
        public int cType { get; set; }          // 0: TotalBet - TotalWin, 1: TotalWin - TotalBet

        [JsonIgnore]
        public int period { get; set; }         // 0: Daily, 1: Weekly, 2: Monthly  

        [JsonIgnore]
        public double minNet { get; set; }

        [JsonIgnore]
        public double cashback { get; set; }

        [JsonIgnore]
        public int rounds { get; set; }



        public PPActiveRace(PPRace race)
        {
            this.id = race.id;
            this.name = race.name;
            this.optin = race.optin;
            this.showWinnersList = race.showWinnersList;
            this.startDate = race.startDate;
            this.endDate = race.endDate;
            this.clientMode = race.clientMode;
            this.clientStyle = race.clientStyle;
            this.status = race.status;
            this.agentid = race.agentid;
            this.prizeslimit = race.prizesLimit;
            this.games = race.games;
            this.playersinc = race.playersinc;
            this.playersexc = race.playersexc;
            this.type = race.type;
        }

        public PPActiveRace()
        {

        }

        public PPActiveRace(PPCashback cashback)
        {
            this.id = cashback.id;
            this.name = cashback.name;
            this.optin = cashback.optin;
            this.showWinnersList = cashback.showWinnersList;
            this.startDate = cashback.startDate;
            this.endDate = cashback.endDate;
            this.clientMode = cashback.clientMode;
            this.clientStyle = cashback.clientStyle;
            this.status = cashback.status;
            this.agentid = cashback.agentid;
            this.prizeslimit = cashback.prizesLimit;
            this.games = cashback.games;
            this.playersinc = cashback.playersinc;
            this.playersexc = cashback.playersexc;
            this.type = cashback.type;
            this.cType = cashback.cType;
            this.period = cashback.period;
            this.minNet = cashback.minNet;
            this.cashback = cashback.cashback;
            this.rounds = cashback.rounds;
        }
    }
    public class PPTournamentDetails
    {
        public int                      error       { get; set; }
        public string                   description { get; set; }
        public List<PPTournamentDetail> details     { get; set; }
    }
    public class PPTournamentDetail
    {
        public int                          id              { get; set; }
        public Dictionary<string, double>   currencyRateMap { get; set; }
        public string                       htmlRules       { get; set; }
        public string                       shortHtmlRules  { get; set; }
        public PPTournamentPrizePool        prizePool       { get; set; }

        public PPTournamentDetail(PPTournament tournament, Dictionary<string, double> currencyRateMap)
        {
            this.id             = tournament.id;
            this.htmlRules      = tournament.htmlRules;
            this.shortHtmlRules = tournament.shortHtmlRules;
            this.prizePool      = tournament.prizePool;
            this.currencyRateMap = currencyRateMap;
        }
    }
    public class PPRaceDetails
    {
        public int                  error       { get; set; }
        public string               description { get; set; }
        public List<PPRaceDetail>   details     { get; set; }
    }
    public class PPRaceDetail
    {
        public int                           id              { get; set; }
        public string                        htmlRules       { get; set; }
        public string                        shortHtmlRules  { get; set; }
        public PPRacePrizePool               prizePool       { get; set; }
        public Dictionary<string, double>    currencyRateMap { get; set; }

        //public PPRaceDetail(PPRace race, Dictionary<string, double> currencyMap)
        //{
        //    this.id              = race.id;
        //    this.htmlRules       = race.htmlRules;
        //    this.shortHtmlRules  = race.shortHtmlRules;
        //    this.prizePool       = race.prizePool;
        //    this.currencyRateMap = currencyMap;

        //}
    }
    public class PPRaceWinnerInfos
    {
        public int                    error         { get; set; }
        public string                 description   { get; set; }
        public List<PPRaceWinnerInfo> winners       { get; set; }
    }
    public class PPRaceWinnerInfo
    {
        public int                      raceID       { get; set; }
        public string                   action       { get; set; }
        public List<PPRaceWinner>       items        { get; set; }

        [JsonIgnore]
        public Dictionary<long, bool>   dicItems     { get; set; }  
        public string                   lastIdentity { get; set; }
    }
    public class PPRaceWinnerInfoV2
    {
        public int raceID { get; set; }
        public string action { get; set; }
        public List<string> items { get; set; }
        public string lastIdentity { get; set; }
    }
    public class PPRacePrizes
    {
        public int      error       { get; set; }
        public string   description { get; set; }
        public List<PPRaceRemainPrize> prizes { get; set; }
    }
    public class PPRaceRemainPrize
    {
        public int                          id                          { get; set; }
        public string                       currency                    { get; set; }
        public double                       maxBetLimitByMultiplier     { get; set; }
        public List<PPRacePrize>            prizeRemains                { get; set; }
    }
    public class PPTournamentLeaderboards
    {
        public int      error                               { get; set; }
        public string   description                         { get; set; }
        public List<PPTournamentLeaderboard> leaderboards   { get; set; }
    }
    public class PPTournamentLeaderboardsV3
    {
        public int error { get; set; }
        public string description { get; set; }
        public List<PPTournamentLeaderboardV3> leaderboards { get; set; }
    }
    public class PPTournamentLeaderboard
    {
        public int tournamentID                     { get; set; }
        public int index                            { get; set; }
        public List<PPTournamentLeaderItem> items   { get; set; }       
    }
    public class PPTournamentLeaderboardV3
    {
        public int          tournamentID    { get; set; }
        public int          index           { get; set; }
        public List<string> items           { get; set; }
    }
}
