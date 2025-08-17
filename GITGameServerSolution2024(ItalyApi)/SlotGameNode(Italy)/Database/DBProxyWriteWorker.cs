using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Akka.Actor;
using Akka.Event;
using System.Data.SqlClient;
using System.Data;
using SlotGamesNode.GameLogics;

namespace SlotGamesNode.Database
{
    public class DBProxyWriteWorker : ReceiveActor
    {
        private string                      _strConnectionString    = "";
        private ICancelable                 _schedulerCancel        = null;
        private readonly ILoggingAdapter    _logger                 = Logging.GetLogger(Context);

        public DBProxyWriteWorker(string strConnectionString)
        {
            _strConnectionString = strConnectionString;
            ReceiveAsync<string>(processCommand);
        }

        protected override void PreStart()
        {
            if (_schedulerCancel != null)
                _schedulerCancel.Cancel();

            _schedulerCancel = Context.System.Scheduler.ScheduleTellOnceCancelable(1000, Self, "write", ActorRefs.NoSender);
        }

        public static Props Props(string strConnString)
        {
            return Akka.Actor.Props.Create(() => new DBProxyWriteWorker(strConnString));
        }

        private async Task processCommand(string strCommand)
        {
            if (strCommand == "write")
            {
                try
                {
                    using (SqlConnection connection = new SqlConnection(_strConnectionString))
                    {
                        await connection.OpenAsync();

                        //프라그마틱게임들에서 유저의 플레이이력을 디비에 보관한다.
                        await insertPPGameHistory(connection);
                        await insertPPRecentGameHistory(connection);

                        //부운고게임이력
                        await insertBNGGameHistory(connection);

                        //CQ9게임이력
                        await insertCQ9GameHistory(connection);

                        //하바네로게임이력
                        await insertHananeroHistory(connection);

                        //플레이선게임이력
                        await insertPlaysonGameHistory(connection);
                    }
                }
                catch (Exception ex)
                {
                    _logger.Error("Exception has been occured in DBProxyWriteWorker::processCommand {0}", ex.ToString());
                }
                _schedulerCancel = Context.System.Scheduler.ScheduleTellOnceCancelable(1000, Self, "write", ActorRefs.NoSender);
            }
            else if (strCommand == "flush")
            {
                if (_schedulerCancel != null)
                    _schedulerCancel.Cancel();

                _logger.Info("Flush Started");

                do
                {
                    using (SqlConnection connection = new SqlConnection(_strConnectionString))
                    {
                        await connection.OpenAsync();
                        int insertCount = await insertPPGameHistory(connection);
                        insertCount     += await insertPPRecentGameHistory(connection);
                        insertCount     += await insertBNGGameHistory(connection);
                        insertCount     += await insertCQ9GameHistory(connection);
                        insertCount     += await insertHananeroHistory(connection);
                        insertCount     += await insertPlaysonGameHistory(connection);

                        _logger.Info("report updated at {0}, count: {1}", DateTime.Now, insertCount);
                        if (insertCount == 0)
                            break;
                    }
                } while (true);
                _logger.Info("Flush Ended");
            }
        }

        private async Task<int> insertPPRecentGameHistory(SqlConnection connection)
        {
            List<PPGameRecentHistoryDBItem> historyItems = null;
            try
            {
                historyItems = await Context.Parent.Ask<List<PPGameRecentHistoryDBItem>>("PopPPRecentGameHistoryItems", TimeSpan.FromSeconds(5));
                if (historyItems == null)
                    return 0;

                DataTable dataTable = new DataTable();
                dataTable.Columns.Add("agentid",    typeof(int));
                dataTable.Columns.Add("username",   typeof(string));
                dataTable.Columns.Add("gameid",     typeof(int));
                dataTable.Columns.Add("balance",    typeof(decimal));
                dataTable.Columns.Add("bet",        typeof(decimal));
                dataTable.Columns.Add("win",        typeof(decimal));
                dataTable.Columns.Add("roundid",    typeof(string));
                dataTable.Columns.Add("detaillog",  typeof(string));
                dataTable.Columns.Add("timestamp",  typeof(long));

                foreach (PPGameRecentHistoryDBItem item in historyItems)
                    dataTable.Rows.Add(item.AgentID, item.UserName, item.GameID, item.Balance, item.Bet, item.Win, item.RoundID, item.DetailLog, item.DateTime);


                SqlCommand command  = new SqlCommand("UpsertPPUserRecentGameLog", connection);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.Add(new SqlParameter("@tblGameLogs", dataTable));
                await command.ExecuteNonQueryAsync();
                return dataTable.Rows.Count;
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occured in DBProxyWriteWorker::insertPPRecentGameHistory {0}", ex.ToString());

                //기록에 실패한 항목들을 다시 넣는다.
                if (historyItems != null && historyItems.Count > 0)
                    Context.Parent.Tell(historyItems);

                return -1;
            }
        }
        
