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
using Microsoft.Owin.Hosting;

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
        private IActorRef                   _amanetWebSocketListener    = null;//AmanetWebSocket
        private IActorRef                   _ipLookupActor              = null;
        private IActorRef                   _scoreDataCacheActor        = null;

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

            Receive<DBProxy.ReadyDBProxy>(dbActors =>
            {
                _logger.Info("Database Proxy has been successfully initialized.");

                //Redis자료기지액터를 창조한다.
                _redisWriter = Context.System.ActorOf(Akka.Actor.Props.Create(() => new RedisWriter()).WithRouter(FromConfig.Instance), "redisWriter");

                //스코자료캐시액터를 창조한다.
                _scoreDataCacheActor = Context.System.ActorOf(Akka.Actor.Props.Create(() => new ScoreCacheActor()), "scoreCacheActor");

                //UserManager 액터를 창조한다.
                _userManager = Context.System.ActorOf(UserManager.Props(), "userManager");

                //자료기지가 초기화되면 소켓서버들을 시작한다.
                _slotGamesRouter = Context.System.ActorOf(Akka.Actor.Props.Empty.WithRouter(FromConfig.Instance), Constants.SlotGameRouterName);

                //아마네트 웹소켓서비스
                var amanetWebsocketConfig = _configuration.GetConfig("amaticwebsocket");
                if (amanetWebsocketConfig != null)
                {
                    _amanetWebSocketListener = Context.System.ActorOf(Akka.Actor.Props.Create(() => new AmaticWebsocketListener(amanetWebsocketConfig, dbActors.Reader, dbActors.Writer, _redisWriter)), "amanetWebSocketListener");
                    _amanetWebSocketListener.Tell("start");
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

                //첫단계로 자료기지련결부분을 초기화한다.
                _dbProxy = Context.System.ActorOf(DBProxy.Props(dbConfig), "dbproxy");
                _dbProxy.Tell("initialize");               
            }
        }


        //Single게임서버가 종료되려고 한다.
        private void onSlotGamesNodeShutdown(SlotsNodeShuttingDownMsg message)
        {
            //새로 게임에 입장하는 유저들이 해당 서버에 입장하지 못하도록 루터에서 삭제한다.
            Routee routee = new ActorSelectionRoutee(Context.System.ActorSelection(Sender.Path));
            _slotGamesRouter.Tell(new RemoveRoutee(routee));

            _userManager.Tell(new SlotGamesNodeShuttingdown(Sender.Path.ToString()));
        }

        private async Task onShutdownSystem(ShutdownSystemMessage message)
        {
            try
            {
                //먼저 소켓서버들을 종료시킨다.
                _logger.Info("Shutting down tcp and web socket server...");

                if (_amanetWebSocketListener != null)
                    await _amanetWebSocketListener.GracefulStop(TimeSpan.FromSeconds(60));

                if (_httpWebService != null)
                    _httpWebService.Dispose();

                await _httpAuthActorGroup.GracefulStop(TimeSpan.FromSeconds(300), new Broadcast("terminate"));

                await _httpWorkActorGroup.GracefulStop(TimeSpan.FromSeconds(300), new Broadcast("terminate"));

                _logger.Info("Terminating user actors....");
                await _userManager.GracefulStop(TimeSpan.FromSeconds(300), "terminate");


                //Redis자료기지액터를 중지한다.
                _logger.Info("Terminating redis write actors....");
                _redisWriter.Tell(new Broadcast(PoisonPill.Instance));
                await _redisWriter.GracefulStop(TimeSpan.FromSeconds(300));

                //기본자료기지액터들을 중지한다.
                _logger.Info("Terminating database proxy actors....");
                await _dbProxy.GracefulStop(TimeSpan.FromSeconds(3600), "terminate");


                //클라스터에서 탈퇴한다.
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
