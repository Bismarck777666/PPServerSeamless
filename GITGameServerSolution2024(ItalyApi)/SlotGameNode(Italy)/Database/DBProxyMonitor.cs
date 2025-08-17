using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Akka.Actor;
using System.Data.SqlClient;
using System.Data.Common;
using GITProtocol;
using SlotGamesNode.GameLogics;
using Akka.Routing;
using Akka.Event;

namespace SlotGamesNode.Database
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
            Receive<PoisonPill>(message =>
            {

            });
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
            using (SqlConnection connection = new SqlConnection(_strConnString))
            {
                await connection.OpenAsync();
                             
                if(DBMonitorSnapshot.Instance.GameConfigUpdateTime == new DateTime(1, 1, 1))
                {
                    DBMonitorSnapshot.Instance.GameConfigUpdateTime = new DateTime(1970, 1, 1);
                    PayoutConfigSnapshot.Instance.PayoutConfigs.Clear();

                    string strQuery         = "SELECT gameid, gametype, openclose, gamesymbol,payoutrate, poolredundency, updatetime FROM gameconfigs order by updatetime";
                    SqlCommand command      = new SqlCommand(strQuery, connection);
                    using (DbDataReader reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            GAMEID  gameID           = (GAMEID)(int)reader["gameid"];
                            double  payoutrate       = (double) (decimal) reader["payoutrate"];
                            double  poolRedandency   = (double) (decimal)  reader["poolredundency"];
                            double  eventrate        = 0.0;
                            bool    hasRandomJackpot = false;
                            DateTime updateTime      = (DateTime)reader["updatetime"];
                            bool isOpened            = (bool)reader["openclose"];
                            GAMETYPE gameType        = (GAMETYPE)(int)reader["gametype"];
                            string gameSymbol        = (string)reader["gamesymbol"];

                            PayoutConfigSnapshot.Instance.PayoutConfigs[gameID] = new GameConfig(payoutrate, eventrate, poolRedandency);

                            if (!isOpened)
                                DBMonitorSnapshot.Instance.ClosedGameIDs.Add((int)gameID);

                            DBMonitorSnapshot.Instance.setGameType((int)gameID, gameType);
                            DBMonitorSnapshot.Instance.setGameSymbol(gameSymbol, gameType, (int)gameID);


                            if (DBMonitorSnapshot.Instance.GameConfigUpdateTime < updateTime)
                                DBMonitorSnapshot.Instance.GameConfigUpdateTime = updateTime;
                        }
                    }
                }
                
                if (DBMonitorSnapshot.Instance.AgentGameConfigUpdateTime == new DateTime(1, 1,1 ))
                {
                    DBMonitorSnapshot.Instance.AgentGameConfigUpdateTime = new DateTime(1970, 1, 1);
                    PayoutConfigSnapshot.Instance.AgentPayoutConfigs.Clear();
                    string strQuery = "SELECT gameid, agentid, payoutrate, updatetime FROM agentgameconfigs order by updatetime";
                    SqlCommand command = new SqlCommand(strQuery, connection);
                    using (DbDataReader reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            GAMEID  gameID              = (GAMEID) (int)    reader["gameid"];
                            int     companyID           = (int)             reader["agentid"];
                            double  payoutRate          = (double)(decimal) reader["payoutrate"];
                            DateTime updateTime         = (DateTime)        reader["updatetime"];

                            if (!PayoutConfigSnapshot.Instance.AgentPayoutConfigs.ContainsKey(gameID))
                                PayoutConfigSnapshot.Instance.AgentPayoutConfigs.Add(gameID, new Dictionary<int, double>());

                            PayoutConfigSnapshot.Instance.AgentPayoutConfigs[gameID][companyID] = payoutRate;

                            if (DBMonitorSnapshot.Instance.AgentGameConfigUpdateTime < updateTime)
                                DBMonitorSnapshot.Instance.AgentGameConfigUpdateTime = updateTime;
                        }
                    }

                    foreach(GAMEID gameID in PayoutConfigSnapshot.Instance.PayoutConfigs.Keys)
                    {
                        if (!PayoutConfigSnapshot.Instance.AgentPayoutConfigs.ContainsKey(gameID))
                            PayoutConfigSnapshot.Instance.AgentPayoutConfigs.Add(gameID, new Dictionary<int, double>());
                    }
                }

                if(DBMonitorSnapshot.Instance.CQ9GameUpdatetime == new DateTime(1, 1, 1))
                {
                    DBMonitorSnapshot.Instance.CQ9GameUpdatetime = new DateTime(1970, 1, 1);
                    CQ9Config.Instance.CQ9GameList = new List<CQ9GameDBItem>();

                    string strQuery     = "SELECT symbol,iconurl,backgroundurl,difficulty FROM cq9games WHERE hasdeveloped=1 AND ishot=1";
                    SqlCommand command  = new SqlCommand(strQuery, connection);

                    using (DbDataReader reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            string symbol           = (string)reader["symbol"];
                            string iconurl          = (string)reader["iconurl"];
                            string backgroundurl    = (string)reader["backgroundurl"];
                            int difficulty          = (int)reader["difficulty"];

                            CQ9GameDBItem addItem   = new CQ9GameDBItem(symbol,iconurl,backgroundurl,difficulty);
                            CQ9Config.Instance.CQ9GameList.Add(addItem);
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

                    //싱글게임설정을 감시한다.
                    string strQuery = "SELECT gameid, gametype, openclose, gamesymbol, payoutrate, poolredundency, updatetime FROM gameconfigs WHERE updatetime > @updatetime ORDER BY updatetime";
                    SqlCommand command = new SqlCommand(strQuery, connection);
                    command.Parameters.AddWithValue("@updatetime", DBMonitorSnapshot.Instance.GameConfigUpdateTime);
                    using (DbDataReader reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            GAMEID gameID           = (GAMEID)(int)reader["gameid"];
                            double payoutrate       = (double)(decimal)reader["payoutrate"];
                            double poolRedandency   = (double)(decimal)reader["poolredundency"];
                            double eventrate        = 0.0;
                            DateTime updateTime     = (DateTime)reader["updatetime"];
                            bool openClose          = (bool)reader["openclose"];
                            GAMETYPE gameType       = (GAMETYPE)(int)reader["gametype"];
                            string gameSymbol       = (string)reader["gamesymbol"];


                            DBMonitorSnapshot.Instance.GameConfigUpdateTime = updateTime;

                            if (openClose && DBMonitorSnapshot.Instance.ClosedGameIDs.Contains((int)gameID))
                                DBMonitorSnapshot.Instance.ClosedGameIDs.Remove((int)gameID);
                            else if (!openClose && !DBMonitorSnapshot.Instance.ClosedGameIDs.Contains((int)gameID))
                                DBMonitorSnapshot.Instance.ClosedGameIDs.Add((int)gameID);

                            DBMonitorSnapshot.Instance.setGameType((int)gameID, gameType);
                            DBMonitorSnapshot.Instance.setGameSymbol(gameSymbol, gameType, (int)gameID);

                            if (!PayoutConfigSnapshot.Instance.PayoutConfigs.ContainsKey(gameID))
                                continue;

                            GameConfig payoutConfig     = PayoutConfigSnapshot.Instance.PayoutConfigs[gameID];
                            payoutConfig.PayoutRate     = payoutrate;
                            payoutConfig.PoolRedundency = poolRedandency;
                            payoutConfig.EventRate      = eventrate;

                            Context.System.ActorSelection("/user/gameManager").Tell(new Broadcast(new PayoutConfigUpdated(gameID)));
                        }
                    }

                    //에이전트게임설정을 감시한다.
                    strQuery    = "SELECT gameid, agentid, payoutrate, updatetime FROM agentgameconfigs WHERE updatetime > @updatetime ORDER BY updatetime";
                     command    = new SqlCommand(strQuery, connection);
                    command.Parameters.AddWithValue("@updatetime", DBMonitorSnapshot.Instance.AgentGameConfigUpdateTime);
                    using (DbDataReader reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            GAMEID  gameID          = (GAMEID)(int)     reader["gameid"];
                            int     agentID         = (int)             reader["agentid"];
                            double  payoutRate      = (double)(decimal) reader["payoutrate"];
                            DateTime updateTime     = (DateTime)        reader["updatetime"];

                            DBMonitorSnapshot.Instance.AgentGameConfigUpdateTime = updateTime;
                            if (!PayoutConfigSnapshot.Instance.AgentPayoutConfigs.ContainsKey(gameID) || !PayoutConfigSnapshot.Instance.PayoutConfigs.ContainsKey(gameID))
                                continue;

                            double oldPayoutRate = PayoutConfigSnapshot.Instance.PayoutConfigs[gameID].PayoutRate;
                            if (PayoutConfigSnapshot.Instance.AgentPayoutConfigs[gameID].ContainsKey(agentID))
                                oldPayoutRate = PayoutConfigSnapshot.Instance.AgentPayoutConfigs[gameID][agentID];

                            PayoutConfigSnapshot.Instance.AgentPayoutConfigs[gameID][agentID] = payoutRate;
                            bool isChangedPayout = false;
                            if (oldPayoutRate != payoutRate)
                                isChangedPayout = true;
                            Context.System.ActorSelection("/user/gameManager").Tell(new Broadcast(new AgentPayoutConfigUpdated(gameID, agentID, isChangedPayout)));
                        }
                    }
                }
            }
            catch(Exception ex)
            {
                _logger.Error("Exception has been occurred in DBProxyMonitor::monitorTables {0}", ex);
            }
        }
    }
}
