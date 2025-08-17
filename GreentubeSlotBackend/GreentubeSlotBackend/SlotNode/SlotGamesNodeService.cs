using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Akka.Actor;
using Akka.Configuration;
using System.IO;
using SlotGamesNode.Database;
using GITProtocol;

namespace SlotGamesNode
{
    public class SlotGamesService
    {
        private ActorSystem _slotGameActorSystem;
        private IActorRef   _bootstrapActor;
        public bool Start()
        {
            var logger = NLog.LogManager.GetCurrentClassLogger();
            logger.Info("Starting SlotGamesNode Service...");

            Config clusterConfig = null;
            try
            {
                clusterConfig = ConfigurationFactory.ParseString(File.ReadAllText("config.hocon"));
            }
            catch
            {
                logger.Error("Please make sure if config.hocon file is correctly formatted");
                return false;
            }

            var slotGamesConfig = clusterConfig.GetConfig("slotgames");
            if(slotGamesConfig == null)
            {
                logger.Error("config.hocon doesn't contain single game server configuration");
                return false;
            }

            var redisConfig = slotGamesConfig.GetConfig("redis");
            if(redisConfig == null)
            {
                logger.Error("config.hocon doesn't contain redis configuration");
                return false;
            }

            setRedisInfo(redisConfig);

            _slotGameActorSystem = SlotGamesHostFactory.LaunchSlotGamesNode(clusterConfig);

            _bootstrapActor = _slotGameActorSystem.ActorOf(BootstrapActor.Props(slotGamesConfig), "bootstrapper");
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
            var logger = NLog.LogManager.GetCurrentClassLogger();
            logger.Info("Stop SlotGamesNode Service Started...");
            try
            {
                await _bootstrapActor.Ask<bool>(new SlotsNodeShuttingDownMsg(), TimeSpan.FromSeconds(60));
            }
            catch (Exception ex)
            {
                logger.Error("Exception has been occured in Stopping bootstrap actor {0}", ex.ToString());
            }

            logger.Info("Leaving from cluster....");
            var cluster = Akka.Cluster.Cluster.Get(_slotGameActorSystem);
            await cluster.LeaveAsync();

        }
    }
}
