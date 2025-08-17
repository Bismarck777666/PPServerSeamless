using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Akka.Actor;
using GITProtocol;
using Akka.Routing;
using Akka.Event;

namespace PromoNode
{
    public class PromoFetchActor : ReceiveActor
    {
        private IActorRef _dbReader;
        private IActorRef _dbWriter;
        private List<PPRace> _lastRaces = null;
        private readonly ILoggingAdapter _logger = Logging.GetLogger(Context);

        private const int WaitTimeForDeletingCancelledPrizes = 3;

        private GetActiveTournamentsResponse _activeTournaments = null;
        private GetActiveFreespinsResponse _activeFreespins = null;
        private GetActiveRaceResponse _activeRaces = null;
        private GetActiveCashbacksResponse _activeCashbacks = null;
        private Dictionary<string, double> _latestCurrencyMap = null;
        private Dictionary<long, int> _cancelledRacePrizes = new Dictionary<long, int>();
        public PromoFetchActor(IActorRef dbReader, IActorRef dbWriter)
        {
            _dbReader = dbReader;
            _dbWriter = dbWriter;
            Receive<GetActiveTournamentsResponse>(onGetActiveTournamentResponse);
            Receive<GetActiveRaceResponse>(onGetActiveRaceResponse);
            Receive<GetActiveFreespinsResponse>(onGetAcriveFreespinsResponse);
            Receive<GetActiveCashbacksResponse>(onGetActiveCashbacksResponse);
            Receive<Dictionary<string, double>>(onGetLatestCurrencyMap);
            Receive<RequirePromoSnapshot>(onRequirePromoSnapshot);
        }

        protected override void PreStart()
        {
            base.PreStart();

            _dbReader.Tell(new GetActiveTournamentsRequest());
            _dbReader.Tell(new GetActiveFreespinsRequest());
            _dbReader.Tell(new GetActiveRaceRequest(_lastRaces));
            _dbReader.Tell(new GetLatestCurrencyMap());
            _dbReader.Tell(new GetActiveCashbacksRequest());
        }

        private void onGetActiveTournamentResponse(GetActiveTournamentsResponse response)
        {
            if (response.TournamentList != null)
            {
                Context.System.ActorSelection("/user/commServers").Tell(new Broadcast(response.TournamentList));

            }
            _logger.Info("Tour Received");

            _activeTournaments = response;
            Context.System.Scheduler.ScheduleTellOnceCancelable(60 * 1000, _dbReader, new GetActiveTournamentsRequest(), Self);
        }

        private void onGetAcriveFreespinsResponse(GetActiveFreespinsResponse response)
        {
            if (response.FreespinList != null && response.FreespinList.Count > 0)
            {
                Context.System.ActorSelection("/user/commServers").Tell(new Broadcast(response.FreespinList));
            }
            _logger.Info("Freespin Received");

            _activeFreespins = response;
            Context.System.Scheduler.ScheduleTellOnceCancelable(10 * 1000, _dbReader, new GetActiveFreespinsRequest(), Self);
        }

        private void onGetLatestCurrencyMap(Dictionary<string, double> currencyMap)
        {
            _latestCurrencyMap = currencyMap;
            Context.System.ActorSelection("/user/commServers").Tell(new Broadcast(currencyMap));
            Context.System.Scheduler.ScheduleTellOnceCancelable(60 * 1000, _dbReader, new GetLatestCurrencyMap(), Self);

        }
        private void onGetActiveRaceResponse(GetActiveRaceResponse response)
        {
            if (response.RaceList != null)
            {
                Context.System.ActorSelection("/user/commServers").Tell(new Broadcast(response.RaceList));
                _lastRaces = response.RaceList;
            }
            _logger.Info("Race Received");

            _activeRaces = response;
            Context.System.Scheduler.ScheduleTellOnceCancelable(10 * 1000, _dbReader, new GetActiveRaceRequest(_lastRaces), Self);
        }
        private void onGetActiveCashbacksResponse(GetActiveCashbacksResponse response)
        {
            if (response.CashbackList != null)
            {
                Context.System.ActorSelection("/user/commServers").Tell(new Broadcast(response.CashbackList));
            }
            _logger.Info("Cashback Received");
            _activeCashbacks = response;
            Context.System.Scheduler.ScheduleTellOnceCancelable(10 * 1000, _dbReader, new GetActiveCashbacksRequest(), Self);
        }
        private void onRequirePromoSnapshot(RequirePromoSnapshot _)
        {
            if (_activeTournaments != null && _activeTournaments.TournamentList != null)
                Sender.Tell(_activeTournaments.TournamentList);

            if (_activeRaces != null && _activeRaces.RaceList != null)
                Sender.Tell(_activeRaces.RaceList);

            if (_latestCurrencyMap != null)
                Sender.Tell(_latestCurrencyMap);
        }
    }
}
