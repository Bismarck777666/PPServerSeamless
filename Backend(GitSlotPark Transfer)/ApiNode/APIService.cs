﻿using Akka.Actor;
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

            //먼저 설정정보를 검사한다.
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

            //먼저 설정파일에서 액터시스템의 이름을 얻는다.
            string systemName = apiNodeConfig.GetString("actorsystem", "godgaming");

            //액터시스템을 창조한다.
            _actorSystem    = ActorSystem.Create(systemName, clusterConfig);
            
            //부트스트랩액터를 창조한다.
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

            //클라스터에서 탈퇴한다.
            logger.Info("Leaving from cluster....");
            Akka.Cluster.Cluster cluster = Akka.Cluster.Cluster.Get(_actorSystem);
            await cluster.LeaveAsync();
        }
    }

    public class APINodeShutdownMsg
    {
    }
}
