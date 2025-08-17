using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Akka.Actor;
using Akka.Configuration;
using System.IO;
using PromoNode.Database;
using GITProtocol;

namespace PromoNode
{
    public class PromoNodeService
    {
        private ActorSystem _promoActorSystem;
        private IActorRef   _bootstrapActor;
        public bool Start()
        {
            var logger = NLog.LogManager.GetCurrentClassLogger();
            logger.Info("Starting PromoNode Service...");

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

            var slotGamesConfig = clusterConfig.GetConfig("promo");
            if(slotGamesConfig == null)
            {
                logger.Error("config.hocon doesn't contain promo node configuration");
                return false;
            }

            var redisConfig = slotGamesConfig.GetConfig("redis");
            if(redisConfig == null)
            {
                logger.Error("config.hocon doesn't contain redis configuration");
                return false;
            }

            setRedisInfo(redisConfig);

            _promoActorSystem = PromoHostFactory.LaunchSlotGamesNode(clusterConfig);

            _bootstrapActor = _promoActorSystem.ActorOf(BootstrapActor.Props(slotGamesConfig), "bootstrapper");
            _bootstrapActor.Tell("startService");
            return true;
        }

        private void setRedisInfo(Config redisConfig)
        {
            string strRedisIP       = redisConfig.GetString("ip", "127.0.0.1");
            int redisPort           = redisConfig.GetInt("port", 6379);
            string redisPassword    = redisConfig.GetString("password", "");
            RedisDatabase.setRedisInfo(strRedisIP, redisPort, redisPassword);
        }

        public Task TerminationHandle => _promoActorSystem.WhenTerminated;

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
            var cluster = Akka.Cluster.Cluster.Get(_promoActorSystem);
            await cluster.LeaveAsync();

        }
    }
}
