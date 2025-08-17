using Akka.Actor;
using Akka.Event;
using SlotGamesNode.GameLogics;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace SlotGamesNode.Database
{
    public class DBProxyWriteWorker : ReceiveActor
    {
        private string                      _strConnectionString    = "";
        private ICancelable                 _schedulerCancel        = null;
        private readonly ILoggingAdapter    _logger                 = Context.GetLogger();

        public DBProxyWriteWorker(string strConnectionString)
        {
            _strConnectionString = strConnectionString;

            ReceiveAsync<string>    (processCommand);
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

                        await insertPPGameHistory(connection);
                        await insertPPRecentGameHistory(connection);
                        await updatePayoutPoolStatus(connection);
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
                        _logger.Info("report updated at {0}, count: {1}", DateTime.Now, insertCount);

                        if (insertCount == 0)
                            break;
                    }
                }
                while (true);
                _logger.Info("Flush Ended");
            }
        }

        private async Task<int> insertPPRecentGameHistory(SqlConnection connection)
        {
            List<PPGameRecentHistoryDBItem> historyItems = null;
            try
            {
                historyItems = await Context.Parent.Ask<List<PPGameRecentHistoryDBItem>>("PopPPRecentGameHistoryItems", TimeSpan.FromSeconds(5.0));
                if (historyItems == null)
                    return 0;

                DataTable dataTable = new DataTable();
                dataTable.Columns.Add("agentid",    typeof(int));
                dataTable.Columns.Add("username",   typeof(string));
                dataTable.Columns.Add("gameid",     typeof(int));
                dataTable.Columns.Add("balance",    typeof(Decimal));
                dataTable.Columns.Add("bet",        typeof(Decimal));
                dataTable.Columns.Add("win",        typeof(Decimal));
                dataTable.Columns.Add("detaillog",  typeof(string));
                dataTable.Columns.Add("timestamp",  typeof(long));
                dataTable.Columns.Add("currency",   typeof(string));

                foreach (PPGameRecentHistoryDBItem item in historyItems)
                    dataTable.Rows.Add(item.AgentID, item.UserName, item.GameID, item.Balance, item.Bet, item.Win, item.DetailLog, item.DateTime, item.Currency);
                
                SqlCommand command = new SqlCommand("UpsertPPUserRecentGameLog", connection);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.Add(new SqlParameter("@tblGameLogs", dataTable));
                await command.ExecuteNonQueryAsync();
                return dataTable.Rows.Count;
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occured in DBProxyWriteWorker::insertPPRecentGameHistory {0}", ex.ToString());
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
                historyItems = await Context.Parent.Ask<List<PPGameHistoryDBItem>>("PopPPGameHistoryItems", TimeSpan.FromSeconds(5.0));
                if (historyItems == null)
                    return 0;
                
                DataTable dataTable = new DataTable();
                dataTable.Columns.Add("agentid",    typeof(int));
                dataTable.Columns.Add("username",   typeof(string));
                dataTable.Columns.Add("gameid",     typeof(int));
                dataTable.Columns.Add("bet",        typeof(Decimal));
                dataTable.Columns.Add("basebet",    typeof(Decimal));
                dataTable.Columns.Add("win",        typeof(Decimal));
                dataTable.Columns.Add("rtp",        typeof(Decimal));
                dataTable.Columns.Add("detaillog",  typeof(string));
                dataTable.Columns.Add("playeddate", typeof(long));
                dataTable.Columns.Add("currency",   typeof(string));

                foreach (PPGameHistoryDBItem item in historyItems)
                    dataTable.Rows.Add(item.AgentID, item.UserName, item.GameID, item.Bet, item.BaseBet, item.Win, item.RTP, item.DetailLog, item.PlayedDate, item.Currency);
                
                SqlCommand command = new SqlCommand("UpsertPPUserTopGameLog", connection);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.Add(new SqlParameter("@tblGameLogs", dataTable));
                await command.ExecuteNonQueryAsync();
                return dataTable.Rows.Count;
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occured in DBProxyWriteWorker::insertPPGameHistory {0}", ex.ToString());
                if (historyItems != null && historyItems.Count > 0)
                    Context.Parent.Tell(historyItems);

                return -1;
            }
        }

        private async Task updatePayoutPoolStatus(SqlConnection connection)
        {
            UpdatePayoutPoolStatus updateItem = null;
            try
            {
                updateItem = await Context.Parent.Ask<UpdatePayoutPoolStatus>("PopUpdatePayoutPoolStatus", TimeSpan.FromSeconds(5.0));
                if (updateItem == null)
                {
                    updateItem = null;
                    return;
                }

                DataTable dataTable = new DataTable();
                dataTable.Columns.Add("agentid",    typeof(int));
                dataTable.Columns.Add("bet",        typeof(Decimal));
                dataTable.Columns.Add("win",        typeof(Decimal));
                foreach (KeyValuePair<int, double[]> pair in updateItem.PoolStatus)
                {
                    dataTable.Rows.Add(pair.Key, pair.Value[0], pair.Value[1]);
                }

                SqlCommand command = new SqlCommand("UpdatePayoutPool", connection);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.Add(new SqlParameter("@tblReports", dataTable));
                await command.ExecuteNonQueryAsync();
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occured in DBProxyWriteWorker::updatePayoutPoolStatus {0}", ex.ToString());
            }
        }
    }
}
