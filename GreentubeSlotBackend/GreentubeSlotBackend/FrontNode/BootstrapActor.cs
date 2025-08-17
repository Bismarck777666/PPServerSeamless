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
using Microsoft.Owin.Hosting;
using FrontNode.WebsocketService;

namespace FrontNode
{
    public class BootstrapActor : ReceiveActor
    {
        private Config                      _configuration              = null;
        private readonly ILoggingAdapter    _logger                     = Logging.GetLogger(Context);
        private IActorRef                   _dbProxy                    = null;
        private IActorRef                   _userRouter                 = null;
        private IActorRef                   _webSocketListener          = null;//AmanetWebSocket

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

                _userRouter             = Context.System.ActorOf(Akka.Actor.Props.Empty.WithRouter(FromConfig.Instance), "userRouter");
                
                var websocketConfig = _configuration.GetConfig("websocket");
                if (websocketConfig != null)
                {
                    _webSocketListener = Context.System.ActorOf(Akka.Actor.Props.Create(() => new WebsocketListener(websocketConfig, dbActors.Reader)), "webSocketListener");
                    _webSocketListener.Tell("start");
                }

                var gameFrontConfig = _configuration.GetConfig("gameFront");
                if (gameFrontConfig != null)
                {
                    FrontConfig.FrontTokenKey = gameFrontConfig.GetString("tokenkey");
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
                _dbProxy = Context.System.ActorOf(DBProxy.Props(dbConfig), "dbproxy");
                _dbProxy.Tell("initialize");               
            }
        }

        private async Task onShutdownSystem(ShutdownSystemMessage message)
        {
            try
            {
                _logger.Info("Shutting down http service...");

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
