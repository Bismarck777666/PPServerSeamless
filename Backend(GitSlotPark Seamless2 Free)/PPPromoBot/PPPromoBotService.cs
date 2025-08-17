using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Akka.Actor;
using Akka.Configuration;
using System.IO;
using PPPromoBot.Database;

namespace PPPromoBot
{
    public class PPPromoBotService
    {
        private ActorSystem _ppPromoBotActorSystem;
        private IActorRef   _bootstrapActor;
        public bool Start()
        {
            var logger = NLog.LogManager.GetCurrentClassLogger();
            logger.Info("Starting PPPromoBot Service...");

            //먼저 설정정보를 검사한다.
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

            var slotGamesConfig = clusterConfig.GetConfig("pppromobot");
            if(slotGamesConfig == null)
            {
                logger.Error("config.hocon doesn't contain pppromo bot configuration");
                return false;
            }

            //Redis 자료기지정보를 설정한다.
            var redisConfig = slotGamesConfig.GetConfig("redis");
            if(redisConfig == null)
            {
                logger.Error("config.hocon doesn't contain redis configuration");
                return false;
            }

            setRedisInfo(redisConfig);

            //액터시스템을 창조한다.
            _ppPromoBotActorSystem = PPPromoBotHostFactory.LaunchPPPromoBot(clusterConfig);

            //부트스트랩액터를 창조한다.
            _bootstrapActor = _ppPromoBotActorSystem.ActorOf(BootstrapActor.Props(slotGamesConfig), "bootstrapper");
            _bootstrapActor.Tell("startService");
            return true;
        }

        private void setRedisInfo(Config redisConfig)
        {
            string strRedisIP       = redisConfig.GetString("ip", "127.0.0.1");
            int redisPort           = redisConfig.GetInt("port", 6379);
            string redisPassword    = redisConfig.GetString("password", "");
            RedisDatabase.setRedisInfo(strRedisIP, redisPort, redisPassword, new List<int>(redisConfig.GetIntList("databases")));
        }

        public Task TerminationHandle => _ppPromoBotActorSystem.WhenTerminated;

        public async Task StopAsync()
        {
            var logger = NLog.LogManager.GetCurrentClassLogger();
            logger.Info("Stop SlotGamesNode Service Started...");
            try
            {
                await _bootstrapActor.Ask<bool>(new PPPromoBotShuttingDownMsg(), TimeSpan.FromSeconds(60));
            }
            catch (Exception ex)
            {
                logger.Error("Exception has been occured in Stopping bootstrap actor {0}", ex.ToString());
            }

            await _ppPromoBotActorSystem.Terminate();
        }    
    }

    public class PPPromoBotShuttingDownMsg
    {

    }
}
