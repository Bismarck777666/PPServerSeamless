using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Akka.Actor;
using Akka.Configuration;
using Akka.Event;
using Akka.Routing;
using GITProtocol;
using SlotGamesNode.Database;
using SlotGamesNode.HTTPService;
using Microsoft.Owin.Hosting;
using SlotGamesNode.GameLogics;
using Akka.Actor.Dsl;
using SlotGamesNode.Jackpot;

namespace SlotGamesNode
{
    public class BootstrapActor : ReceiveActor
    {
        private Config                      _configuration              = null;
        private readonly ILoggingAdapter    _logger                     = Logging.GetLogger(Context);
        private IActorRef                   _dbProxy                    = null;
        private IActorRef                   _redisWriter                = null;
        private IActorRef                   _userManager                = null;
        private IActorRef                   _gameManager            = null;

        private IActorRef                   _httpWorkActorGroup         = null;
        private IActorRef                   _httpAuthActorGroup         = null;

        private IDisposable                 _httpWebService             = null;


        public BootstrapActor(Config configuration)
        {
            _configuration = configuration;

            Receive<string>(command =>
            {
                processCommand(command);
            });

            ReceiveAsync<DBProxy.ReadyDBProxy>  (onDBReady);
            ReceiveAsync<ShutdownSystemMessage> (onShutdownSystem);

        }
        private async Task onDBReady(DBProxy.ReadyDBProxy dbActors)
        {
            _logger.Info("Database Proxy has been successfully initialized.");
            _redisWriter = Context.System.ActorOf(Akka.Actor.Props.Create(() => new RedisWriter()).WithRouter(FromConfig.Instance), "redisWriter");

            await RedisDatabase.RedisCache.KeyDeleteAsync("onlineusers");

            _userManager = Context.System.ActorOf(UserManager.Props(), "userManager");
            Context.System.ActorOf(Akka.Actor.Props.Create(() => new SpinDBReader()).WithRouter(FromConfig.Instance), "spinDBReaders");

            var props    = Akka.Actor.Props.Create(() => new GameCollectionActor(dbActors.Reader, dbActors.Writer, _redisWriter));
            _gameManager = Context.System.ActorOf(props, "gameManager");

            _logger.Info("started loading game database....");
            bool iSuccess = await _gameManager.Ask<bool>(new LoadSpinDataRequest());
            if (!iSuccess)
            {
                _logger.Error("failed in  loading of game database.");
                return;
            }
            _logger.Info("completed loading of game database successfully.");

            _httpAuthActorGroup = Context.System.ActorOf(Akka.Actor.Props.Create(() => new HTTPAuthWorker(dbActors.Reader, dbActors.Writer, _redisWriter)).WithRouter(FromConfig.Instance), "httpAuthWorkers");
            _httpWorkActorGroup = Context.System.ActorOf(Akka.Actor.Props.Create(() => new HTTPWorkActor()).WithRouter(FromConfig.Instance), "httpWorkers");

            HTTPServiceConfig.Instance.WorkerGroup      = _httpWorkActorGroup;
            HTTPServiceConfig.Instance.AuthWorkerGroup  = _httpAuthActorGroup;

            var httpConfig = _configuration.GetConfig("http");
            if (httpConfig != null)
            {
                string baseAddress = httpConfig.GetString("baseurl", "http://127.0.0.1/");
                _httpWebService = WebApp.Start<Startup>(url: baseAddress);
            }

            if(!LobbyDataSnapshot.Instance.loadJsons())
            {
                _logger.Error("failed in loading of Lobby json datas");
                return;
            }
            LobbyDataSnapshot.Instance.removeGamesNotReady();
        }

        public static Props Props(Config config)
        {
            return Akka.Actor.Props.Create(() => new BootstrapActor(config));
        }

        private void processCommand(string strCommand)
        {
            if(strCommand == "startService")
            {
                HTTPServiceConfig.Instance.JackpotActor = Context.System.ActorOf(Akka.Actor.Props.Create(() => new JackpotWorkActor()), "jackpotActor");
                HTTPServiceConfig.Instance.JackpotActor.Tell("start");

                var dbConfig = _configuration.GetConfig("database");
                if (dbConfig == null)
                {
                    _logger.Error("config.hocon doesn't contain database configuration");
                    return;
                }
                _logger.Info("Initializing database proxy...");

                _dbProxy = Context.System.ActorOf(DBProxy.Props(dbConfig), "dbproxy");
                _dbProxy.Tell("initialize");               
            }
        }

        private async Task onShutdownSystem(ShutdownSystemMessage message)
        {
            try
            {
                _logger.Info("Shutting down tcp and web socket server...");
                if (_httpWebService != null)
                    _httpWebService.Dispose();

                await _httpAuthActorGroup.GracefulStop(TimeSpan.FromSeconds(300), new Broadcast("terminate"));
                await _httpWorkActorGroup.GracefulStop(TimeSpan.FromSeconds(300), new Broadcast("terminate"));

                _logger.Info("Terminating user actors....");
                await _userManager.GracefulStop(TimeSpan.FromSeconds(300), "terminate");


                _logger.Info("Terminating redis write actors....");
                _redisWriter.Tell(new Broadcast(PoisonPill.Instance));
                await _redisWriter.GracefulStop(TimeSpan.FromSeconds(300));

                _logger.Info("Terminating database proxy actors....");
                await _dbProxy.GracefulStop(TimeSpan.FromSeconds(3600), "terminate");

                _logger.Info("Leaving from cluster....");
                var cluster = Akka.Cluster.Cluster.Get(Context.System);
                await cluster.LeaveAsync();
            }
            catch(Exception)
            {

            }
            Sender.Tell(true);
        }
    }

    public class ShutdownSystemMessage
    {

    }
}
