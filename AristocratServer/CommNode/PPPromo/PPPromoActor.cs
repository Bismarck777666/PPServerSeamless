using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Akka.Actor;
using Akka.Event;
using CommNode.Database;
using GITProtocol;
using static CommNode.UserActor;

namespace CommNode.PPPromo
{
    public class PPPromoActor : ReceiveActor
    {
        public PPPromoActor()
        {
            Receive<List<PPTournament>>(onReceiveTournaments);
            Receive<List<PPRace>>(onReceiveRaces);
            Receive<List<PPFreespin>>(onReceiveFreespins);
            Receive<List<PPCashback>>(onReceiveCashbacks);
            Receive<Dictionary<string, double>>(onLatestCurrencyMap);
            Receive<PPPromoGetStatus>(onPPPromoGetStatus);
        }

        private void onReceiveTournaments(List<PPTournament> tournamentList)
        {
            PPPromoSnapshot.Instance.TournamentList = tournamentList;
            PPPromoSnapshot.Instance.generateActivePromos();
            PPPromoSnapshot.Instance.generateTournamentDetails();

            Context.System.EventStream.Publish(new PPPromoUpdateEvent(
                PPPromoSnapshot.Instance.ActivePromos, PPPromoSnapshot.Instance.ActiveTournamentDetails,
                PPPromoSnapshot.Instance.ActiveRaceDetails, PPPromoSnapshot.Instance.DicActiveRaceWinners,
                PPPromoSnapshot.Instance.DicRaceWinnerStrings, PPPromoSnapshot.Instance.ActiveRacePrizes,
                PPPromoSnapshot.Instance.OpenTournamentID, PPPromoSnapshot.Instance.ActiveTournamentIDs,
                PPPromoSnapshot.Instance.OpenTournamentMinBet, PPPromoSnapshot.Instance.CurrencyMap, 
                PPPromoSnapshot.Instance.ActiveFreespins));
        }
        private void onReceiveFreespins(List<PPFreespin> freespins)
        {
            PPPromoSnapshot.Instance.ActiveFreespins = freespins;
            Context.System.EventStream.Publish(new PPPromoUpdateEvent(
                PPPromoSnapshot.Instance.ActivePromos, PPPromoSnapshot.Instance.ActiveTournamentDetails,
                PPPromoSnapshot.Instance.ActiveRaceDetails, PPPromoSnapshot.Instance.DicActiveRaceWinners,
                PPPromoSnapshot.Instance.DicRaceWinnerStrings, PPPromoSnapshot.Instance.ActiveRacePrizes,
                PPPromoSnapshot.Instance.OpenTournamentID, PPPromoSnapshot.Instance.ActiveTournamentIDs,
                PPPromoSnapshot.Instance.OpenTournamentMinBet, PPPromoSnapshot.Instance.CurrencyMap, 
                PPPromoSnapshot.Instance.ActiveFreespins));

        }
        private void onReceiveRaces(List<PPRace> raceList)
        {
            PPPromoSnapshot.Instance.RaceList = raceList;
            PPPromoSnapshot.Instance.generateActivePromos();
            PPPromoSnapshot.Instance.generateRaceDetails();
            PPPromoSnapshot.Instance.generateRaceWinners();

            Context.System.EventStream.Publish(new PPPromoUpdateEvent(
                PPPromoSnapshot.Instance.ActivePromos, PPPromoSnapshot.Instance.ActiveTournamentDetails,
                PPPromoSnapshot.Instance.ActiveRaceDetails, PPPromoSnapshot.Instance.DicActiveRaceWinners,
                PPPromoSnapshot.Instance.DicRaceWinnerStrings, PPPromoSnapshot.Instance.ActiveRacePrizes,
                PPPromoSnapshot.Instance.OpenTournamentID, PPPromoSnapshot.Instance.ActiveTournamentIDs,
                PPPromoSnapshot.Instance.OpenTournamentMinBet, PPPromoSnapshot.Instance.CurrencyMap, 
                PPPromoSnapshot.Instance.ActiveFreespins));
        }
        private void onReceiveCashbacks(List<PPCashback> cashbackList)
        {
            PPPromoSnapshot.Instance.CashbackList = cashbackList;
            PPPromoSnapshot.Instance.generateActivePromos();
            PPPromoSnapshot.Instance.generateRaceDetails();
            Context.System.EventStream.Publish(new PPPromoUpdateEvent(
              PPPromoSnapshot.Instance.ActivePromos, PPPromoSnapshot.Instance.ActiveTournamentDetails,
              PPPromoSnapshot.Instance.ActiveRaceDetails, PPPromoSnapshot.Instance.DicActiveRaceWinners,
              PPPromoSnapshot.Instance.DicRaceWinnerStrings, PPPromoSnapshot.Instance.ActiveRacePrizes,
              PPPromoSnapshot.Instance.OpenTournamentID, PPPromoSnapshot.Instance.ActiveTournamentIDs,
              PPPromoSnapshot.Instance.OpenTournamentMinBet, PPPromoSnapshot.Instance.CurrencyMap, PPPromoSnapshot.Instance.ActiveFreespins));
        }
        
