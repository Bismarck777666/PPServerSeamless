using Akka.Actor;
using Akka.Event;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace QueenApiNode.Database
{
    public class DBProxyMonitor : ReceiveActor
    {
        private string                      _strConnString      = "";
        private ICancelable                 _monitorCancelable  = null;
        private readonly ILoggingAdapter    _logger             = Context.GetLogger();

        public DBProxyMonitor(string strConnString)
        {
            _strConnString = strConnString;
            ReceiveAsync<string>(processCommand);
        }

        public static Props Props(string strConnString)
        {
            return Akka.Actor.Props.Create(() => new DBProxyMonitor(strConnString));
        }

        protected override void PreStart()
        {
            base.PreStart();
        }

        protected override void PostStop()
        {
            if (_monitorCancelable != null)
                _monitorCancelable.Cancel();

            base.PostStop();
        }

        protected override void PostRestart(Exception reason)
        {
            base.PostRestart(reason);
            Self.Tell("initialize");
        }

        private async Task processCommand(string strCommand)
        {
            if(strCommand == "initialize")
            {
                await initializeMonitor();
                _monitorCancelable = Context.System.Scheduler.ScheduleTellRepeatedlyCancelable(5000, 5000, Self, "monitor", ActorRefs.NoSender);
                Sender.Tell("dbInitialized");
            }
            else if(strCommand == "monitor")
            {
                await monitorTables();
            }
        }

        private async Task initializeMonitor()
        {
            try
            {
                DBMonitorSnapshot.Instance.AgentHashKeys = new Dictionary<string, string>();
                using (SqlConnection connection = new SqlConnection(_strConnString))
                {
                    await connection.OpenAsync();

                    string strQuery     = "SELECT username, authtoken, updatetime FROM agents WHERE authtoken is not null ORDER BY updatetime";
                    SqlCommand command  = new SqlCommand(strQuery, connection);
                    using (DbDataReader reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            DBMonitorSnapshot.Instance.AgentHashKeys.Add((string) reader["username"], (string) reader["authtoken"]);
                            DBMonitorSnapshot.Instance.LastAgentUpdateTime = (DateTime) reader["updatetime"];
                        }
                    }

                    strQuery    = "SELECT gameid, gamename, gametype, openclose, gamesymbol, updatetime, releasedate FROM gameconfigs";
                    command     = new SqlCommand(strQuery, connection);
                    using (DbDataReader reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            int         gameID      = (int)reader["gameid"];
                            DateTime    updateTime  = (DateTime)reader["updatetime"];
                            int         gameType    = (int)reader["gametype"];
                            string      gameSymbol  = (string)reader["gamesymbol"];
                            string      gameName    = (string)reader["gamename"];
                            DateTime    releaseDate = (DateTime)reader["releasedate"];

                            DBMonitorSnapshot.Instance.setGameInfo(gameID, gameType, gameSymbol, gameName, releaseDate);
                            if (DBMonitorSnapshot.Instance.GameConfigUpdateTime < updateTime)
                                DBMonitorSnapshot.Instance.GameConfigUpdateTime = updateTime;
                        }
                    }

                    DBMonitorSnapshot.Instance.sortGameInfos();
                }
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occurred in DBProxyMonitor::initializeMonitor {0}", ex);
            }
        }

        private async Task monitorTables()
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(_strConnString))
                {
                    await connection.OpenAsync();
                    
                    string      strQuery    = "SELECT username, authtoken, updatetime FROM agents WHERE (authtoken is not null) and (updatetime > @updatetime) ORDER BY updatetime";
                    SqlCommand  command     = new SqlCommand(strQuery, connection);
                    command.Parameters.AddWithValue("@updatetime", DBMonitorSnapshot.Instance.LastAgentUpdateTime);
                    using (DbDataReader reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            string strAgentID   = (string) reader["username"];
                            string strAuthToken = (string) reader["authtoken"];
                            Context.System.ActorSelection("/user/agentManager").Tell(new AuthTokenUpdated(strAgentID, strAuthToken));
                            DBMonitorSnapshot.Instance.LastAgentUpdateTime = (DateTime) reader["updatetime"];
                        }
                    }

                    strQuery    = "SELECT gameid, gamename, gametype, openclose, gamesymbol, releasedate, updatetime FROM gameconfigs WHERE updatetime > @updatetime ORDER BY updatetime";
                    command     = new SqlCommand(strQuery, connection);
                    command.Parameters.AddWithValue("@updatetime", DBMonitorSnapshot.Instance.GameConfigUpdateTime);
                    bool isGameConfigUpdated = false;

                    using(DbDataReader reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            int         gameID      = (int) reader["gameid"];
                            int         gameType    = (int) reader["gametype"];
                            string      gameSymbol  = (string) reader["gamesymbol"];
                            string      gameName    = (string) reader["gamename"];
                            DateTime    releaseDate = (DateTime) reader["releasedate"];
                            DBMonitorSnapshot.Instance.setGameInfo(gameID, gameType, gameSymbol, gameName, releaseDate);
                            DBMonitorSnapshot.Instance.GameConfigUpdateTime = (DateTime) reader["updatetime"];
                            isGameConfigUpdated = true;
                        }
                    }
                    
                    if (isGameConfigUpdated)
                        DBMonitorSnapshot.Instance.sortGameInfos();
                }
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occurred in DBProxyMonitor::monitorTables {0}", ex);
            }
        }
    }

    public class AuthTokenUpdated
    {
        public string AgentID   { get; set; }
        public string AuthToken { get; set; }

        public AuthTokenUpdated(string agentID, string authToken)
        {
            AgentID     = agentID;
            AuthToken   = authToken;
        }
    }
}
