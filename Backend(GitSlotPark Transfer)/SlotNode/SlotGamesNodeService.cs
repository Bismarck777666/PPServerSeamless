using Akka.Actor;
using Akka.Configuration;
using GITProtocol;
using NLog;
using SlotGamesNode.Database;
using System;
using System.IO;
using System.Threading.Tasks;

namespace SlotGamesNode
{
    public class SlotGamesService
    {
        private ActorSystem _slotGameActorSystem;
        private IActorRef   _bootstrapActor;

        public bool Start()
        {
            Logger logger = LogManager.GetCurrentClassLogger();
            logger.Info("Starting SlotGamesNode Service...");

            Config clusterConfig;
            try
            {
                clusterConfig = ConfigurationFactory.ParseString(File.ReadAllText("config.hocon"));
            }
            catch
            {
                logger.Error("Please make sure if config.hocon file is correctly formatted");
                return false;
            }

            Config slotConfig = clusterConfig.GetConfig("slotgames");
            if (slotConfig == null)
            {
                logger.Error("config.hocon doesn't contain single game server configuration");
                return false;
            }

            Config redisConfig = slotConfig.GetConfig("redis");
            if (redisConfig == null)
            {
                logger.Error("config.hocon doesn't contain redis configuration");
                return false;
            }

            setRedisInfo(redisConfig);
            _slotGameActorSystem    = SlotGamesHostFactory.LaunchSlotGamesNode(clusterConfig);
            _bootstrapActor         = _slotGameActorSystem.ActorOf(BootstrapActor.Props(slotConfig), "bootstrapper");
            _bootstrapActor.Tell("startService");
            return true;
        }

        private void setRedisInfo(Config redisConfig)
        {
            string strRedisIP       = redisConfig.GetString("ip",       "127.0.0.1");
            int redisPort           = redisConfig.GetInt("port",        6379);
            string redisPassword    = redisConfig.GetString("password", "");
            int database            = redisConfig.GetInt("database",    0);

            RedisDatabase.setRedisInfo(strRedisIP, redisPort, redisPassword, database);
        }

        public Task TerminationHandle => _slotGameActorSystem.WhenTerminated;

        public async Task StopAsync()
        {
            Logger logger = LogManager.GetCurrentClassLogger();
            logger.Info("Stop SlotGamesNode Service Started...");
            try
            {
                await _bootstrapActor.Ask<bool>(new SlotsNodeShuttingDownMsg(), TimeSpan.FromSeconds(60.0));
            }
            catch (Exception ex)
            {
                logger.Error("Exception has been occured in Stopping bootstrap actor {0}", ex.ToString());
            }

            logger.Info("Leaving from cluster....");
            Akka.Cluster.Cluster cluster = Akka.Cluster.Cluster.Get(_slotGameActorSystem);
            await cluster.LeaveAsync();
        }
    }
}
