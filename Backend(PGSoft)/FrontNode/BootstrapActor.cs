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
using FrontNode.Database;
using FrontNode.HTTPService;
using Microsoft.Owin.Hosting;

namespace FrontNode
{
    public class BootstrapActor : ReceiveActor
    {
        private Config                      _configuration              = null;
        private readonly ILoggingAdapter    _logger                     = Logging.GetLogger(Context);
        private IActorRef                   _dbProxy                    = null;
        private IActorRef                   _userRouter                 = null;
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

                _userRouter         = Context.System.ActorOf(Akka.Actor.Props.Empty.WithRouter(FromConfig.Instance), "userRouter");

                _httpAuthActorGroup         = Context.System.ActorOf(Akka.Actor.Props.Create(() => new HTTPAuthWorker(dbActors.Reader)).WithRouter(FromConfig.Instance), "httpAuthWorkers");
                _httpWorkActorGroup         = Context.System.ActorOf(Akka.Actor.Props.Create(() => new HTTPWorkActor()).WithRouter(FromConfig.Instance),  "httpWorkers");

                HTTPServiceConfig.Instance.WorkerGroup          = _httpWorkActorGroup;
                HTTPServiceConfig.Instance.AuthWorkerGroup      = _httpAuthActorGroup;


                var httpConfig = _configuration.GetConfig("http");
                if (httpConfig != null)
                {
                    string baseAddress = httpConfig.GetString("baseurl", "http://127.0.0.1/");
                    _httpWebService = WebApp.Start<Startup>(url: baseAddress);
                }
                var gameFrontConfig = _configuration.GetConfig("apiconfig");
                if (gameFrontConfig != null)
                {
                    ApiConfig.FrontTokenKey = gameFrontConfig.GetString("tokenkey");
                    ApiConfig.OperatorGUID  = gameFrontConfig.GetString("operatorguid");
                }

            });

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
                //자료기지련결부분을 초기화한다.
                _dbProxy = Context.System.ActorOf(DBProxy.Props(dbConfig), "dbproxy");
                _dbProxy.Tell("initialize");               
            }
        }

        private void onSlotGamesNodeShutdown(SlotsNodeShuttingDownMsg message)
        {
            Routee routee = new ActorSelectionRoutee(Context.System.ActorSelection(Sender.Path));
        }

        private async Task onShutdownSystem(ShutdownSystemMessage message)
        {
            try
            {
                _logger.Info("Shutting down http service...");

                if (_httpWebService != null)
                    _httpWebService.Dispose();

                await _httpAuthActorGroup.GracefulStop(TimeSpan.FromSeconds(300), new Broadcast("terminate"));
                await _httpWorkActorGroup.GracefulStop(TimeSpan.FromSeconds(300), new Broadcast("terminate"));

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
