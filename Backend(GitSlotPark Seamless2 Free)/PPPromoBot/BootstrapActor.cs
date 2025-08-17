using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Akka.Actor;
using Akka.Configuration;
using Akka.Event;
using Akka.Routing;
using PPPromoBot.Database;

namespace PPPromoBot
{
    public class BootstrapActor : ReceiveActor
    {
        private Config                      _configuration  = null;
        private readonly ILoggingAdapter    _logger         = Logging.GetLogger(Context);
        private IActorRef                   _dbProxy        = null;
        private IActorRef                   _ppPromoFetcher = null;
        public BootstrapActor(Config configuration)
        {
            _configuration = configuration;

            Receive<string>                         (processCommand);
            ReceiveAsync<PPPromoBotShuttingDownMsg> (onShutdownServer);
        }

        public static Props Props(Config config)
        {
            return Akka.Actor.Props.Create(() => new BootstrapActor(config));
        }

        private async Task onShutdownServer(PPPromoBotShuttingDownMsg message)
        {
            _logger.Info("Shutting down server....");
            await _ppPromoFetcher.GracefulStop(TimeSpan.FromSeconds(10), "terminate");
            Sender.Tell(true);
        }

        private void processCommand(string strCommand)
        {
            if (strCommand == "startService")
            {
                _logger.Info("Start PPPromo Bot...");

                _ppPromoFetcher = Context.System.ActorOf(Akka.Actor.Props.Create(() => new PPPromoFetcher(_configuration.GetConfig("siteinfo"), RedisDatabase.DBCount)), "ppPromoFetcher");
                _ppPromoFetcher.Tell("tick");
            }
        }
    }
}
