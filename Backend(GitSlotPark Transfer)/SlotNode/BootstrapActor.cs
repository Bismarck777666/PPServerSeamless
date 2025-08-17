using Akka.Actor;
using Akka.Configuration;
using Akka.Event;
using Akka.Routing;
using GITProtocol;
using SlotGamesNode.Database;
using SlotGamesNode.GameLogics;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace SlotGamesNode
{
    public class BootstrapActor : ReceiveActor
    {
        private Config                      _configuration      = null;
        private readonly ILoggingAdapter    _logger             = Context.GetLogger();
        private IActorRef                   _dbProxy            = null;
        private IActorRef                   _gameManager        = null;
        private IActorRef                   _redisWriter        = null;
        private string                      _serverID           = "";
        private IActorRef                   _userServerGroup    = null;

        public BootstrapActor(Config configuration)
        {
            _configuration = configuration;
            
            Receive<string>                         (processCommand);
            ReceiveAsync<DBProxy.ReadyDBProxy>      (onReadyDBProxy);
            ReceiveAsync<SlotsNodeShuttingDownMsg>  (onShutdownServer);
        }

        public static Props Props(Config config)
        {
            return Akka.Actor.Props.Create(() => new BootstrapActor(config));
        }

        private async Task onReadyDBProxy(DBProxy.ReadyDBProxy dbActors)
        {
            _logger.Info("Database Proxy has been successfully initialized.");
            
            //Redis자료기지액터를 창조한다.
            _redisWriter        = Context.System.ActorOf(Akka.Actor.Props.Create(() => new RedisWriter()).WithRouter(FromConfig.Instance), "redisWriter");
            
            //유저서버루터창조
            _userServerGroup    = Context.System.ActorOf(Akka.Actor.Props.Empty.WithRouter(FromConfig.Instance),        "userServers");
            
            //API클라스터시작
            Context.System.ActorOf(Akka.Actor.Props.Empty.WithRouter((RouterConfig)FromConfig.Instance),   "apiWorker");
            
            IActorRef tourScoreWriter = Context.System.ActorOf(Akka.Actor.Props.Create(() => new TournamentScoreWriter()).WithRouter(FromConfig.Instance), "tourScoreWriter");
            tourScoreWriter.Tell(new Broadcast((object)"start"));

            PayoutPoolConfig.Instance.PoolActor = Context.System.ActorOf(Akka.Actor.Props.Create(() => new PayoutPoolActor()),  "payoutPool");
            Context.System.ActorOf(Akka.Actor.Props.Create(() => new SpinDBReader()).WithRouter(FromConfig.Instance),           "spinDBReaders");
            Props props = Akka.Actor.Props.Create(() => new GameCollectionActor(dbActors.Reader, dbActors.Writer, _redisWriter)).WithRouter(FromConfig.Instance);
            
            _gameManager = Context.System.ActorOf(props, "gameManager");
            _logger.Info("started loading game database....");

            bool iSuccess = await _gameManager.Ask<bool>(new LoadSpinDataRequest());
            if (!iSuccess)
            {
                _logger.Error("failed in  loading of game database.");
                return;
            }
                
            _logger.Info("completed loading of game database successfully.");
            _logger.Info("cluster joining...");

            Akka.Cluster.Cluster cluster    = Akka.Cluster.Cluster.Get(Context.System);
            IList<string> strSeedNodeList   = _configuration.GetStringList("seednodes");
            List<Address> seedNodeAddressList = new List<Address>();

            foreach (string strAddress in strSeedNodeList)
                seedNodeAddressList.Add(Address.Parse(strAddress));

            await cluster.JoinSeedNodesAsync(seedNodeAddressList);
        }

        private async Task onShutdownServer(SlotsNodeShuttingDownMsg message)
        {
            _logger.Info("Shutting down server....");
            _logger.Info("Inform connect servers to remove this server from router....");

            _userServerGroup.Tell(new Broadcast(new SlotsNodeShuttingDownMsg()), _gameManager);
            _logger.Info("Waiting for 20 seconds until all the users connected to this server can connect to other servers");
            await Task.Delay(20000);

            _logger.Info("Terminating game logic actors....");
            await _gameManager.GracefulStop(TimeSpan.FromSeconds(10.0), "terminate");
            
            _logger.Info("Terminating redis write actors....");
            await _redisWriter.GracefulStop(TimeSpan.FromSeconds(10.0));
            
            _logger.Info("Terminating database proxy actors....");
            await _dbProxy.GracefulStop(TimeSpan.FromSeconds(30.0), "terminate");
            
            Sender.Tell(true);
        }

        private void processCommand(string strCommand)
        {
            if (strCommand == "startService")
            {
                Config config = _configuration.GetConfig("database");
                if (config == null)
                    _logger.Error("config.hocon doesn't contain database configuration");
                else if (!ChipsetManager.Instance.loadChipsetFile())
                    _logger.Error("failed to load currency chipset information.");
                else if (!ChipsetManager.Instance.loadRTPInfos())
                    _logger.Error("failed to load gamertp information.");
                else
                {
                    _serverID = _configuration.GetString("identifider", "");

                    _logger.Info("Start Single Game Server(id:{0})...", _serverID);
                    _logger.Info("Initializing database proxy...");
                    
                    _dbProxy = Context.System.ActorOf(DBProxy.Props(config), "dbproxy");
                    _dbProxy.Tell("initialize");
                }
            }
        }
    }

    public class PerformanceTestRequest
    {
        public GAMEID GameID { get; private set; }

        public PerformanceTestRequest(GAMEID gameID)
        {
            GameID = gameID;
        }
    }
}
