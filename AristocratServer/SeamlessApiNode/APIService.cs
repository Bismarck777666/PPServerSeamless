using Akka.Actor;
using Akka.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using QueenApiNode.Database;

namespace QueenApiNode
{
    public class APIService
    {
        private ActorSystem _actorSystem;
        private IActorRef _bootstrapActor;

        public bool Start()
        {
            var logger = NLog.LogManager.GetCurrentClassLogger();
            logger.Info("Starting API Backend Service...");

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

            var apiNodeConfig = clusterConfig.GetConfig("apinode");
            if (apiNodeConfig == null)
            {
                logger.Error("config.hocon doesn't contain apinode configuration");
                return false;
            }

            string systemName = apiNodeConfig.GetString("actorsystem", "godgaming");

            _actorSystem = ActorSystem.Create(systemName, clusterConfig);

            _bootstrapActor = _actorSystem.ActorOf(BootstrapActor.Props(apiNodeConfig), "bootstrapper");
            _bootstrapActor.Tell("startService");

            return true;
        }
        public Task TerminationHandle => _actorSystem.WhenTerminated;

        public async Task StopAsync()
        {
            var logger = NLog.LogManager.GetCurrentClassLogger();
            logger.Info("Stop QueenApiNode Service Started...");
            try
            {
                await _bootstrapActor.Ask<bool>(new APINodeShutdownMsg(), TimeSpan.FromSeconds(60));
            }
            catch (Exception ex)
            {
                logger.Error("Exception has been occured in Stopping bootstrap actor {0}", ex.ToString());
            }

            logger.Info("Leaving from cluster....");
            var cluster = Akka.Cluster.Cluster.Get(_actorSystem);
            await cluster.LeaveAsync();
        }
    }

    public class APINodeShutdownMsg
    {

    }

}
