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
using CommNode.PPPromo;

namespace CommNode
{
    public class BootstrapActor : ReceiveActor
    {
        private Config                      _configuration              = null;
        private readonly ILoggingAdapter    _logger                     = Logging.GetLogger(Context);
        private IActorRef                   _dbProxy                    = null;
        private IActorRef                   _redisWriter                = null;

        private IActorRef                   _userManager                = null;
        private IActorRef                   _slotGamesRouter            = null;
        private IActorRef                   _webSocketListener          = null;//CQ9WebSocket
        private IActorRef                   _amanetWebSocketListener    = null;//AmanetWebSocket
        private IActorRef                   _scoreDataCacheActor        = null;
        private IActorRef                   _ppPromoRouter              = null;
        private IActorRef                   _ppPromotionActor           = null;
        private IActorRef                   _bngPromotionActor          = null;

        private IActorRef                   _httpWorkActorGroup         = null;
        private IActorRef                   _httpReplayWorkActorGroup   = null;
        private IActorRef                   _httpHistoryWorkActorGroup = null;

        private IActorRef                   _httpAuthActorGroup     = null;
        private IDisposable                 _httpWebService         = null;


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

                //创建Redis数据库角色。
                _redisWriter = Context.System.ActorOf(Akka.Actor.Props.Create(() => new RedisWriter()).WithRouter(FromConfig.Instance), "redisWriter");

                //管理Pragmatic推广信息的Actor
                _ppPromotionActor = Context.System.ActorOf(Akka.Actor.Props.Create(() => new PPPromoActor()), "promofetcher");

                _bngPromotionActor = Context.System.ActorOf(Akka.Actor.Props.Create(() => new BNGPromoActor()), "bngPromofetcher");
                _bngPromotionActor.Tell("fetch");

                //创建UserManager角色。
                _userManager = Context.System.ActorOf(UserManager.Props(), "userManager");

                //数据库初始化后启动套接字服务器。
                _slotGamesRouter = Context.System.ActorOf(Akka.Actor.Props.Empty.WithRouter(FromConfig.Instance), Constants.SlotGameRouterName);

                //促销服务器路由器
                _ppPromoRouter = Context.System.ActorOf(Akka.Actor.Props.Empty.WithRouter(FromConfig.Instance), "promoRouter");

                _httpAuthActorGroup         = Context.System.ActorOf(Akka.Actor.Props.Create(() => new HTTPAuthWorker(dbActors.Reader, dbActors.Writer, _redisWriter)).WithRouter(FromConfig.Instance), "httpAuthWorkers");
                _httpWorkActorGroup         = Context.System.ActorOf(Akka.Actor.Props.Create(() => new HTTPWorkActor()).WithRouter(FromConfig.Instance),  "httpWorkers");
                _httpReplayWorkActorGroup   = Context.System.ActorOf(Akka.Actor.Props.Create(() => new HTTPReplayWorkActor(dbActors.Reader)).WithRouter(FromConfig.Instance),  "httpReplayWorkers");
                _httpHistoryWorkActorGroup  = Context.System.ActorOf(Akka.Actor.Props.Create(() => new HTTPHistoryWorkActor(dbActors.Reader)).WithRouter(FromConfig.Instance), "httpHistoryWorkers");

                HTTPServiceConfig.Instance.WorkerGroup          = _httpWorkActorGroup;
                HTTPServiceConfig.Instance.AuthWorkerGroup      = _httpAuthActorGroup;
                HTTPServiceConfig.Instance.ReplayWorkerGroup    = _httpReplayWorkActorGroup;
                HTTPServiceConfig.Instance.HistoryWorkerGroup   = _httpHistoryWorkActorGroup;

                _ppPromotionActor.Tell("start");

                //WebSocket服务
                var websocketConfig = _configuration.GetConfig("websocket");
                if (websocketConfig != null)
                {
                    _webSocketListener = Context.System.ActorOf(Akka.Actor.Props.Create(() => new WebsocketListener(websocketConfig, dbActors.Reader, dbActors.Writer, _redisWriter)), "webSocketListener");
                    _webSocketListener.Tell("start");
                }

                //Amanet网络套接字服务
                var amanetWebsocketConfig = _configuration.GetConfig("amaticwebsocket");
                if (amanetWebsocketConfig != null)
                {
                    _amanetWebSocketListener = Context.System.ActorOf(Akka.Actor.Props.Create(() => new AmaticWebsocketListener(amanetWebsocketConfig, dbActors.Reader, dbActors.Writer, _redisWriter)), "amanetWebSocketListener");
                    _amanetWebSocketListener.Tell("start");
                }

                //网络请求服务
                var httpConfig = _configuration.GetConfig("http");
                if (httpConfig != null)
                {
                    string baseAddress = httpConfig.GetString("baseurl", "http://127.0.0.1/");
                    _httpWebService = WebApp.Start<Startup>(url: baseAddress);
                }
                
                //Pragmatic Play游戏设置
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


                //第一步初始化数据库连接部分。
                _dbProxy = Context.System.ActorOf(DBProxy.Props(dbConfig), "dbproxy");
                _dbProxy.Tell("initialize");               
            }
        }


        //Single游戏服务器即将关闭。
        private void onSlotGamesNodeShutdown(SlotsNodeShuttingDownMsg message)
        {
            //从路由器中删除，防止新进入游戏的用户进入该服务器。
            Routee routee = new ActorSelectionRoutee(Context.System.ActorSelection(Sender.Path));
            _slotGamesRouter.Tell(new RemoveRoutee(routee));

            _userManager.Tell(new SlotGamesNodeShuttingdown(Sender.Path.ToString()));
        }

        private async Task onShutdownSystem(ShutdownSystemMessage message)
        {
            try
            {
                //首先关闭套接字服务器。
                _logger.Info("Shutting down tcp and web socket server...");

                if (_webSocketListener != null)
                    await _webSocketListener.GracefulStop(TimeSpan.FromSeconds(60));

                if (_httpWebService != null)
                    _httpWebService.Dispose();

                await _httpAuthActorGroup.GracefulStop(TimeSpan.FromSeconds(300), new Broadcast("terminate"));
                await _httpWorkActorGroup.GracefulStop(TimeSpan.FromSeconds(300), new Broadcast("terminate"));

                _logger.Info("Terminating user actors....");
                await _userManager.GracefulStop(TimeSpan.FromSeconds(300), "terminate");


                //停止Redis数据库角色。
                _logger.Info("Terminating redis write actors....");
                _redisWriter.Tell(new Broadcast(PoisonPill.Instance));
                await _redisWriter.GracefulStop(TimeSpan.FromSeconds(300));

                //停止基础数据库角色。
                _logger.Info("Terminating database proxy actors....");
                await _dbProxy.GracefulStop(TimeSpan.FromSeconds(3600), "terminate");


                //从集群中退出。
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
