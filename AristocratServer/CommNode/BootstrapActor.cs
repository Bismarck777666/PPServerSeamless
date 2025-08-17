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
using CommNode.Database;
using CommNode.HTTPService;
using Microsoft.Owin.Hosting;

namespace CommNode
{
    public class BootstrapActor : ReceiveActor
    {
        private Config                      _configuration                  = null;
        private readonly ILoggingAdapter    _logger                         = Logging.GetLogger(Context);
        private IActorRef                   _dbProxy                        = null;
        private IActorRef                   _redisWriter                    = null;
        private IActorRef                   _userManager                    = null;
        private IActorRef                   _slotGamesRouter                = null;
        private IActorRef                   _ppPromoRouter                  = null;
        private IActorRef                   _ppPromotionActor               = null;
        private IActorRef                   _httpWorkActorGroup             = null;
        private IActorRef                   _httpReplayWorkActorGroup       = null;
        private IActorRef                   _httpHistoryWorkActorGroup      = null;
        private IActorRef                   _httpCallbackWorkActorGroup     = null;
        private IActorRef                   _httpAuthActorGroup             = null;
        private IDisposable                 _httpWebService                 = null;
        private IActorRef                   _retryWorkers                   = null;


        public BootstrapActor(Config configuration)
        {
            _configuration = configuration;

            Receive<string>(command =>
            {
                processCommand(command);
            });

            Receive<DBProxy.ReadyDBProxy>(dbActors =>
            {
                _logger.Info("Database Proxy has been successfully initialized.");

                _redisWriter        = Context.System.ActorOf(Akka.Actor.Props.Create(() => new RedisWriter()).WithRouter(FromConfig.Instance), "redisWriter");
                _userManager        = Context.System.ActorOf(UserManager.Props(), "userManager");
                _slotGamesRouter    = Context.System.ActorOf(Akka.Actor.Props.Empty.WithRouter(FromConfig.Instance), Constants.SlotGameRouterName);
                _ppPromoRouter      = Context.System.ActorOf(Akka.Actor.Props.Empty.WithRouter(FromConfig.Instance), "promoRouter");
                _retryWorkers       = Context.System.ActorOf(Akka.Actor.Props.Create(() => new RetryWorker(dbActors.Writer)).WithRouter(FromConfig.Instance), "retryWorkers");
                _retryWorkers.Tell(new Broadcast("tick"));

                _httpAuthActorGroup = Context.System.ActorOf(Akka.Actor.Props.Create(() => new HTTPAuthWorker(dbActors.Reader, dbActors.Writer, _redisWriter)).WithRouter(FromConfig.Instance), "httpAuthWorkers");
                _httpWorkActorGroup         = Context.System.ActorOf(Akka.Actor.Props.Create(() => new HTTPWorkActor()).WithRouter(FromConfig.Instance),  "httpWorkers");
                _httpReplayWorkActorGroup   = Context.System.ActorOf(Akka.Actor.Props.Create(() => new HTTPReplayWorkActor(dbActors.Reader)).WithRouter(FromConfig.Instance),  "httpReplayWorkers");
                _httpHistoryWorkActorGroup  = Context.System.ActorOf(Akka.Actor.Props.Create(() => new HTTPHistoryWorkActor(dbActors.Reader)).WithRouter(FromConfig.Instance), "httpHistoryWorkers");
                _httpCallbackWorkActorGroup = Context.System.ActorOf(Akka.Actor.Props.Create(() => new HTTPCallbackWorkActor(dbActors.Reader, dbActors.Writer, _redisWriter)).WithRouter(FromConfig.Instance), "httpCallbackWorkers");

                HTTPServiceConfig.Instance.WorkerGroup          = _httpWorkActorGroup;
                HTTPServiceConfig.Instance.AuthWorkerGroup      = _httpAuthActorGroup;
                HTTPServiceConfig.Instance.ReplayWorkerGroup    = _httpReplayWorkActorGroup;
                HTTPServiceConfig.Instance.HistoryWorkerGroup   = _httpHistoryWorkActorGroup;
                HTTPServiceConfig.Instance.CallbackWorkActor    = _httpCallbackWorkActorGroup;
                //_ppPromotionActor.Tell("start");

                var httpConfig = _configuration.GetConfig("http");
                if (httpConfig != null)
                {
                    string baseAddress          = httpConfig.GetString("baseurl", "http://127.0.0.1/");
                    StartOptions startOption    = new StartOptions();
                    startOption.Urls.Add(baseAddress);
                    //startOption.Urls.Add("http://localhost:8080");
                    _httpWebService = WebApp.Start<Startup>(startOption);
                }
                
                var ppConfig = _configuration.GetConfig("ppconfig");
                if (ppConfig != null)
                {
                    PragmaticConfig.Instance.ReplayURL = ppConfig.GetString("url", "");
                }

            });

            Receive<SlotsNodeShuttingDownMsg>   (_ => onSlotGamesNodeShutdown(_));
            ReceiveAsync<ShutdownSystemMessage> (onShutdownSystem);

        }

        public static Props Props(Config config)
        {
            return Akka.Actor.Props.Create(() => new BootstrapActor(config));
        }

        private void processCommand(string strCommand)
        {
            if(strCommand == "startService")
            {
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

        private void onSlotGamesNodeShutdown(SlotsNodeShuttingDownMsg message)
        {
            Routee routee = new ActorSelectionRoutee(Context.System.ActorSelection(Sender.Path));
            _slotGamesRouter.Tell(new RemoveRoutee(routee));

            _userManager.Tell(new SlotGamesNodeShuttingdown(Sender.Path.ToString()));
        }

        private async Task onShutdownSystem(ShutdownSystemMessage message)
        {
            try
            {
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
