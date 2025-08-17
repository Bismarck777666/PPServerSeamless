using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PPPromoBot
{
    public class PPActiveRace
    {
        public int      id                  { get; set; }
        public string   name                { get; set; }
        public bool     optin               { get; set; }
        public bool     showWinnersList     { get; set; }
        public int      startDate           { get; set; }
        public int      endDate             { get; set; }
        public string   clientMode          { get; set; }
        public string   clientStyle         { get; set; }
        public string   status              { get; set; }

    }

    public class PPActiveTournament
    {
        public int id   { get; set; }
        public string name { get; set; }
        public List<string> optJurisdiction { get; set; }
        public bool optin { get; set; }
        public int startDate { get; set; }
        public int endDate { get; set; }
        public string status { get; set; }
        public string clientMode { get; set; }        
    }

    public class PPActivePromos
    {
        public int                      error       { get; set; }
        public string                   description { get; set; }
        public int                      serverTime  { get; set; }
        public List<PPActiveRace>       races       { get; set; }
        public List<PPActiveTournament> tournaments { get; set; }
    }

    public class PPRaceWinner
    {
        public int    prizeID                       { get; set; }
        public string playerID                      { get; set; }
        public string countryID                     { get; set; }
        public string memberCurrency                { get; set; }
        public double bet                           { get; set; }
        public double effectiveBetForBetMultiplier  { get; set; }
        public double effectiveBetForFreeRounds     { get; set; }

    }

    public class PPTournamentDetails
    {
        public int error { get; set; }
        public string description { get; set; }
        public List<PPTournamentDetail> details { get; set; }
    }

    public class PPTournamentDetail
    {
        public int id { get; set; }
        public Dictionary<string, double> currencyRateMap { get; set; }
        public string htmlRules { get; set; }
        public string shortHtmlRules { get; set; }
        public PPTournamentPrizePool prizePool { get; set; }
        
    }

    public class PPTournamentPrizePool
    {
        public string currency { get; set; }
        public string currencyOriginal { get; set; }
        public double minBetLimit { get; set; }
        public double totalPrizeAmount { get; set; }
        public List<PPTournamentPrize> prizesList { get; set; }

    }
    public class PPTournamentPrize
    {
        public int placeFrom { get; set; }
        public int placeTo { get; set; }
        public long amount { get; set; }
        public string type { get; set; }
    }

    public class PPRaceDetails
    {
        public int error { get; set; }
        public string description { get; set; }
        public List<PPRaceDetail> details { get; set; }
    }
    public class PPRaceDetail
    {
        public int id { get; set; }
        public string htmlRules                             { get; set; }
        public string shortHtmlRules                        { get; set; }
        public PPRacePrizePool prizePool                    { get; set; }
        public Dictionary<string, double> currencyRateMap   { get; set; }
    }

    public class PPRacePrizePool
    {
        public string currency { get; set; }
        public string currencyOriginal { get; set; }
        public double maxBetLimitByMultiplier { get; set; }
        public double minBetLimit { get; set; }
        public List<PPRacePrize> prizesList { get; set; }
        public double totalPrizeAmount { get; set; }
    }
    public class PPRacePrize
    {
        public int prizeID { get; set; }
        public int count { get; set; }
        public string type { get; set; }
        public double betMultiplier { get; set; }
    }

    public class PPTournamentLeaderboards
    {
        public int error { get; set; }
        public string description { get; set; }
        public List<PPTournamentLeaderboard> leaderboards { get; set; }
    }

    public class PPTournamentLeaderboard
    {
        public int tournamentID { get; set; }
        public int index { get; set; }
        public List<PPTournamentLeaderItem> items { get; set; }
    }

    public class PPTournamentLeaderboardsV3
    {
        public int error { get; set; }
        public string description { get; set; }
        public List<PPTournamentLeaderboardV3> leaderboards { get; set; }
    }
    public class PPTournamentLeaderboardV3
    {
        public int tournamentID { get; set; }
        public int index { get; set; }
        public List<string> items { get; set; }
    }


    public class PPTournamentLeaderItem
    {
        public int position { get; set; }
        public string playerID { get; set; }
        public double score { get; set; }
        public double scoreBet { get; set; }
        public string memberCurrency { get; set; }
        public double effectiveBetForFreeRounds { get; set; }
        public double effectiveBetForBetMultiplier { get; set; }
        public string countryID { get; set; }
    }

    public class PPRaceWinnerInfos
    {
        public int error { get; set; }
        public string description { get; set; }
        public List<PPRaceWinnerInfo> winners { get; set; }
    }
    public class PPRaceWinnerInfo
    {
        public int raceID { get; set; }
        public string action { get; set; }
        public List<PPRaceWinner> items { get; set; }
        public string lastIdentity { get; set; }
    }
}
