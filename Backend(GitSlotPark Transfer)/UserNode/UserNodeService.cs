using Akka;
using Akka.Actor;
using Akka.Configuration;
using NLog;
using System;
using System.IO;
using System.Threading.Tasks;
using UserNode.Database;

namespace UserNode
{
    public class UserNodeService
    {
        private ActorSystem _connectSystem;
        private IActorRef   _bootstrapActor;

        public bool Start()
        {
            Logger logger = LogManager.GetCurrentClassLogger();
            logger.Info("Starting Connect Service...");

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

            Config userNodeConfig = clusterConfig.GetConfig("user");
            if (userNodeConfig == null)
            {
                logger.Error("config.hocon doesn't contain connector server configuration");
                return false;
            }

            Config redisConfig = userNodeConfig.GetConfig("redis");
            if (redisConfig == null)
            {
                logger.Error("config.hocon doesn't contain redis configuration");
                return false;
            }
            setRedisInfo(redisConfig);

            _connectSystem  = UserHostFactory.LauchUserNode(clusterConfig);

            _bootstrapActor = _connectSystem.ActorOf(BootstrapActor.Props(userNodeConfig), "bootstrapper");
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

        public Task TerminationHandle => _connectSystem.WhenTerminated;

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

            await CoordinatedShutdown.Get(_connectSystem).Run(CoordinatedShutdown.ClrExitReason.Instance);
        }
    }
}
