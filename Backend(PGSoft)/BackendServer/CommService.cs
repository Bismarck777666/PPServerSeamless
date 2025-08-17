using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Akka.Actor;
using Akka.Configuration;
using System.IO;
using SlotGamesNode.Database;
using Topshelf;
using Akka.Event;

namespace SlotGamesNode
{
    public class CommService
    {
        private ActorSystem                 _connectSystem;
        private IActorRef                   _bootstrapActor;

        public bool Start()
        {
            var logger = NLog.LogManager.GetCurrentClassLogger();
            logger.Info("Starting Connect Service...");

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

            var connectorConfig = clusterConfig.GetConfig("comm");
            if(connectorConfig == null)
            {
                logger.Error("config.hocon doesn't contain connector server configuration");
                return false;
            }

            var redisConfig = connectorConfig.GetConfig("redis");
            if (redisConfig == null)
            {
                logger.Error("config.hocon doesn't contain redis configuration");
                return false;
            }
            setRedisInfo(redisConfig);

            _connectSystem = CommHostFactory.LauchCommNode(clusterConfig);

            _bootstrapActor = _connectSystem.ActorOf(BootstrapActor.Props(connectorConfig), "bootstrapper");
            _bootstrapActor.Tell("startService");
            return true;
        }
        private void setRedisInfo(Config redisConfig)
        {
            string strRedisIP = redisConfig.GetString("ip", "127.0.0.1");
            int redisPort = redisConfig.GetInt("port", 6379);
            string redisPassword = redisConfig.GetString("password", "");
            int database = redisConfig.GetInt("database", 0);

            RedisDatabase.setRedisInfo(strRedisIP, redisPort, redisPassword, database);
        }
        public Task TerminationHandle => _connectSystem.WhenTerminated;

        public async Task StopAsync()
        {
            var logger = NLog.LogManager.GetCurrentClassLogger();
            logger.Info("Stop Service Started...");
            try
            {
                await _bootstrapActor.Ask<bool>(new ShutdownSystemMessage(), TimeSpan.FromSeconds(60));
            }
            catch(Exception ex)
            {
                logger.Error("Exception has been occured in Stopping bootstrap actor {0}", ex.ToString());
            }
            await CoordinatedShutdown.Get(_connectSystem).Run(CoordinatedShutdown.ClrExitReason.Instance);    
        }


    }
}