        private void onLatestCurrencyMap(Dictionary<string, double> currencyMap)
        {
            PPPromoSnapshot.Instance.CurrencyMap = currencyMap;
        }
        private void onPPPromoGetStatus(PPPromoGetStatus _)
        {
            Sender.Tell(new PPPromoUpdateEvent(
                PPPromoSnapshot.Instance.ActivePromos, PPPromoSnapshot.Instance.ActiveTournamentDetails,
                PPPromoSnapshot.Instance.ActiveRaceDetails, PPPromoSnapshot.Instance.DicActiveRaceWinners,
                PPPromoSnapshot.Instance.DicRaceWinnerStrings, PPPromoSnapshot.Instance.ActiveRacePrizes,
                PPPromoSnapshot.Instance.OpenTournamentID, PPPromoSnapshot.Instance.ActiveTournamentIDs,
                PPPromoSnapshot.Instance.OpenTournamentMinBet, PPPromoSnapshot.Instance.CurrencyMap, PPPromoSnapshot.Instance.ActiveFreespins));
        }
        protected override void PreStart()
        {
            base.PreStart();

            PPPromoSnapshot.Instance.generateActivePromos();
            PPPromoSnapshot.Instance.generateTournamentDetails();
            PPPromoSnapshot.Instance.generateRaceDetails();
            PPPromoSnapshot.Instance.generateRaceWinners();

            Context.System.ActorSelection("/user/promoRouter").Tell(new RequirePromoSnapshot());
        }
    }

    public class PPPromoGetStatus
    {

    }
    public class PPPromoUpdateEvent
    {
        public PPActivePromos ActivePromos { get; set; }
        public PPTournamentDetails TournamentDetails { get; set; }
        public PPRaceDetails RaceDetails { get; set; }
        public Dictionary<int, string> RaceWinners { get; set; }
        public Dictionary<int, Dictionary<string, string>> DicRaceWinnersString { get; set; }
        public PPRacePrizes RacePrizes { get; set; }
        public List<int> TournamentIDs { get; set; }
        public int OpenTournament { get; set; }
        public double OpenTournamentMinBet { get; set; }
        public Dictionary<string, double> CurrencyMap { get; set; }
        public List<PPFreespin> ActiveFreeSpins;
        public PPPromoUpdateEvent(PPActivePromos strActivePromos, PPTournamentDetails strTournamentDetails, PPRaceDetails strRaceDetails, Dictionary<int, string> raceWinners,
            Dictionary<int, Dictionary<string, string>> dicRaceWinnersString, PPRacePrizes strRacePrizes, int openTournament, List<int> tournamentIDs, double openTournamentMinBet,
            Dictionary<string, double> currencyMap, List<PPFreespin> freeSpins)
        {
            this.ActivePromos = strActivePromos;
            this.TournamentDetails = strTournamentDetails;
            this.RaceDetails = strRaceDetails;
            this.RaceWinners = new Dictionary<int, string>(raceWinners);
            this.DicRaceWinnersString = new Dictionary<int, Dictionary<string, string>>(dicRaceWinnersString);
            this.RacePrizes = strRacePrizes;
            this.OpenTournament = openTournament;

            if (tournamentIDs != null)
                this.TournamentIDs = new List<int>(tournamentIDs);
            else
                this.TournamentIDs = new List<int>();

            this.OpenTournamentMinBet = openTournamentMinBet;
            if (currencyMap != null)
                this.CurrencyMap = currencyMap;
            else
                this.CurrencyMap = new Dictionary<string, double>();
            this.ActiveFreeSpins = freeSpins;
        }
    }
}
