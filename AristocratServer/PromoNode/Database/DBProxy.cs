using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Akka.Actor;
using Akka.Configuration;
using Akka.Event;
using System.Data.SqlClient;
using Akka.Routing;

namespace PromoNode.Database
{
    public class DBProxy : ReceiveActor
    {
        private string                      _strConnectionString        = "";
        private int                         _poolSize                   = 5;
        private IActorRef                   _readerRouter;
        private IActorRef                   _writeActor;
        private IActorRef                   _monitorActor;
        private readonly ILoggingAdapter    _logger                     = Logging.GetLogger(Context);

        public DBProxy(Config config)
        {
            SqlConnectionStringBuilder connStringBuilder    = new SqlConnectionStringBuilder();
            connStringBuilder.DataSource                    = config.GetString("ip",            "127.0.0.1");
            connStringBuilder.InitialCatalog                = config.GetString("dbname",        "gitigaming");
            connStringBuilder.UserID                        = config.GetString("user",          "root");
            connStringBuilder.Password                      = config.GetString("pass",          "akduifwkro");
            _poolSize                                       = (int)config.GetInt("poolcount",   2);
            _strConnectionString                            = connStringBuilder.ConnectionString;

            ReceiveAsync<string>(processCommand);
        }

        public static Props Props(Config config)
        {
            return Akka.Actor.Props.Create(() => new DBProxy(config));
        }
        private async Task processCommand(string command)
        {
            if (command == "initialize")
            {
                if(!await checkDBConnection())
                {
                    _logger.Error("Can not connect to database. Please check if database is correctly configured.");
                    return;
                }

                await _monitorActor.Ask("initialize");
                Sender.Tell(new ReadyDBProxy(_readerRouter, _writeActor));
            }
            else if (command == "terminate")
            {
                _monitorActor.Tell(PoisonPill.Instance);
                _readerRouter.Tell(new Broadcast(PoisonPill.Instance));
                _writeActor.Tell(PoisonPill.Instance);

                Become(ShuttingDown);
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

                if (_monitorActor == null && _readerRouter == null && _writeActor == null)
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
            catch(Exception)
            {
                return false;
            }
        }

        protected override void PreStart()
        {
            _readerRouter = Context.ActorOf(DBProxyReader.Props(_strConnectionString, _poolSize), "readers");
            _writeActor   = Context.ActorOf(DBProxyWriter.Props(_strConnectionString),            "writer");
            _monitorActor = Context.ActorOf(DBProxyMonitor.Props(_strConnectionString),           "monitor");

            Context.Watch(_readerRouter);
            Context.Watch(_writeActor);
            Context.Watch(_monitorActor);
            base.PreStart();
        }
        
        #region Messages
        public class ReadyDBProxy
        {
            public IActorRef Reader { get; private set; }
            public IActorRef Writer { get; private set; }

            public ReadyDBProxy(IActorRef reader, IActorRef writer)
            {
                this.Reader = reader;
                this.Writer = writer;
            }
        }
        #endregion

    }
}