        private async Task<int> insertPPGameHistory(SqlConnection connection)
        {
            List<PPGameHistoryDBItem> historyItems = null;
            try
            {
                historyItems = await Context.Parent.Ask<List<PPGameHistoryDBItem>>("PopPPGameHistoryItems", TimeSpan.FromSeconds(5));
                if (historyItems == null)
                    return 0;

                DataTable dataTable = new DataTable();
                dataTable.Columns.Add("agentid",    typeof(int));
                dataTable.Columns.Add("username",   typeof(string));
                dataTable.Columns.Add("gameid",     typeof(int));
                dataTable.Columns.Add("bet",        typeof(decimal));
                dataTable.Columns.Add("basebet",    typeof(decimal));
                dataTable.Columns.Add("win",        typeof(decimal));
                dataTable.Columns.Add("rtp",        typeof(decimal));
                dataTable.Columns.Add("roundid",    typeof(string));
                dataTable.Columns.Add("detaillog",  typeof(string));
                dataTable.Columns.Add("playeddate", typeof(long));

                foreach (PPGameHistoryDBItem item in historyItems)
                    dataTable.Rows.Add(item.AgentID, item.UserName, item.GameID, item.Bet, item.BaseBet, item.Win, item.RTP,item.RoundID, item.DetailLog, item.PlayedDate);


                SqlCommand command  = new SqlCommand("UpsertPPUserTopGameLog", connection);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.Add(new SqlParameter("@tblGameLogs", dataTable));
                await command.ExecuteNonQueryAsync();
                return dataTable.Rows.Count;
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occured in DBProxyWriteWorker::insertPPGameHistory {0}", ex.ToString());

                //기록에 실패한 항목들을 다시 넣는다.
                if (historyItems != null && historyItems.Count > 0)
                    Context.Parent.Tell(historyItems);

                return -1;
            }
        }
        
        private async Task<int> insertBNGGameHistory(SqlConnection connection)
        {
            List<BNGHistoryItem> historyItems = null;
            try
            {
                historyItems = await Context.Parent.Ask<List<BNGHistoryItem>>("PopBNGGameHistoryItems", TimeSpan.FromSeconds(5));
                if (historyItems == null)
                    return 0;

                DataTable dataTable = new DataTable();
                dataTable.Columns.Add("agentid",        typeof(int));
                dataTable.Columns.Add("userid",         typeof(string));
                dataTable.Columns.Add("gameid",         typeof(int));
                dataTable.Columns.Add("bet",            typeof(decimal));
                dataTable.Columns.Add("win",            typeof(decimal));
                dataTable.Columns.Add("roundid",        typeof(string));
                dataTable.Columns.Add("transactionid",  typeof(string));    
                dataTable.Columns.Add("overview",       typeof(string));
                dataTable.Columns.Add("detail",         typeof(string));
                dataTable.Columns.Add("time",           typeof(DateTime));

                foreach (BNGHistoryItem item in historyItems)
                    dataTable.Rows.Add(item.AgentID, item.UserID, item.GameID, (decimal)item.Bet, (decimal)item.Win, item.RoundID, item.TransactionID, item.Overview, item.Detail, item.Time);

                SqlCommand command  = new SqlCommand("UpsertBNGGameHistory", connection);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.Add(new SqlParameter("@tblGameHistory", dataTable));
                await command.ExecuteNonQueryAsync();
                return dataTable.Rows.Count;
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occured in DBProxyWriteWorker::insertBNGGameHistory {0}", ex.ToString());

                //기록에 실패한 항목들을 다시 넣는다.
                if (historyItems != null && historyItems.Count > 0)
                    Context.Parent.Tell(historyItems);

                return -1;
            }
        }
        
