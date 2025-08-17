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
        private ICancelable                 _monitorScoreCancelable = null;
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
                _monitorCancelable      = Context.System.Scheduler.ScheduleTellOnceCancelable(5000, Self, "monitor", ActorRefs.NoSender);
                _monitorScoreCancelable = Context.System.Scheduler.ScheduleTellOnceCancelable(1000, Self, "monitorScores", ActorRefs.NoSender);
                Sender.Tell("dbInitialized");
            }
            else if(strCommand == "monitor")
            {
                await monitorTables();
            }
            else if(strCommand == "monitorScores")
            {
                await monitorScores();
            }
        }
        
        private async Task initializeMonitor()
        {
            using (SqlConnection connection = new SqlConnection(_strConnString))
            {
                await connection.OpenAsync();

                if(DBMonitorSnapshot.Instance.LastScoreID == -1)
                {
                    string strQuery = "SELECT TOP 1 id FROM gamescorelogs ORDER BY id DESC";
                    SqlCommand command = new SqlCommand(strQuery, connection);
                    using (DbDataReader reader = await command.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            DBMonitorSnapshot.Instance.LastScoreID = reader.GetInt64(0);
                        }
                    }
                }

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

                if(DBMonitorSnapshot.Instance.LastServerStatesUpdateTime == new DateTime(1, 1, 1))
                {
                    DBMonitorSnapshot.Instance.LastServerStatesUpdateTime = new DateTime(1970, 1, 1);
                    string strQuery = "SELECT status, updatetime FROM serverstates";
                    SqlCommand command = new SqlCommand(strQuery, connection);
                    using (DbDataReader reader = await command.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            DBMonitorSnapshot.Instance.IsServerUp                   = ((int)reader["status"] == 1);
                            DBMonitorSnapshot.Instance.LastServerStatesUpdateTime   = (DateTime)reader["updatetime"];
                        }
                    }
                }
                
                if (DBMonitorSnapshot.Instance.GameConfigUpdateTime == new DateTime(1, 1, 1))
                {
                    DBMonitorSnapshot.Instance.GameConfigUpdateTime = new DateTime(1970, 1, 1);
                    DBMonitorSnapshot.Instance.ClosedGameIDs.Clear();

                    string strQuery         = "SELECT gameid, gametype, openclose, gamesymbol, updatetime FROM gameconfigs";
                    SqlCommand command      = new SqlCommand(strQuery, connection);
                    using (DbDataReader reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            int         gameID          = (int)             reader["gameid"];
                            bool        isOpened        = (bool)            reader["openclose"];
                            DateTime    updateTime      = (DateTime)        reader["updatetime"];
                            GameProviders    gameType        = (GameProviders)(int)   reader["gametype"];
                            string      gameSymbol      = (string)          reader["gamesymbol"];


                            if (!isOpened)
                                DBMonitorSnapshot.Instance.ClosedGameIDs.Add(gameID);

                            DBMonitorSnapshot.Instance.setGameType(gameID, gameType);
                            DBMonitorSnapshot.Instance.setGameSymbol(gameSymbol, gameType, gameID);

                            if (DBMonitorSnapshot.Instance.GameConfigUpdateTime < updateTime)
                                DBMonitorSnapshot.Instance.GameConfigUpdateTime = updateTime;
                        }
                    }
                }

                if(DBMonitorSnapshot.Instance.LastTurboSpeedUpdateTime == new DateTime(1, 1, 1))
                {
                    DBMonitorSnapshot.Instance.LastTurboSpeedUpdateTime = new DateTime(1970, 1, 1);
                    string strQuery = "SELECT TOP 1 * FROM turbospeeds ORDER BY updatetime DESC";
                    SqlCommand command = new SqlCommand(strQuery, connection);
                    using (DbDataReader reader = await command.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            DBMonitorSnapshot.Instance.TurboSpeed               = (double)(decimal)reader["speed"];
                            DBMonitorSnapshot.Instance.LastTurboSpeedUpdateTime = (DateTime) reader["updatetime"];
                        }
                    }
                }
                
                if(DBMonitorSnapshot.Instance.LastRangeEventPlayerID == -1)
                {
                    string strQuery = "SELECT TOP 1 id FROM userrangeevents ORDER BY id DESC";
                    SqlCommand command = new SqlCommand(strQuery, connection);
                    using (DbDataReader reader = await command.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            DBMonitorSnapshot.Instance.LastRangeEventPlayerID = reader.GetInt64(0);
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
            }
        }
        
        private async Task monitorScores()
        {
            List<SetScoreData> setScoreDatas = new List<SetScoreData>();
            try
            {
                using (SqlConnection connection = new SqlConnection(_strConnString))
                {
                    await connection.OpenAsync();

                    string strQuery = "SELECT * FROM gamescorelogs WHERE id > @id ORDER BY  id";
                    SqlCommand command = new SqlCommand(strQuery, connection);
                    command.Parameters.AddWithValue("@id", DBMonitorSnapshot.Instance.LastScoreID);

                    using (DbDataReader reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            long scoreID = (long)reader["id"];
                            string strUserName = (string)reader["username"];
                            double addedScore = (double)(decimal)reader["addedscore"];

                            setScoreDatas.Add(new SetScoreData(scoreID, strUserName, addedScore));
                            DBMonitorSnapshot.Instance.LastScoreID = scoreID;
                        }
                    }
                }
            }
            catch(Exception ex)
            {
                _logger.Error("Exception has been occurred in DBProxyMonitor::monitorScores {0}", ex);
            }           
            
            //Changed by Foresight(2019.10.09)
            try
            {
                Context.System.ActorSelection("/user/userManager").Tell(setScoreDatas);
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occurred in DBProxyMonitor::monitorScores {0}", ex);
            }
            _monitorScoreCancelable = Context.System.Scheduler.ScheduleTellOnceCancelable(1000, Self, "monitorScores", ActorRefs.NoSender);
        }
        
        private async Task monitorTables()
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(_strConnString))
                {
                    await connection.OpenAsync();

                    //강퇴유저설정을 감시한다.
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

                    //터보스핀속도설정을 감시한다.
                    strQuery    = "SELECT * FROM turbospeeds WHERE updatetime > @updatetime";
                    command     = new SqlCommand(strQuery, connection);
                    command.Parameters.AddWithValue("@updatetime", DBMonitorSnapshot.Instance.LastTurboSpeedUpdateTime);
                    using (DbDataReader reader = await command.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            DBMonitorSnapshot.Instance.TurboSpeed               = (double) (decimal) reader["speed"];
                            DBMonitorSnapshot.Instance.LastTurboSpeedUpdateTime = (DateTime) reader["updatetime"];
                        }
                    }

                    //서버점검상태
                    strQuery = "SELECT status, updatetime FROM serverstates WHERE updatetime > @updatetime";
                    command = new SqlCommand(strQuery, connection);
                    command.Parameters.AddWithValue("@updatetime", DBMonitorSnapshot.Instance.LastServerStatesUpdateTime);
                    using (DbDataReader reader = await command.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            DBMonitorSnapshot.Instance.IsServerUp                 = ((int)reader["status"] == 1);
                            DBMonitorSnapshot.Instance.LastServerStatesUpdateTime = (DateTime)reader["updatetime"];

                            if(!DBMonitorSnapshot.Instance.IsServerUp)
                                Context.System.ActorSelection("/user/userManager").Tell(new ServerMaintenanceNotify());

                        }
                    }
                    
                    //싱글게임설정을 감시한다.
                    strQuery = "SELECT gameid, gametype, openclose, gamesymbol, updatetime FROM gameconfigs WHERE updatetime > @updatetime ORDER BY updatetime";
                    command = new SqlCommand(strQuery, connection);
                    command.Parameters.AddWithValue("@updatetime", DBMonitorSnapshot.Instance.GameConfigUpdateTime);
                    using (DbDataReader reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            int      gameID     = (int)             reader["gameid"];
                            bool     openClose  = (bool)            reader["openclose"];
                            GameProviders gameType   = (GameProviders) (int)  reader["gametype"];
                            string   gameSymbol = (string)          reader["gamesymbol"];

                            if (openClose && DBMonitorSnapshot.Instance.ClosedGameIDs.Contains(gameID))
                                DBMonitorSnapshot.Instance.ClosedGameIDs.Remove(gameID);
                            else if (!openClose && !DBMonitorSnapshot.Instance.ClosedGameIDs.Contains(gameID))
                                DBMonitorSnapshot.Instance.ClosedGameIDs.Add(gameID);

                            DBMonitorSnapshot.Instance.setGameType  (gameID, gameType);
                            DBMonitorSnapshot.Instance.setGameSymbol(gameSymbol, gameType, gameID);

                            DBMonitorSnapshot.Instance.GameConfigUpdateTime = (DateTime)reader["updatetime"];
                        }
                    }

                    //userrangeevents표를 감시한다.
                    strQuery = "SELECT * FROM userrangeevents WHERE id > @id and processed=0 ORDER BY id";
                    command = new SqlCommand(strQuery, connection);
                    command.Parameters.AddWithValue("@id", DBMonitorSnapshot.Instance.LastRangeEventPlayerID);
                    using (DbDataReader reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            long    bonusID      = (long)reader["id"];
                            string  strUserID    = (string)reader["username"];
                            double  minOdd       = (double)(decimal)reader["minodd"];
                            double  maxOdd       = (double)(decimal)reader["maxodd"];
                            double  maxBet       = (double)(decimal)reader["maxbet"];

                            DBMonitorSnapshot.Instance.LastRangeEventPlayerID = (long)reader["id"];
                            Context.System.ActorSelection("/user/userManager").Tell(new UserRangeOddEventItem(bonusID, strUserID, minOdd, maxOdd,maxBet));
                        }
                    }
 
                    await monitorUserAgentTable(connection);
                }
            }
            catch(Exception ex)
            {
                _logger.Error("Exception has been occurred in DBProxyMonitor::monitorTables {0}", ex);
            }
            _monitorCancelable = Context.System.Scheduler.ScheduleTellOnceCancelable(5000, Self, "monitor", ActorRefs.NoSender);
        }

        private async Task monitorUserAgentTable(SqlConnection connection)
        {
            try
            {
                string strQuery = "SELECT username, rollfee, updatetime FROM users WHERE updatetime > @updatetime";
                SqlCommand command = new SqlCommand(strQuery, connection);
                command.Parameters.AddWithValue("@updatetime", DBMonitorSnapshot.Instance.LastUserUpdateTime);
                using (DbDataReader reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        string strUserID  = (string)            reader["username"];
                        double rollingPer = (double) (decimal)  reader["rollfee"];

                        DBMonitorSnapshot.Instance.LastUserUpdateTime = (DateTime)reader["updatetime"];

                        Context.System.ActorSelection("/user/userManager").Tell(new UserRollingPerUpdated(strUserID, rollingPer));
                    }
                }

                strQuery = "SELECT id, rollfee, updatetime FROM agents WHERE updatetime > @updatetime";
                command  = new SqlCommand(strQuery, connection);
                command.Parameters.AddWithValue("@updatetime", DBMonitorSnapshot.Instance.LastAgentUpdateTime);
                using (DbDataReader reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        int     agentID     = (int)reader["id"];
                        double  rollingPer  = (double)(decimal)reader["rollfee"];

                        DBMonitorSnapshot.Instance.LastAgentUpdateTime = (DateTime)reader["updatetime"];

                        Context.System.ActorSelection("/user/userManager").Tell(new AgentRollingPerUpdated(agentID, rollingPer));
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occurred in DBProxyMonitor::monitorLoseAgentUsers {0}", ex);
            }
        }
    }
}
