using Akka.Actor;
using Akka.Configuration;
using Akka.Event;
using Akka.Routing;
using System;
using System.Data.SqlClient;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace SlotGamesNode.Database
{
    public class DBProxy : ReceiveActor
    {
        private string      _strConnectionString    = "";
        private IActorRef   _readerRouter;
        private IActorRef   _writeActor;
        private IActorRef   _monitorActor;
        private int         _poolSize               = 0;
        private readonly ILoggingAdapter _logger = Context.GetLogger();

        public DBProxy(Config config)
        {
            SqlConnectionStringBuilder connectionStringBuilder = new SqlConnectionStringBuilder();
            connectionStringBuilder.DataSource      = config.GetString("ip", "127.0.0.1");
            connectionStringBuilder.InitialCatalog  = config.GetString("dbname", "gitigaming");
            connectionStringBuilder.UserID          = config.GetString("user", "root");
            connectionStringBuilder.Password        = config.GetString("pass", "akduifwkro");
            
            _poolSize               = config.GetInt("poolcount", 2);
            _strConnectionString    = connectionStringBuilder.ConnectionString;

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
                if (!await checkDBConnection())
                {
                    _logger.Error("Can not connect to database. Please check if database is correctly configured.");
                    return;
                }

                await _monitorActor.Ask("initialize");
                Sender.Tell(new ReadyDBProxy(_readerRouter, _writeActor));
            }
            else if(command == "terminate")
            {
                _monitorActor.Tell(PoisonPill.Instance);
                _readerRouter.Tell(new Broadcast(PoisonPill.Instance));
                _writeActor.Tell(PoisonPill.Instance);

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
                else if (_writeActor != null && terminated.ActorRef.Equals(_writeActor))
                    _writeActor = null;
                if (_monitorActor != null || _readerRouter != null || _writeActor != null)
                    return;
                Self.Tell(PoisonPill.Instance);
            });
        }

        private async Task<bool> checkDBConnection()
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(_strConnectionString))
                    await connection.OpenAsync();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        protected override void PreStart()
        {
            _readerRouter   = Context.ActorOf(DBProxyReader.Props(_strConnectionString, _poolSize), "readers");
            _writeActor     = Context.ActorOf(DBProxyWriter.Props(_strConnectionString),            "writer");
            _monitorActor   = Context.ActorOf(DBProxyMonitor.Props(_strConnectionString),           "monitor");
            
            Context.Watch(_readerRouter);
            Context.Watch(_writeActor);
            Context.Watch(_monitorActor);
            
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
