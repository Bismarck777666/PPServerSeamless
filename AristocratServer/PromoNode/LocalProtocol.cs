using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GITProtocol;

namespace PromoNode
{
    public class GetActiveTournamentsRequest
    {

    }

    public class GetLatestCurrencyMap
    {

    }
    public class GetActiveTournamentsResponse
    {
        public List<PPTournament> TournamentList { get; private set; }

        public GetActiveTournamentsResponse(List<PPTournament> tournamentList)
        {
            this.TournamentList = tournamentList;
        }
    }
    public class GetActiveRaceRequest
    {
        public List<PPRace> LastRaceList { get; private set; }
        public GetActiveRaceRequest(List<PPRace> raceList)
        {
            this.LastRaceList = raceList;
        }
    }

    public class GetActiveRaceResponse
    {
        public List<PPRace> RaceList { get; private set; }
        public GetActiveRaceResponse(List<PPRace> raceList)
        {
            this.RaceList = raceList;
        }
    }

    public class GetActiveFreespinsRequest
    {

    }
    public class GetActiveFreespinsResponse
    {
        public List<PPFreespin> FreespinList { get; private set; }
        public GetActiveFreespinsResponse(List<PPFreespin> freespinList)
        {
            this.FreespinList = freespinList;
        }
    }

    public class GetActiveCashbacksRequest
    {

    }
    public class GetActiveCashbacksResponse
    {
        public List<PPCashback> CashbackList { get; private set;}
        public GetActiveCashbacksResponse(List<PPCashback> cashbackList)
        {
            CashbackList = cashbackList;
        }
    }
}
