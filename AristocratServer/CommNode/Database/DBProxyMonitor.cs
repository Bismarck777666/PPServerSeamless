using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Akka.Actor;
using System.Data.SqlClient;
using System.Data.Common;
using GITProtocol;
using Akka.Event;

namespace CommNode.Database
{
    public class DBProxyMonitor : ReceiveActor
    {
        private string                      _strConnString          = "";
        private ICancelable                 _monitorCancelable      = null;
        private readonly ILoggingAdapter    _logger                 = Logging.GetLogger(Context);

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
                _monitorCancelable      = Context.System.Scheduler.ScheduleTellOnceCancelable(5000, Self, "monitor", ActorRefs.NoSender);
                Sender.Tell("dbInitialized");
            }
            else if(strCommand == "monitor")
            {
                await monitorTables();
            }

        }        
        private async Task initializeMonitor()
        {
            using (SqlConnection connection = new SqlConnection(_strConnString))
            {
                await connection.OpenAsync();
                
                if (DBMonitorSnapshot.Instance.LastQuitUserID == -1)
                {
                    string strQuery = "SELECT TOP 1 id FROM quitusers ORDER BY id DESC";
                    SqlCommand command = new SqlCommand(strQuery, connection);
                    using (DbDataReader reader = await command.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            DBMonitorSnapshot.Instance.LastQuitUserID = reader.GetInt64(0);
                        }
                    }
                }
                if (DBMonitorSnapshot.Instance.GameConfigUpdateTime == new DateTime(1, 1, 1))
                {
                    DBMonitorSnapshot.Instance.GameConfigUpdateTime = new DateTime(1970, 1, 1);

                    string strQuery         = "SELECT gameid, gametype, openclose, gamesymbol, gamedata, updatetime FROM gameconfigs";
                    SqlCommand command      = new SqlCommand(strQuery, connection);
                    using (DbDataReader reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            int         gameID          = (int)             reader["gameid"];
                            DateTime    updateTime      = (DateTime)        reader["updatetime"];
                            GAMETYPE    gameType        = (GAMETYPE)(int)   reader["gametype"];
                            string      gameSymbol      = (string)          reader["gamesymbol"];
                            string      gameData        = (string)          reader["gamedata"];
                            DBMonitorSnapshot.Instance.setGameType(gameID, gameType);
                            DBMonitorSnapshot.Instance.setGameSymbol(gameSymbol, gameType, gameID, gameData);

                            if (DBMonitorSnapshot.Instance.GameConfigUpdateTime < updateTime)
                                DBMonitorSnapshot.Instance.GameConfigUpdateTime = updateTime;
                        }
                    }
                }
                if (DBMonitorSnapshot.Instance.LastAgentUpdateTime == new DateTime(1, 1, 1))
                {
                    DBMonitorSnapshot.Instance.LastAgentUpdateTime = new DateTime(1970, 1, 1);
                    string strQuery = "SELECT TOP 1 updatetime FROM agents order by updatetime desc";
                    SqlCommand command = new SqlCommand(strQuery, connection);
                    using (DbDataReader reader = await command.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync() && !(reader["updatetime"] is DBNull)) 
                            DBMonitorSnapshot.Instance.LastAgentUpdateTime = (DateTime)reader["updatetime"];
                    }
                }

                if (DBMonitorSnapshot.Instance.LastUserUpdateTime == new DateTime(1, 1, 1))
                {
                    DBMonitorSnapshot.Instance.LastUserUpdateTime = new DateTime(1970, 1, 1);
                    string strQuery = "SELECT TOP 1 updatetime FROM users order by updatetime desc";
                    SqlCommand command = new SqlCommand(strQuery, connection);
                    using (DbDataReader reader = await command.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync() && !(reader["updatetime"] is DBNull))
                            DBMonitorSnapshot.Instance.LastUserUpdateTime = (DateTime)reader["updatetime"];
                    }
                }
                if (AgentSnapshot.Instance.LastUpdateTime == new DateTime(1, 1, 1))
                {
                    AgentSnapshot.Instance.LastUpdateTime = new DateTime(1970, 1, 1);
                    string strQuery = "SELECT * FROM agents ORDER BY updatetime";
                    SqlCommand command = new SqlCommand(strQuery, connection);
                    using (DbDataReader reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            int         agentID         = (int)reader["id"];
                            int         apiMode         = (int)reader["apimode"];
                            if (apiMode == 0)
                            {
                                string authToken        = (string)reader["authtoken"];
                                AgentSnapshot.Instance.updateAgentConfig(agentID, new AgentAPIConfig(authToken));
                            }
                            else
                            {
                                string secretKey        = (string)reader["secretkey"];
                                string callbackURL      = (string)reader["callbackurl"];
                                string apiToken         = (string)reader["apitoken"];
                                AgentSnapshot.Instance.updateAgentConfig(agentID, new AgentAPIConfig(apiToken, secretKey, callbackURL));
                            }

                            DateTime    updateTime      = (DateTime)reader["updatetime"];
                            AgentSnapshot.Instance.LastUpdateTime = updateTime;
                        }
                    }
                }
                if (DBMonitorSnapshot.Instance.MaintenanceUpdateTime == new DateTime(1, 1, 1))
                {
                    DBMonitorSnapshot.Instance.MaintenanceUpdateTime = new DateTime(1970, 1, 1);
                    string strQuery = "SELECT TOP 1 maintenance, updatetime FROM maintenance ORDER BY updatetime DESC";
                    SqlCommand command = new SqlCommand(strQuery, connection);
                    using (DbDataReader reader = await command.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            int         maintenance = (int)reader["maintenance"];
                            DateTime    updateTime  = (DateTime)reader["updatetime"];
                            if (maintenance == 0)
                                DBMonitorSnapshot.Instance.IsNowMaintenance = false;
                            else
                                DBMonitorSnapshot.Instance.IsNowMaintenance = true;
                            DBMonitorSnapshot.Instance.MaintenanceUpdateTime = updateTime;
                        }
                    }
                }
            }
        }
        private async Task monitorTables()
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(_strConnString))
                {
                    await connection.OpenAsync();

                    string      strQuery = "SELECT * FROM quitusers WHERE id > @id ORDER BY id";
                    SqlCommand  command = new SqlCommand(strQuery, connection);
                    command.Parameters.AddWithValue("@id", DBMonitorSnapshot.Instance.LastQuitUserID);
                    using (DbDataReader reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            string strUserID = (string)reader["username"];
                            DBMonitorSnapshot.Instance.LastQuitUserID = (long)reader["id"];

                            Context.System.ActorSelection("/user/userManager").Tell(new QuitUserMessage(strUserID));
                        }
                    }

                    strQuery = "SELECT gameid, gametype, openclose, gamesymbol, gamedata, updatetime FROM gameconfigs WHERE updatetime > @updatetime ORDER BY updatetime";
                    command = new SqlCommand(strQuery, connection);
                    command.Parameters.AddWithValue("@updatetime", DBMonitorSnapshot.Instance.GameConfigUpdateTime);
                    using (DbDataReader reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            int      gameID     = (int)             reader["gameid"];
                            bool     openClose  = (bool)            reader["openclose"];
                            GAMETYPE gameType   = (GAMETYPE) (int)  reader["gametype"];
                            string   gameSymbol = (string)          reader["gamesymbol"];
                            string   gameData   = (string)          reader["gamedata"];

                            DBMonitorSnapshot.Instance.setGameType(gameID, gameType);
                            DBMonitorSnapshot.Instance.setGameSymbol(gameSymbol, gameType, gameID, gameData);
                            DBMonitorSnapshot.Instance.GameConfigUpdateTime = (DateTime)reader["updatetime"];
                        }
                    }

                    strQuery = "SELECT * FROM agents WHERE updatetime > @updatetime ORDER BY updatetime";
                    command = new SqlCommand(strQuery, connection);
                    command.Parameters.AddWithValue("@updatetime", AgentSnapshot.Instance.LastUpdateTime);

                    using (DbDataReader reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            int agentID = (int)reader["id"];
                            int apiMode = (int)reader["apimode"];
                            if (apiMode == 0)
                            {
                                string authToken = (string)reader["authtoken"];
                                AgentSnapshot.Instance.updateAgentConfig(agentID, new AgentAPIConfig(authToken));
                            }
                            else
                            {
                                string secretKey    = (string)reader["secretkey"];
                                string callbackURL  = (string)reader["callbackurl"];
                                string apiToken     = (string)reader["apitoken"];
                                AgentSnapshot.Instance.updateAgentConfig(agentID, new AgentAPIConfig(apiToken, secretKey, callbackURL));
                            }

                            DateTime updateTime = (DateTime)reader["updatetime"];
                            AgentSnapshot.Instance.LastUpdateTime = updateTime;
                        }
                    }

                    strQuery = "SELECT TOP 1 maintenance, updatetime FROM maintenance WHERE updatetime > @updatetime ORDER BY updatetime DESC";
                    command = new SqlCommand(strQuery, connection);
                    command.Parameters.AddWithValue("@updatetime", DBMonitorSnapshot.Instance.MaintenanceUpdateTime);
                    using (DbDataReader reader = await command.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            int         maintenance = (int)         reader["maintenance"];
                            DateTime    updateTime  = (DateTime)    reader["updatetime"];

                            bool isMaintenance      = false;
                            if (maintenance == 0)
                                isMaintenance = false;
                            else
                                isMaintenance = true;

                            if (!DBMonitorSnapshot.Instance.IsNowMaintenance && isMaintenance)
                            {
                                DBMonitorSnapshot.Instance.IsNowMaintenance = isMaintenance;
                                Context.System.ActorSelection("/user/userManager").Tell(new MaintenanceStartMessage());
                            }
                            else
                            {
                                DBMonitorSnapshot.Instance.IsNowMaintenance = isMaintenance;
                            }

                            DBMonitorSnapshot.Instance.MaintenanceUpdateTime = updateTime;
                        }
                    }
                }
            }
            catch(Exception ex)
            {
                _logger.Error("Exception has been occurred in DBProxyMonitor::monitorTables {0}", ex);
            }
            _monitorCancelable = Context.System.Scheduler.ScheduleTellOnceCancelable(5000, Self, "monitor", ActorRefs.NoSender);
        }
    }
}
