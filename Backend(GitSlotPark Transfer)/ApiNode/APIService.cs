using Akka.Actor;
using Akka.Configuration;
using NLog;
using System;
using System.IO;
using System.Threading.Tasks;

namespace QueenApiNode
{
    public class APIService
    {
        private ActorSystem _actorSystem;
        private IActorRef _bootstrapActor;

        public bool Start()
        {
            Logger currentClassLogger = LogManager.GetCurrentClassLogger();
            currentClassLogger.Info("Starting API Backend Service...");

            //首先检查配置信息。
            Config clusterConfig;
            try
            {
                clusterConfig = ConfigurationFactory.ParseString(File.ReadAllText("config.hocon"));
            }
            catch
            {
                currentClassLogger.Error("Please make sure if config.hocon file is correctly formatted");
                return false;
            }

            Config apiNodeConfig = clusterConfig.GetConfig("apinode");
            if (apiNodeConfig == null)
            {
                currentClassLogger.Error("config.hocon doesn't contain apinode configuration");
                return false;
            }

            //首先从配置文件中获取参与者系统的名称。
            string systemName = apiNodeConfig.GetString("actorsystem", "godgaming");

            //创建参与者系统。
            _actorSystem    = ActorSystem.Create(systemName, clusterConfig);
            
            //创建引导参与者。
            _bootstrapActor = _actorSystem.ActorOf(BootstrapActor.Props(apiNodeConfig), "bootstrapper");
            _bootstrapActor.Tell("startService");
            return true;
        }

        public Task TerminationHandle => _actorSystem.WhenTerminated;

        public async Task StopAsync()
        {
            Logger logger = LogManager.GetCurrentClassLogger();
            logger.Info("Stop QueenApiNode Service Started...");
            try
            {
                await _bootstrapActor.Ask<bool>(new APINodeShutdownMsg(), TimeSpan.FromSeconds(60.0));
            }
            catch (Exception ex)
            {
                logger.Error("Exception has been occured in Stopping bootstrap actor {0}", ex.ToString());
            }

            //从集群中退出。
            logger.Info("Leaving from cluster....");
            Akka.Cluster.Cluster cluster = Akka.Cluster.Cluster.Get(_actorSystem);
            await cluster.LeaveAsync();
        }
    }

    public class APINodeShutdownMsg
    {
    }
}
