using Akka.Actor;
using Akka.Event;
using GITProtocol;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Text;
using System.Threading.Tasks;

namespace QueenApiNode.Database
{
    public class DBProxyMonitor : ReceiveActor
    {
        private string          _strConnString      = "";
        private ICancelable     _monitorCancelable  = null;
        private readonly ILoggingAdapter _logger = Logging.GetLogger(Context);

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
            if (strCommand == "initialize")
            {
                await initializeMonitor();
                _monitorCancelable = Context.System.Scheduler.ScheduleTellRepeatedlyCancelable(5000, 5000, Self, "monitor", ActorRefs.NoSender);
                Sender.Tell("dbInitialized");
            }
            else if (strCommand == "monitor")
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

                    string strQuery = "SELECT username, authtoken, updatetime FROM agents WHERE authtoken is not null ORDER BY updatetime";
                    var command = new SqlCommand(strQuery, connection);
                    using(var reader = await command.ExecuteReaderAsync())
                    {
                        while(await reader.ReadAsync())
                        {
                            DBMonitorSnapshot.Instance.AgentHashKeys.Add((string)reader["username"], (string)reader["authtoken"]);
                            DBMonitorSnapshot.Instance.LastAgentUpdateTime = (DateTime)reader["updatetime"]; 
                        }
                    }

                    strQuery = "SELECT gameid, gamename, gametype, openclose, gamesymbol, updatetime,publishtime FROM gameconfigs ORDER BY publishtime DESC";
                    command = new SqlCommand(strQuery, connection);
                    using (DbDataReader reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            int             gameID      = (int)         reader["gameid"];
                            DateTime        updateTime  = (DateTime)    reader["updatetime"];
                            DateTime        publishtime = (DateTime)    reader["publishtime"];
                            int             gameType    = (int)         reader["gametype"];
                            string          gameSymbol  = (string)      reader["gamesymbol"];
                            string          gameName    = (string)      reader["gamename"];

                            DBMonitorSnapshot.Instance.setGameInfo(gameID, gameType, gameSymbol, gameName, publishtime);
                            if (DBMonitorSnapshot.Instance.GameConfigUpdateTime < updateTime)
                                DBMonitorSnapshot.Instance.GameConfigUpdateTime = updateTime;
                        }
                    }

                    strQuery    = "SELECT * FROM agentgameopens";
                    command     = new SqlCommand(strQuery, connection);
                    using(DbDataReader reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            int         gameID      = (int)reader["gameid"];
                            int         agentID     = (int)reader["agentid"];
                            int         isClose     = (int)reader["isclose"];
                            DateTime    updateTime  = (DateTime)reader["updatetime"];

                            DBMonitorSnapshot.Instance.setAgentGameOpenClose(agentID, gameID, isClose);
                            if (DBMonitorSnapshot.Instance.AgentGameCloseUpdateTime < updateTime)
                                DBMonitorSnapshot.Instance.AgentGameCloseUpdateTime = updateTime;
                        }
                    }
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

                    string  strQuery = "SELECT username, authtoken, updatetime FROM agents WHERE (authtoken is not null) and (updatetime > @updatetime) ORDER BY updatetime";
                    var     command  = new SqlCommand(strQuery, connection);
                    command.Parameters.AddWithValue("@updatetime", DBMonitorSnapshot.Instance.LastAgentUpdateTime);
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            string strAgentID   = (string)reader["username"];
                            string strAuthToken = (string)reader["authtoken"];

                            Context.System.ActorSelection("/user/agentManager").Tell(new AuthTokenUpdated(strAgentID, strAuthToken));
                            DBMonitorSnapshot.Instance.LastAgentUpdateTime = (DateTime)reader["updatetime"];
                        }
                    }

                    strQuery    = "SELECT gameid, gamename, gametype, openclose, gamesymbol, updatetime, publishtime FROM gameconfigs WHERE updatetime > @updatetime ORDER BY publishtime DESC";
                    command     = new SqlCommand(strQuery, connection);
                    command.Parameters.AddWithValue("@updatetime", DBMonitorSnapshot.Instance.GameConfigUpdateTime);
                    using (DbDataReader reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            int         gameID      = (int)     reader["gameid"];
                            int         gameType    = (int)     reader["gametype"];
                            string      gameSymbol  = (string)  reader["gamesymbol"];
                            string      gameName    = (string)  reader["gamename"];
                            DateTime    publishtime = (DateTime)reader["publishtime"];

                            DBMonitorSnapshot.Instance.setGameInfo(gameID, gameType, gameSymbol, gameName, publishtime);
                            DBMonitorSnapshot.Instance.GameConfigUpdateTime = (DateTime)reader["updatetime"];
                        }
                    }

                    strQuery    = "SELECT * FROM agentgameopens WHERE updatetime > @updatetime ORDER BY updatetime";
                    command     = new SqlCommand(strQuery, connection);
                    command.Parameters.AddWithValue("@updatetime", DBMonitorSnapshot.Instance.AgentGameCloseUpdateTime);

                    using (DbDataReader reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            int agentID = (int)reader["agentid"];
                            int gameID  = (int)reader["gameid"];
                            int isClose = (int)reader["isclose"];

                            DBMonitorSnapshot.Instance.setAgentGameOpenClose(agentID, gameID, isClose);
                            DBMonitorSnapshot.Instance.AgentGameCloseUpdateTime = (DateTime)reader["updatetime"];
                        }
                    }
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
            AgentID   = agentID;
            AuthToken = authToken;
        }
    }
}
