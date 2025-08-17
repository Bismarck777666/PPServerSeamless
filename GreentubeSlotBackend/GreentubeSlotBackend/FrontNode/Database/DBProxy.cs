﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Akka.Actor;
using Akka.Configuration;
using Akka.Event;
using System.Data.SqlClient;
using Akka.Routing;

namespace FrontNode.Database
{
    public class DBProxy : ReceiveActor
    {
        private string      _strConnectionString        = "";
        private IActorRef   _readerRouter;
        private IActorRef   _monitorActor;

        private int         _poolSize                   = 0;
        private readonly ILoggingAdapter _logger = Logging.GetLogger(Context);

        public DBProxy(Config config)
        {
            SqlConnectionStringBuilder connStringBuilder = new SqlConnectionStringBuilder();
            connStringBuilder.DataSource        = config.GetString("ip", "127.0.0.1");
            connStringBuilder.InitialCatalog    = config.GetString("dbname", "xe888");
            connStringBuilder.UserID            = config.GetString("user", "root");
            connStringBuilder.Password          = config.GetString("pass", "akduifwkro");
            _poolSize                           = (int)  config.GetInt("poolcount", 2);
            _strConnectionString                = connStringBuilder.ConnectionString;

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
                if (!await checkDBConnection())
                {
                    _logger.Error("Can not connect to database. Please check if database is correctly configured.");
                    return;
                }

                await _monitorActor.Ask("initialize");
                Sender.Tell(new ReadyDBProxy(_readerRouter));
            }
            else if (command == "terminate")
            {
                _monitorActor.Tell(PoisonPill.Instance);
                _readerRouter.Tell(new Broadcast(PoisonPill.Instance));
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
            catch(Exception)
            {
                return false;
            }
        }

        protected override void PreStart()
        {
            _readerRouter = Context.ActorOf(DBProxyReader.Props(_strConnectionString, _poolSize), "readers");
            _monitorActor = Context.ActorOf(DBProxyMonitor.Props(_strConnectionString),           "monitor");

            Context.Watch(_readerRouter);
            Context.Watch(_monitorActor);
            base.PreStart();
        }
        
        #region Messages
        public class ReadyDBProxy
        {
            public IActorRef Reader { get; private set; }
            public ReadyDBProxy(IActorRef reader)
            {
                this.Reader         = reader;
            }
        }
        #endregion

    }
}
