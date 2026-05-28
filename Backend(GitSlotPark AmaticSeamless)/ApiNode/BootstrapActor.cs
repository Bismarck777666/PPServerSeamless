using Akka.Actor;
using Akka.Configuration;
using Akka.Event;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Text;
using System.Threading.Tasks;
using QueenApiNode.Database;
using Akka.Routing;
using QueenApiNode.HttpService;
using Microsoft.Owin.Hosting;
using GITProtocol;
using QueenApiNode.Agent;

namespace QueenApiNode
{
    public class BootstrapActor : ReceiveActor
    {
        private Config                      _configuration      = null;
        private readonly ILoggingAdapter    _logger             = Logging.GetLogger(Context);
        private IActorRef                   _dbProxy            = null;
        private IDisposable                 _httpWebService     = null;
        private IActorRef                   _agentManager       = null;


        public BootstrapActor(Config configuration)
        {
            _configuration = configuration;

            Receive<string>(command =>
            {
                processCommand(command);
            });
            Receive<DBProxy.ReadyDBProxy>(dbActors =>
            {
                _logger.Info("Database Proxy has been successfully initialized.");

                _agentManager               = Context.System.ActorOf(Akka.Actor.Props.Create(() => new AgentManager()), "agentManager");
                _agentManager.Tell(DBMonitorSnapshot.Instance.AgentHashKeys);

                ApiConfig.WorkActorGroup    = Context.System.ActorOf(Akka.Actor.Props.Create(() => new HTTPWorkActor(dbActors.Reader, dbActors.Writer)).WithRouter(FromConfig.Instance), "httpWorkers");

                //网络请求服务
                var httpConfig = _configuration.GetConfig("http");
                if (httpConfig != null)
                {
                    string baseAddress = httpConfig.GetString("baseurl", "http://127.0.0.1/");
                    _httpWebService = WebApp.Start<Startup>(url: baseAddress);
                }
                var gameFrontConfig = _configuration.GetConfig("gameFront");
                if (gameFrontConfig != null)
                {
                    ApiConfig.GameFrontUrl  = gameFrontConfig.GetString("url");
                    ApiConfig.FrontTokenKey = gameFrontConfig.GetString("tokenkey");
                }

            });
            ReceiveAsync<APINodeShutdownMsg>(onShutdownSystem);
        }

        public static Props Props(Config config)
        {
            return Akka.Actor.Props.Create(() => new BootstrapActor(config));
        }
        private void processCommand(string strCommand)
        {
            if (strCommand == "startService")
            {
                var dbConfig = _configuration.GetConfig("database");
                if (dbConfig == null)
                {
                    _logger.Error("config.hocon doesn't contain database configuration");
                    return;
                }

                _logger.Info("Initializing database proxy...");

                //第一步初始化数据库连接部分。
                _dbProxy = Context.System.ActorOf(DBProxy.Props(dbConfig), "dbproxy");
                _dbProxy.Tell("initialize");
            }
        }
        private async Task onShutdownSystem(APINodeShutdownMsg message)
        {
            try
            {
                //首先关闭套接字服务器。
                _logger.Info("Shutting down tcp and web socket server...");

                if (_httpWebService != null)
                    _httpWebService.Dispose();

                await ApiConfig.WorkActorGroup.GracefulStop(TimeSpan.FromSeconds(300), new Broadcast("terminate"));

                //停止基础数据库角色。
                _logger.Info("Terminating database proxy actors....");
                await _dbProxy.GracefulStop(TimeSpan.FromSeconds(3600), "terminate");

                //从集群中退出。
                _logger.Info("Leaving from cluster....");
                var cluster = Akka.Cluster.Cluster.Get(Context.System);
                await cluster.LeaveAsync();
            }
            catch (Exception)
            {

            }
            Sender.Tell(true);
        }
    }

    public class ShutdownSystemMessage
    {

    }
}
