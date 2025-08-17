using Akka.Actor;
using Akka.Event;
using GITProtocol;
using System;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace UserNode.Database
{
    public class DBProxyMonitor : ReceiveActor
    {
        private string      _strConnString          = "";
        private ICancelable _monitorCancelable      = null;
        private ICancelable _monitorScoreCancelable = null;
        private readonly ILoggingAdapter _logger    = Context.GetLogger();

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

            if (_monitorScoreCancelable != null)
                _monitorScoreCancelable.Cancel();
            
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
                _monitorCancelable      = Context.System.Scheduler.ScheduleTellOnceCancelable(5000, Self, "monitor",        ActorRefs.NoSender);
                _monitorScoreCancelable = Context.System.Scheduler.ScheduleTellOnceCancelable(1000, Self, "monitorScores",  ActorRefs.NoSender);
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
                using (SqlConnection connection = new SqlConnection(_strConnString))
                {
                    await connection.OpenAsync();

                    if (DBMonitorSnapshot.Instance.LastUserScoreID == -1L)
                    {
                        string      strQuery    = "SELECT TOP 1 id FROM userscorelogs ORDER BY id DESC";
                        SqlCommand  command     = new SqlCommand(strQuery, connection);

                        using (DbDataReader reader = await command.ExecuteReaderAsync())
                        {
                            if (await reader.ReadAsync())
                                DBMonitorSnapshot.Instance.LastUserScoreID = reader.GetInt64(0);
                        }
                    }
                    
                    if (DBMonitorSnapshot.Instance.LastQuitUserID == -1L)
                    {
                        string      strQuery    = "SELECT TOP 1 id FROM quitusers ORDER BY id DESC";
                        SqlCommand  command     = new SqlCommand(strQuery, connection);

                        using (DbDataReader reader = await command.ExecuteReaderAsync())
                        {
                            if (await reader.ReadAsync())
                                DBMonitorSnapshot.Instance.LastQuitUserID = reader.GetInt64(0);
                        }
                    }
                    
                    if (DBMonitorSnapshot.Instance.LastAgentUpdateTime == new DateTime(1, 1, 1))
                    {
                        DBMonitorSnapshot.Instance.LastAgentUpdateTime = new DateTime(1970, 1, 1);
                        string      strQuery    = "SELECT TOP 1 updatetime FROM agents ORDER BY updatetime DESC";
                        SqlCommand  command     = new SqlCommand(strQuery, connection);

                        using (DbDataReader reader = await command.ExecuteReaderAsync())
                        {
                            if (await reader.ReadAsync())
                            {
                                if (!(reader["updatetime"] is DBNull))
                                    DBMonitorSnapshot.Instance.LastAgentUpdateTime = (DateTime)reader["updatetime"];
                            }
                        }
                    }
                    
                    if (DBMonitorSnapshot.Instance.LastUserUpdateTime == new DateTime(1, 1, 1))
                    {
                        DBMonitorSnapshot.Instance.LastUserUpdateTime = new DateTime(1970, 1, 1);
                        string      strQuery    = "SELECT TOP 1 updatetime FROM users ORDER BY updatetime DESC";
                        SqlCommand  command     = new SqlCommand(strQuery, connection);

                        using (DbDataReader reader = await command.ExecuteReaderAsync())
                        {
                            if (await reader.ReadAsync())
                            {
                                if (!(reader["updatetime"] is DBNull))
                                    DBMonitorSnapshot.Instance.LastUserUpdateTime = (DateTime)reader["updatetime"];
                            }
                        }
                    }
                    
                    if (DBMonitorSnapshot.Instance.GameConfigUpdateTime == new DateTime(1, 1, 1))
                    {
                        DBMonitorSnapshot.Instance.GameConfigUpdateTime = new DateTime(1970, 1, 1);
                        
                        string      strQuery    = "SELECT gameid, gametype, openclose, gamesymbol, updatetime FROM gameconfigs";
                        SqlCommand  command     = new SqlCommand(strQuery, connection);

                        using (DbDataReader reader = await command.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                int             gameID      = (int)reader["gameid"];
                                bool            isOpened    = (bool)reader["openclose"];
                                DateTime        updateTime  = (DateTime)reader["updatetime"];
                                GameProviders   gameType    = (GameProviders)reader["gametype"];
                                string          gameSymbol  = (string)reader["gamesymbol"];

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
                    string      strQuery    = "SELECT gameid, gametype, openclose, gamesymbol, updatetime FROM gameconfigs WHERE updatetime > @updatetime ORDER BY updatetime";
                    SqlCommand  command     = new SqlCommand(strQuery, connection);
                    command.Parameters.AddWithValue("@updatetime", DBMonitorSnapshot.Instance.GameConfigUpdateTime);

                    using(DbDataReader reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            int             gameID      = (int)reader["gameid"];
                            GameProviders   gameType    = (GameProviders)reader["gametype"];
                            string          gameSymbol  = (string)reader["gamesymbol"];

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
            _monitorCancelable = Context.System.Scheduler.ScheduleTellOnceCancelable(5000, Self, "monitor", ActorRefs.NoSender);
        }
    }
}
