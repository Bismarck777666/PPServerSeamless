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

                if (DBMonitorSnapshot.Instance.LastAgentUpdateTime == new DateTime(1, 1, 1))
                {
                    DBMonitorSnapshot.Instance.LastAgentUpdateTime = new DateTime(1970, 1, 1);
                    string strQuery     = "SELECT TOP 1 updatetime FROM agents order by updatetime desc";
                    SqlCommand command  = new SqlCommand(strQuery, connection);
                    using (DbDataReader reader = await command.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync() && !(reader["updatetime"] is DBNull))
                            DBMonitorSnapshot.Instance.LastAgentUpdateTime = (DateTime)reader["updatetime"];
                    }
                }

                if (DBMonitorSnapshot.Instance.LastUserUpdateTime == new DateTime(1, 1, 1))
                {
                    DBMonitorSnapshot.Instance.LastUserUpdateTime = new DateTime(1970, 1, 1);
                    string strQuery     = "SELECT TOP 1 updatetime FROM users order by updatetime desc";
                    SqlCommand command  = new SqlCommand(strQuery, connection);
                    using (DbDataReader reader = await command.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync() && !(reader["updatetime"] is DBNull))
                            DBMonitorSnapshot.Instance.LastUserUpdateTime = (DateTime)reader["updatetime"];
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
                            GAMETYPE    gameType        = (GAMETYPE)(int)   reader["gametype"];
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

                if (DBMonitorSnapshot.Instance.LastRangeEventPlayerID == -1)
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

                if (DBMonitorSnapshot.Instance.LastRangeEventUpdateTime == new DateTime(1, 1, 1))
                {
                    DBMonitorSnapshot.Instance.LastRangeEventUpdateTime = new DateTime(1970, 1, 1);
                    string strQuery = "SELECT TOP 1 updatetime FROM userrangeevents WHERE updatetime is not null ORDER BY updatetime DESC";
                    SqlCommand command = new SqlCommand(strQuery, connection);

                    using (DbDataReader reader = await command.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                            DBMonitorSnapshot.Instance.LastRangeEventUpdateTime = reader.GetDateTime(0);
                    }
                }

                if (DBMonitorSnapshot.Instance.LastBNGGameUpdateTime == new DateTime(1, 1, 1))
                {
                    DBMonitorSnapshot.Instance.LastBNGGameUpdateTime = new DateTime(1970, 1, 1);
                    string strQuery = "SELECT * FROM bnggames ORDER BY updatetime";
                    SqlCommand command = new SqlCommand(strQuery, connection);
                    using (DbDataReader reader = await command.ExecuteReaderAsync())
                    {
                        while(await reader.ReadAsync())
                        {
                            string strGameName = (string)reader["name"];
                            string strDrawVer = (string)reader["drawver"];
                            DBMonitorSnapshot.Instance.setBNGGameDrawVer(strGameName, strDrawVer);
                            DBMonitorSnapshot.Instance.LastBNGGameUpdateTime = (DateTime)reader["updatetime"];
                        }
                    }
                }
                
                if (PPAllGamesSnapshot.Instance.LastUpdateTime == new DateTime(1,1,1))
                {
                    PPAllGamesSnapshot.Instance.LastUpdateTime = new DateTime(1970, 1, 1);
                    string strQuery = "SELECT * FROM ppgames";
                    SqlCommand command = new SqlCommand(strQuery, connection);

                    PPAllGamesSnapshot.Instance.AllGames.Clear();
                    using (DbDataReader reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            string  gameSymbol      = (string)reader["symbol"];
                            string  gameName        = (string)reader["name"];
                            bool    hasDeveloped    = (int)reader["hasdeveloped"] == 1;
                            bool    isNew           = (int)reader["isnew"] == 1;
                            bool    isHot           = (int)reader["ishot"] == 1;
                            int     newOrder        = (int)reader["neworder"];
                            int     hotOrder        = (int)reader["hotorder"];
                            PPGame  ppGame          = new PPGame(gameSymbol, gameName, hasDeveloped, isNew, isHot, newOrder, hotOrder);
                            PPAllGamesSnapshot.Instance.AllGames.Add(ppGame);

                            if (!(reader["updatetime"] is DBNull))
                            {
                                DateTime updateTime = (DateTime)reader["updatetime"];
                                if (PPAllGamesSnapshot.Instance.LastUpdateTime < updateTime)
                                    PPAllGamesSnapshot.Instance.LastUpdateTime = updateTime;
                            }
                        }
                    }
                    PPAllGamesSnapshot.Instance.generateMiniLobbyGamesSnapshot();
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

                    //강퇴유저설정을 감시한다.
                    string      strQuery = "SELECT * FROM quitusers WHERE id > @id ORDER BY id";
                    SqlCommand  command = new SqlCommand(strQuery, connection);
                    command.Parameters.AddWithValue("@id", DBMonitorSnapshot.Instance.LastQuitUserID);
                    using (DbDataReader reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            int agentID         = (int)reader["agentid"];
                            string strUserID    = (string)reader["username"];
                            DBMonitorSnapshot.Instance.LastQuitUserID = (long)reader["id"];

                            Context.System.ActorSelection("/user/userManager").Tell(new QuitUserMessage(agentID, strUserID));
                        }
                    }

                    //게임설정을 감시한다.
                    strQuery = "SELECT gameid, gametype, openclose, gamesymbol, updatetime FROM gameconfigs WHERE updatetime > @updatetime ORDER BY updatetime";
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

                            if (openClose && DBMonitorSnapshot.Instance.ClosedGameIDs.Contains(gameID))
                                DBMonitorSnapshot.Instance.ClosedGameIDs.Remove(gameID);
                            else if (!openClose && !DBMonitorSnapshot.Instance.ClosedGameIDs.Contains(gameID))
                                DBMonitorSnapshot.Instance.ClosedGameIDs.Add(gameID);

                            DBMonitorSnapshot.Instance.setGameType(gameID, gameType);
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
                            long    bonusID     = (long)reader["id"];
                            int     agentID     = (int)reader["agentid"];
                            string  strUserID   = (string)reader["username"];
                            double  minOdd      = (double)(decimal)reader["minodd"];
                            double  maxOdd      = (double)(decimal)reader["maxodd"];
                            double  maxBet      = (double)(decimal)reader["maxbet"];

                            DBMonitorSnapshot.Instance.LastRangeEventPlayerID = (long)reader["id"];
                            Context.System.ActorSelection("/user/userManager").Tell(new UserRangeOddEventItem(bonusID, agentID, strUserID, minOdd, maxOdd, maxBet));
                        }
                    }

                    strQuery = "SELECT * FROM userrangeevents WHERE updatetime > @updatetime  ORDER BY updatetime";
                    command = new SqlCommand(strQuery, connection);
                    command.Parameters.AddWithValue("@updatetime", DBMonitorSnapshot.Instance.LastRangeEventUpdateTime);
                    using (DbDataReader reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            long    bonusID     = (long)reader["id"];
                            int     agentID     = (int)reader["agentid"];
                            string  strUserID   = (string)reader["username"];
                            int     processed   = (int)reader["processed"];
                            DBMonitorSnapshot.Instance.LastRangeEventUpdateTime = (DateTime)reader["updatetime"];

                            if (processed == 2)
                                Context.System.ActorSelection("/user/userManager").Tell(new UserEventCancelled(agentID, strUserID, bonusID));
                        }
                    }

                    strQuery = "SELECT * FROM ppgames WHERE updatetime > @updatetime ORDER BY updatetime";
                    command = new SqlCommand(strQuery, connection);
                    command.Parameters.AddWithValue("@updatetime", PPAllGamesSnapshot.Instance.LastUpdateTime);
                    using (DbDataReader reader = await command.ExecuteReaderAsync())
                    {
                        bool changed = false;
                        while (await reader.ReadAsync())
                        {
                            string gameSymbol   = (string)reader["symbol"];
                            string gameName     = (string)reader["name"];
                            bool hasDeveloped   = (int)reader["hasdeveloped"] == 1;
                            bool isNew          = (int)reader["isnew"] == 1;
                            bool isHot          = (int)reader["ishot"] == 1;
                            int newOrder        = (int)reader["neworder"];
                            int hotOrder        = (int)reader["hotorder"];
                            PPGame ppGame       = new PPGame(gameSymbol, gameName, hasDeveloped, isNew, isHot, newOrder, hotOrder);

                            int index = PPAllGamesSnapshot.Instance.AllGames.FindIndex(x => x.Symbol == gameSymbol);
                            if (index == -1)
                                PPAllGamesSnapshot.Instance.AllGames.Add(ppGame);
                            else
                                PPAllGamesSnapshot.Instance.AllGames[index] = ppGame;

                            changed = true;
                            PPAllGamesSnapshot.Instance.LastUpdateTime = (DateTime)reader["updatetime"];
                        }
                        if(changed)
                            PPAllGamesSnapshot.Instance.generateMiniLobbyGamesSnapshot();
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
