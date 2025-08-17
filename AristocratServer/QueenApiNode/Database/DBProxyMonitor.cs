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

                    string strQuery = "SELECT username, authtoken, whitelist, state, updatetime FROM agents WHERE authtoken is not null ORDER BY updatetime";
                    var command = new SqlCommand(strQuery, connection);
                    using(var reader = await command.ExecuteReaderAsync())
                    {
                        while(await reader.ReadAsync())
                        {
                            DBMonitorSnapshot.Instance.AgentHashKeys.Add((string)reader["username"], (string)reader["authtoken"]);
                            DBMonitorSnapshot.Instance.LastAgentUpdateTime = (DateTime)reader["updatetime"]; 
                        }
                    }

                    if (DBMonitorSnapshot.Instance.GameConfigUpdateTime == new DateTime(1, 1, 1))
                    {
                        strQuery = "SELECT gameid, gametype, openclose, gamesymbol, updatetime FROM gameconfigs";
                        command  = new SqlCommand(strQuery, connection);
                        using (DbDataReader reader = await command.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                int         gameID      = (int)reader["gameid"];
                                DateTime    updateTime  = (DateTime)reader["updatetime"];
                                GAMETYPE    gameType    = (GAMETYPE)(int)reader["gametype"];
                                string      gameSymbol  = (string)reader["gamesymbol"];

                                DBMonitorSnapshot.Instance.setGameType(gameID, gameType);
                                DBMonitorSnapshot.Instance.setGameSymbol(gameSymbol, gameType, gameID);
                                if (DBMonitorSnapshot.Instance.GameConfigUpdateTime < updateTime)
                                    DBMonitorSnapshot.Instance.GameConfigUpdateTime = updateTime;
                            }
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

                    string  strQuery = "SELECT username, authtoken, whitelist, state, updatetime FROM agents WHERE (authtoken is not null) and (updatetime > @updatetime) ORDER BY updatetime";
                    var     command  = new SqlCommand(strQuery, connection);
                    command.Parameters.AddWithValue("@updatetime", DBMonitorSnapshot.Instance.LastAgentUpdateTime);
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            string strAgentID   = (string)reader["username"];
                            string strAuthToken = (string)reader["authtoken"];
                            string strWhiteList = (string)reader["whitelist"];
                            int state           = (int)reader["state"];

                            Context.System.ActorSelection("/user/agentManager").Tell(new AuthTokenUpdated(strAgentID, strAuthToken, strWhiteList, state));
                            DBMonitorSnapshot.Instance.LastAgentUpdateTime = (DateTime)reader["updatetime"];
                        }
                    }

                    strQuery = "SELECT gameid, gametype, openclose, gamesymbol, updatetime FROM gameconfigs WHERE updatetime > @updatetime ORDER BY updatetime";
                    command  = new SqlCommand(strQuery, connection);
                    command.Parameters.AddWithValue("@updatetime", DBMonitorSnapshot.Instance.GameConfigUpdateTime);
                    using (DbDataReader reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            int gameID = (int)reader["gameid"];
                            GAMETYPE gameType = (GAMETYPE)(int)reader["gametype"];
                            string gameSymbol = (string)reader["gamesymbol"];

                            DBMonitorSnapshot.Instance.setGameType(gameID, gameType);
                            DBMonitorSnapshot.Instance.setGameSymbol(gameSymbol, gameType, gameID);
                            DBMonitorSnapshot.Instance.GameConfigUpdateTime = (DateTime)reader["updatetime"];
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
        public string   AgentID         { get; set; }
        public string   AuthToken       { get; set; }
        public string   WhilteList      { get; set; }
        public int      State           { get; set; }

        public AuthTokenUpdated(string agentID, string authToken, string whiteList, int state)
        {
            AgentID     = agentID;
            AuthToken   = authToken;
            WhilteList  = whiteList;
            State       = state;
        }
    }
}
