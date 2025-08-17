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
using PromoNode.Database;

namespace PromoNode
{
    public class BootstrapActor : ReceiveActor
    {
        private Config                      _configuration      = null;
        private readonly ILoggingAdapter    _logger             = Logging.GetLogger(Context);
        private IActorRef                   _dbProxy            = null;
        private IActorRef                   _promoFetcher       = null;
        private IActorRef                   _redisWriter        = null;
        private string                      _serverID           = "";
        private IActorRef                   _commServerGroup    = null;

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

                _commServerGroup = Context.System.ActorOf(Akka.Actor.Props.Empty.WithRouter(FromConfig.Instance),               "commServers");

                _promoFetcher = Context.System.ActorOf(Akka.Actor.Props.Create(() => new PromoFetchActor(dbActors.Reader, dbActors.Writer)),     "promofetcher");

            });
            ReceiveAsync<SlotsNodeShuttingDownMsg>(onShutdownServer);
        }

        public static Props Props(Config config)
        {
            return Akka.Actor.Props.Create(() => new BootstrapActor(config));
        }

        private async Task onShutdownServer(SlotsNodeShuttingDownMsg message)
        {
            _logger.Info("Shutting down server....");

           
            _logger.Info("Terminating tournament actor....");
            await _promoFetcher.GracefulStop(TimeSpan.FromSeconds(10));


            _logger.Info("Terminating database proxy actors....");
            await _dbProxy.GracefulStop(TimeSpan.FromSeconds(30), "terminate");

            Sender.Tell(true);


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

                _serverID = _configuration.GetString("identifider", "");
                _logger.Info("Start Single Game Server(id:{0})...", _serverID);
                _logger.Info("Initializing database proxy...");

                _dbProxy        = Context.System.ActorOf(DBProxy.Props(dbConfig), "dbproxy");
                _dbProxy.Tell("initialize");
            }
            else
            {

            }
        }
    }
}