        private async Task<int> insertCQ9GameHistory(SqlConnection connection)
        {
            List<CQ9GameLogItem> historyItems = null;
            try
            {
                historyItems = await Context.Parent.Ask<List<CQ9GameLogItem>>("PopCQ9GameHistoryItems", TimeSpan.FromSeconds(5));
                if (historyItems == null)
                    return 0;

                DataTable dataTable = new DataTable();
                dataTable.Columns.Add("agentid",    typeof(int));
                dataTable.Columns.Add("userid",     typeof(string));
                dataTable.Columns.Add("gameid",     typeof(int));
                dataTable.Columns.Add("roundid",    typeof(string));
                dataTable.Columns.Add("bet",        typeof(decimal));
                dataTable.Columns.Add("win",        typeof(decimal));
                dataTable.Columns.Add("overview",   typeof(string));
                dataTable.Columns.Add("detail",     typeof(string));
                dataTable.Columns.Add("time",       typeof(DateTime));

                foreach (CQ9GameLogItem item in historyItems)
                    dataTable.Rows.Add(item.AgentID, item.UserID, item.GameID, item.RoundID, (decimal)item.Bet, (decimal)item.Win,  item.Overview, item.Detail, item.Time);

                SqlCommand command  = new SqlCommand("UpsertCQ9GameHistory", connection);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.Add(new SqlParameter("@tblGameHistory", dataTable));
                command.Parameters.Add(new SqlParameter("@lasttime",       DateTime.UtcNow.Subtract(TimeSpan.FromHours(4.0)).Date.Subtract(TimeSpan.FromDays(8.0))));
                await command.ExecuteNonQueryAsync();
                return dataTable.Rows.Count;
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occured in DBProxyWriteWorker::insertCQ9GameHistory {0}", ex.ToString());

                //기록에 실패한 항목들을 다시 넣는다.
                if (historyItems != null && historyItems.Count > 0)
                    Context.Parent.Tell(historyItems);

                return -1;
            }
        }
        
        private async Task<int> insertHananeroHistory(SqlConnection connection)
        {
            List<HabaneroLogItem> historyItems = null;
            try
            {
                historyItems = await Context.Parent.Ask<List<HabaneroLogItem>>("PopHabaneroGameHistoryItems", TimeSpan.FromSeconds(5));
                if (historyItems == null)
                    return 0;

                DataTable dataTable = new DataTable();
                dataTable.Columns.Add("agentid",    typeof(int));
                dataTable.Columns.Add("userid",     typeof(string));
                dataTable.Columns.Add("gameid",     typeof(int));
                dataTable.Columns.Add("bet",        typeof(decimal));
                dataTable.Columns.Add("win",        typeof(decimal));
                dataTable.Columns.Add("roundid",    typeof(string));
                dataTable.Columns.Add("gamelogid",  typeof(string));
                dataTable.Columns.Add("overview",   typeof(string));
                dataTable.Columns.Add("detail",     typeof(string));
                dataTable.Columns.Add("time",       typeof(DateTime));

                foreach (HabaneroLogItem item in historyItems)
                    dataTable.Rows.Add(item.AgentID, item.UserID, item.GameID, (decimal)item.Bet, (decimal)item.Win, item.RoundID, item.GameLogID, item.Overview, item.Detail, item.Time);

                SqlCommand command = new SqlCommand("UpsertHabaneroGameHistory", connection);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.Add(new SqlParameter("@tblGameHistory", dataTable));
                await command.ExecuteNonQueryAsync();
                return dataTable.Rows.Count;
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occured in DBProxyWriteWorker::insertHabaneroGameHistory {0}", ex.ToString());

                //기록에 실패한 항목들을 다시 넣는다.
                if (historyItems != null && historyItems.Count > 0)
                    Context.Parent.Tell(historyItems);

                return -1;
            }
        }

        private async Task<int> insertPlaysonGameHistory(SqlConnection connection)
        {
            List<PlaysonHistoryItem> historyItems = null;
            try
            {
                historyItems = await Context.Parent.Ask<List<PlaysonHistoryItem>>("PopPlaysonGameHistoryItems", TimeSpan.FromSeconds(5));
                if (historyItems == null)
                    return 0;

                DataTable dataTable = new DataTable();
                dataTable.Columns.Add("agentid",        typeof(string));
                dataTable.Columns.Add("userid",         typeof(string));
                dataTable.Columns.Add("gameid",         typeof(int));
                dataTable.Columns.Add("bet",            typeof(decimal));
                dataTable.Columns.Add("win",            typeof(decimal));
                dataTable.Columns.Add("roundid",        typeof(string));
                dataTable.Columns.Add("transactionid",  typeof(string));    
                dataTable.Columns.Add("overview",       typeof(string));
                dataTable.Columns.Add("detail",         typeof(string));
                dataTable.Columns.Add("time",           typeof(DateTime));

                foreach (PlaysonHistoryItem item in historyItems)
                    dataTable.Rows.Add(item.AgentID, item.UserID, item.GameID, (decimal)item.Bet, (decimal)item.Win, item.RoundID, item.TransactionID, item.Overview, item.Detail, item.Time);


                SqlCommand command = new SqlCommand("UpsertPlaysonGameHistory", connection);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.Add(new SqlParameter("@tblGameHistory", dataTable));
                await command.ExecuteNonQueryAsync();
                return dataTable.Rows.Count;
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occured in DBProxyWriteWorker::insertPlaysonGameHistory {0}", ex.ToString());

                //기록에 실패한 항목들을 다시 넣는다.
                if (historyItems != null && historyItems.Count > 0)
                    Context.Parent.Tell(historyItems);

                return -1;
            }
        }
    }
}
