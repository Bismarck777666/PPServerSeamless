using Akka.Actor;
using Akka.Configuration;
using Akka.Event;
using Akka.Routing;
using System;
using System.Data.SqlClient;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace QueenApiNode.Database
{
    public class DBProxy : ReceiveActor
    {
        private string                      _strConnectionString    = "";
        private IActorRef                   _readerRouter;
        private IActorRef                   _monitorActor;
        private IActorRef                   _writeActor;
        private int                         _poolSize               = 0;

        private readonly ILoggingAdapter    _logger                 = Context.GetLogger();

        public DBProxy(Config config)
        {
            SqlConnectionStringBuilder connectionStringBuilder = new SqlConnectionStringBuilder();
            connectionStringBuilder.DataSource      = config.GetString("ip", "127.0.0.1");
            connectionStringBuilder.InitialCatalog  = config.GetString("dbname", "xe888");
            connectionStringBuilder.UserID          = config.GetString("user", "root");
            connectionStringBuilder.Password        = config.GetString("pass", "akduifwkro");
            _poolSize                               = config.GetInt("poolcount", 2);
            _strConnectionString                    = connectionStringBuilder.ConnectionString;
            
            ReceiveAsync<string>(processCommand);
        }

        public static Props Props(Config config)
        {
            return Akka.Actor.Props.Create(() => new DBProxy(config));
        }

        private async Task processCommand(string command)
        {
            if(command == "initialize")
            {
                //자료기지서버의 련결가능성을 검사한다.
                if (!await checkDBConnection())
                {
                    _logger.Error("Can not connect to database. Please check if database is correctly configured.");
                    return;
                }

                //모니터액토를 초기화
                await _monitorActor.Ask("initialize");
                Sender.Tell(new ReadyDBProxy(_readerRouter, _writeActor));
            }
            else if(command == "terminate")
            {
                _monitorActor.Tell(PoisonPill.Instance);
                _readerRouter.Tell(new Broadcast(PoisonPill.Instance));
                _writeActor.Tell("terminate");

                Become(new Action(ShuttingDown));
            }
        }

        private void ShuttingDown()
        {
            Receive<Terminated>(terminated =>
            {
                if (_monitorActor != null && terminated.ActorRef.Equals(_monitorActor))
                    _monitorActor = null;
                else if (_readerRouter != null && terminated.ActorRef.Equals(_readerRouter))
                    _readerRouter = null;

                if (_monitorActor == null && _readerRouter == null)
                    Self.Tell(PoisonPill.Instance);
            });
        }

        private async Task<bool> checkDBConnection()
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(_strConnectionString))
                {
                    await connection.OpenAsync();
                }
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        protected override void PreStart()
        {
            //자식액터들을 창조한다.
            _readerRouter   = Context.ActorOf(DBProxyReader.Props(_strConnectionString, _poolSize), "readers");
            _monitorActor   = Context.ActorOf(DBProxyMonitor.Props(_strConnectionString),           "monitor");
            _writeActor     = Context.ActorOf(DBProxyWriter.Props(_strConnectionString),            "writer");
            
            Context.Watch(_readerRouter);
            Context.Watch(_monitorActor);
            Context.Watch(_writeActor);
            
            base.PreStart();
        }

        public class ReadyDBProxy
        {
            public IActorRef Reader { get; private set; }
            public IActorRef Writer { get; private set; }

            public ReadyDBProxy(IActorRef reader, IActorRef writer)
            {
                Reader = reader;
                Writer = writer;
            }
        }
    }
}
