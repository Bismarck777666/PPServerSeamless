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
using SlotGamesNode.Jackpot;
using SlotGamesNode.HTTPService;

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

                if (DBMonitorSnapshot.Instance.LastGameJackpotID == -1)
                {
                    string strQuery = "SELECT TOP 1 id FROM userjackpots ORDER BY id DESC";
                    SqlCommand command = new SqlCommand(strQuery, connection);
                    using (DbDataReader reader = await command.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            DBMonitorSnapshot.Instance.LastGameJackpotID = reader.GetInt64(0);
                        }
                    }
                }
                if(DBMonitorSnapshot.Instance.LastGameJackpotThemeTime == new DateTime(1, 1, 1))
                {
                    DBMonitorSnapshot.Instance.LastGameJackpotThemeTime = new DateTime(1970, 1, 1);
                    string      strQuery    = "SELECT * FROM gamejackpotthemes ORDER BY updatetime";
                    SqlCommand  command     = new SqlCommand(strQuery, connection);
                    List<GameJackpotConfig> gameJackpotConfigs = new List<GameJackpotConfig>();
                    using (DbDataReader reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            GameJackpotConfig config                            = new GameJackpotConfig();
                            config.GameId                                       = (int) reader["gameid"];
                            config.TypeCount                                    = (int) reader["typecount"];
                            config.ThemeId                                      = (int) reader["themeid"];
                            DBMonitorSnapshot.Instance.LastGameJackpotThemeTime = (DateTime)reader["updatetime"];
                            gameJackpotConfigs.Add(config);
                        }
                    }
                    HTTPServiceConfig.Instance.JackpotActor.Tell(gameJackpotConfigs);
                }
                if (DBMonitorSnapshot.Instance.LastGameJackpotConfigTime == new DateTime(1, 1, 1))
                {
                    DBMonitorSnapshot.Instance.LastGameJackpotConfigTime = new DateTime(1970, 1, 1);
                    string strQuery = "SELECT * FROM gamejackpotconfigs ORDER BY updatetime";
                    SqlCommand command = new SqlCommand(strQuery, connection);
                    Dictionary<int, List<GameJackpotTypeConfig>> gameJackpotConfigs = new Dictionary<int, List<GameJackpotTypeConfig>>();
                    using (DbDataReader reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            int gameId = (int)reader["gameid"];
                            if (!gameJackpotConfigs.ContainsKey(gameId))
                                gameJackpotConfigs[gameId] = new List<GameJackpotTypeConfig>();

                            GameJackpotTypeConfig typeConfig = new GameJackpotTypeConfig();
                            typeConfig.TypeID       = (GameJackpotTypes)reader["typeid"];
                            typeConfig.Level        = (int)reader["level"];
                            typeConfig.Category     = (int)reader["category"];
                            typeConfig.Min          = (double)(decimal) reader["min"];
                            typeConfig.Max          = (double)(decimal) reader["max"];
                            typeConfig.RateUp       = (double)(decimal) reader["rateup"];
                            typeConfig.CurrentAmt   = (double)(decimal) reader["currentAmt"];
                            DBMonitorSnapshot.Instance.LastGameJackpotConfigTime = (DateTime)reader["updatetime"];
                            gameJackpotConfigs[gameId].Add(typeConfig);
                        }
                    }
                    HTTPServiceConfig.Instance.JackpotActor.Tell(gameJackpotConfigs);
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
                    string      strQuery    = "SELECT * FROM userjackpots WHERE id > @id and processed=0 ORDER BY id";
                    SqlCommand  command     = new SqlCommand(strQuery, connection);
                    command.Parameters.AddWithValue("@id", DBMonitorSnapshot.Instance.LastGameJackpotID);
                    using (DbDataReader reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            long    bonusID      = (long)reader["id"];
                            string  strUserID    = (string)reader["username"];
                            double  bonusMoney   = (double)(decimal)reader["jackpotmoney"];
                            int      bonusType   = (int)reader["jackpottype"];
                            DBMonitorSnapshot.Instance.LastGameJackpotID = (long)reader["id"];
                            Context.System.ActorSelection("/user/userManager").Tell(new GameJackpotBonusItem(bonusID, strUserID, bonusMoney, bonusType));
                        }
                    }
                    strQuery    = "SELECT * FROM gamejackpotthemes WHERE updatetime>@updatetime ORDER BY updatetime";
                    command     = new SqlCommand(strQuery, connection);
                    command.Parameters.AddWithValue("@updatetime", DBMonitorSnapshot.Instance.LastGameJackpotThemeTime);
                    using (DbDataReader reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            GameJackpotConfig config = new GameJackpotConfig();
                            config.GameId            = (int)reader["gameid"];
                            config.TypeCount         = (int)reader["typecount"];
                            config.ThemeId           = (int)reader["themeid"];
                            DBMonitorSnapshot.Instance.LastGameJackpotThemeTime = (DateTime)reader["updatetime"];
                            HTTPServiceConfig.Instance.JackpotActor.Tell(config);
                        }
                    }
                    strQuery = "SELECT * FROM gamejackpotconfigs WHERE updatetime > @updatetime ORDER BY updatetime";
                    command  = new SqlCommand(strQuery, connection);
                    command.Parameters.AddWithValue("@updatetime", DBMonitorSnapshot.Instance.LastGameJackpotConfigTime);
                    using (DbDataReader reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            GameJackpotTypeChanged changedEvent = new GameJackpotTypeChanged();
                            changedEvent.GameId = (int)reader["gameid"];
                            changedEvent.JackpotTypeConfig                          = new GameJackpotTypeConfig();
                            changedEvent.JackpotTypeConfig.TypeID                   = (GameJackpotTypes)reader["typeid"];
                            changedEvent.JackpotTypeConfig.Level                    = (int) reader["level"];
                            changedEvent.JackpotTypeConfig.Category                 = (int) reader["category"];
                            changedEvent.JackpotTypeConfig.Min                      = (double) (decimal) reader["min"];
                            changedEvent.JackpotTypeConfig.Max                      = (double) (decimal) reader["max"];
                            changedEvent.JackpotTypeConfig.RateUp                   = (double) (decimal) reader["rateup"];
                            changedEvent.JackpotTypeConfig.ResetAmt                 = (int) reader["resetAmt"] == 1;
                            changedEvent.JackpotTypeConfig.CurrentAmt               = (double) (decimal) reader["currentAmt"];
                            DBMonitorSnapshot.Instance.LastGameJackpotConfigTime    = (DateTime)reader["updatetime"];
                            HTTPServiceConfig.Instance.JackpotActor.Tell(changedEvent); 
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
