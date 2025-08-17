using Akka;
using Akka.Actor;
using Akka.Configuration;
using FrontNode.Database;
using NLog;
using System;
using System.IO;
using System.Threading.Tasks;

namespace FrontNode
{
    public class FrontService
    {
        private ActorSystem _actorSystem;
        private IActorRef   _bootstrapActor;

        public bool Start()
        {
            Logger _logger = LogManager.GetCurrentClassLogger();
            _logger.Info("Starting FrontNode Service...");
            Config clusterConfig;
            try
            {
                clusterConfig = ConfigurationFactory.ParseString(File.ReadAllText("config.hocon"));
            }
            catch
            {
                _logger.Error("Please make sure if config.hocon file is correctly formatted");
                return false;
            }

            Config frontConfig = clusterConfig.GetConfig("front");
            if (frontConfig == null)
            {
                _logger.Error("config.hocon doesn't contain connector server configuration");
                return false;
            }

            Config redisConfig = frontConfig.GetConfig("redis");
            if (redisConfig == null)
            {
                _logger.Error("config.hocon doesn't contain redis configuration");
                return false;
            }
            setRedisInfo(redisConfig);
            
            _actorSystem    = ActorSystem.Create(frontConfig.GetString("actorsystem", "gitigaming"), clusterConfig);
            _bootstrapActor = _actorSystem.ActorOf(BootstrapActor.Props(frontConfig), "bootstrapper");
            _bootstrapActor.Tell("startService");
            return true;
        }

        private void setRedisInfo(Config redisConfig)
        {
            string  strRedisIP      = redisConfig.GetString("ip", "127.0.0.1");
            int     redisPort       = redisConfig.GetInt("port", 6379);
            string  redisPassword   = redisConfig.GetString("password", "");
            int     database        = redisConfig.GetInt("database",    0);

            RedisDatabase.setRedisInfo(strRedisIP, redisPort, redisPassword, database);
        }

        public Task TerminationHandle => _actorSystem.WhenTerminated;

        public async Task StopAsync()
        {
            Logger logger = LogManager.GetCurrentClassLogger();
            logger.Info("Stop Service Started...");
            try
            {
                await _bootstrapActor.Ask<bool>(new ShutdownSystemMessage(), TimeSpan.FromSeconds(60.0));
            }
            catch (Exception ex)
            {
                logger.Error("Exception has been occured in Stopping bootstrap actor {0}", ex.ToString());
            }
            await CoordinatedShutdown.Get(_actorSystem).Run(CoordinatedShutdown.ClrExitReason.Instance);
        }
    }
}
