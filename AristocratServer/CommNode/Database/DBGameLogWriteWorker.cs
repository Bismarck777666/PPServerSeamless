using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Akka.Actor;
using Akka.Event;
using System.Data.SqlClient;
using System.Data;
using GITProtocol;
using Microsoft.Extensions.Logging;
using System.Net.PeerToPeer;

namespace CommNode.Database
{
    public class DBGameLogWriteWorker : ReceiveActor
    {
        private string                          _strConnectionString    = "";
        private             ICancelable         _schedulerCancel        = null;
        private readonly    ILoggingAdapter     _logger                 = Logging.GetLogger(Context);

        public DBGameLogWriteWorker(string strConnectionString)
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
            return Akka.Actor.Props.Create(() => new DBGameLogWriteWorker(strConnString));
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

                        await insertGameLogs(connection);

                        await insertApiTransactions(connection);
                        await insertFailedTransactions(connection);
                        await updateDepositTransactions(connection);

                        await insertUserTournamentBetItems(connection);
                    }
                }
                catch (Exception ex)
                {
                    _logger.Error("Exception has been occured in DBProxyWriter {0}", ex.ToString());
                }
                _schedulerCancel = Context.System.Scheduler.ScheduleTellOnceCancelable(1000, Self, "write", ActorRefs.NoSender);
            }
            else if(strCommand == "flush")
            {
                if (_schedulerCancel != null)
                    _schedulerCancel.Cancel();

                _logger.Info("Flush");
                do
                {
                    try
                    {
                        using (SqlConnection connection = new SqlConnection(_strConnectionString))
                        {
                            await connection.OpenAsync();

                            int transactionCount            = await insertApiTransactions(connection);
                            int insertedLogCount            = await insertGameLogs(connection);
                            int faildTransationCount        = await insertFailedTransactions(connection);
                            int updateTransCount            = await updateDepositTransactions(connection);
                            int insertedTournamentBetCount  = await insertUserTournamentBetItems(connection);
                            if (insertedLogCount == 0 && transactionCount == 0 && faildTransationCount == 0 && updateTransCount == 0 && insertedTournamentBetCount == 0)
                                break;
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.Error("Exception has been occured in DBProxyWriter {0}", ex.ToString());
                    }
                } while (true);

                _logger.Info("Flush Ended");
            }
        }
        private async Task<int> insertGameLogs(SqlConnection connection)
        {
            List<GameLogItem> insertItems = await Context.Parent.Ask<List<GameLogItem>>("PopGameLogItems", TimeSpan.FromSeconds(5));
            if (insertItems == null)
                return 0;

            Dictionary<int, List<GameLogItem>> dicGameLogItems = new Dictionary<int, List<GameLogItem>>();
            foreach (GameLogItem item in insertItems)
            {
                if (!dicGameLogItems.ContainsKey(item.AgentID))
                    dicGameLogItems.Add(item.AgentID, new List<GameLogItem>());

                dicGameLogItems[item.AgentID].Add(item);
            }

            int totalCount = 0;
            foreach (KeyValuePair<int, List<GameLogItem>> pair in dicGameLogItems)
            {
                if (!WriterSnapshot.Instance.IsGameLogTableCreated(pair.Key))
                {
                    try
                    {
                        SqlCommand command = new SqlCommand("usp_CreateAgentLogTables", connection);
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.Add(new SqlParameter("@agentid", pair.Key));
                        await command.ExecuteNonQueryAsync();

                        WriterSnapshot.Instance.HasCreatedGameLogTable(pair.Key);
                    }
                    catch (Exception ex)
                    {
                        _logger.Error("Create GameLog Table {0} failed : {1}", pair.Key, ex.ToString());
                    }
                }

                try
                {

                    DataTable dataTable = new DataTable();
                    dataTable.Columns.Add("username",   typeof(string));
                    dataTable.Columns.Add("gameid",     typeof(int));
                    dataTable.Columns.Add("gamename",   typeof(string));
                    dataTable.Columns.Add("tableid",    typeof(string));
                    dataTable.Columns.Add("bet",        typeof(decimal));
                    dataTable.Columns.Add("win",        typeof(decimal));
                    dataTable.Columns.Add("beginmoney", typeof(decimal));
                    dataTable.Columns.Add("endmoney",   typeof(decimal));
                    dataTable.Columns.Add("gamelog",    typeof(string));
                    dataTable.Columns.Add("logtime",    typeof(DateTime));

                    foreach (GameLogItem item in pair.Value)
                        dataTable.Rows.Add(item.UserID, item.GameID, item.GameName, item.TableID ?? "", (decimal)Math.Round(item.Bet, 2), (decimal)Math.Round(item.Win, 2), (decimal)Math.Round(item.BeginMoney, 2), (decimal)Math.Round(item.EndMoney, 2), item.GameLog, item.LogTime);


                    using (SqlBulkCopy bulkCopy = new SqlBulkCopy(connection))
                    {
                        bulkCopy.DestinationTableName = string.Format("gamelog_{0}", pair.Key);
                        for (int i = 0; i < dataTable.Columns.Count; i++)
                            bulkCopy.ColumnMappings.Add(i, i + 1);

                        await bulkCopy.WriteToServerAsync(dataTable);
                    }

                    totalCount += dataTable.Rows.Count;
                }
                catch (Exception ex)
                {
                    _logger.Error("Exception has been occured in DBWriteWorker while inserting game logs : {0}", ex.ToString());

                    Context.Parent.Tell(pair.Value);
                    break;
                }
            }

            return totalCount;
        }

        private async Task<int> insertUserTournamentBetItems(SqlConnection connection)
        {
            List<PPTourLeaderboardDBItem> insertItems = await Context.Parent.Ask<List<PPTourLeaderboardDBItem>>("PopUserTournamentBetItems", TimeSpan.FromSeconds(5));
            if (insertItems == null)
                return 0;

            int totalCount = insertItems.Count;
            try
            {

                DataTable dataTable = new DataTable();

	            dataTable.Columns.Add("agentid",        typeof(string));
                dataTable.Columns.Add("isagent",        typeof(int));
                dataTable.Columns.Add("tournamentid",   typeof(int));
                dataTable.Columns.Add("type",           typeof(int));
                dataTable.Columns.Add("username",       typeof(string));
                dataTable.Columns.Add("country",        typeof(string));
                dataTable.Columns.Add("currency",       typeof(string));
                dataTable.Columns.Add("addscore",       typeof(double));
                dataTable.Columns.Add("bet",            typeof(decimal));
                dataTable.Columns.Add("addbet",         typeof(double));
                dataTable.Columns.Add("addwin",         typeof(double));
                dataTable.Columns.Add("updatetime",     typeof(DateTime));

                foreach (PPTourLeaderboardDBItem item in insertItems)
                    dataTable.Rows.Add(item.AgentID, item.IsAgent, item.TournamentID, item.TournamentType, item.UserName, item.Country, item.Currency, 
                        item.AddScore, (decimal) item.Bet, (decimal)item.AddBet, (decimal)item.AddWin, DateTime.UtcNow);


                SqlCommand command  = new SqlCommand("UpdateTournamentBets", connection);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.Add(new SqlParameter("@tblBets", dataTable));
                await command.ExecuteNonQueryAsync();
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occured in DBWriteWorker while inserting tournament user bet logs : {0}", ex.ToString());

                Context.Parent.Tell(insertItems);
            }
            return totalCount;

        }
        private async Task<int> insertApiTransactions(SqlConnection connection)
        {
            List<ApiTransactionItem> transactionItems = null;
            try
            {
                transactionItems = await Context.Parent.Ask<List<ApiTransactionItem>>("PopApiTransactionItems", TimeSpan.FromSeconds(5));
                if (transactionItems == null || transactionItems.Count == 0)
                    return 0;

                DataTable insertDataTable = new DataTable();
                insertDataTable.Columns.Add("agentid", typeof(string));
                insertDataTable.Columns.Add("userid", typeof(string));
                insertDataTable.Columns.Add("transactionid", typeof(string));
                insertDataTable.Columns.Add("reltransactionid", typeof(string));
                insertDataTable.Columns.Add("platformtransactionid", typeof(string));
                insertDataTable.Columns.Add("transactiontype", typeof(int));
                insertDataTable.Columns.Add("gameid", typeof(int));
                insertDataTable.Columns.Add("amount", typeof(decimal));
                insertDataTable.Columns.Add("timestamp", typeof(DateTime));


                foreach (ApiTransactionItem item in transactionItems)
                {
                    insertDataTable.Rows.Add(item.AgentID, item.UserID, item.TransactionID, item.RelTransactionID, item.PlatformTransID,
                        (int)item.TransactionType, item.GameID, (decimal)item.Amount, item.Timestamp);
                }

                if (insertDataTable.Rows.Count > 0)
                {
                    using (SqlBulkCopy bulkCopy = new SqlBulkCopy(connection))
                    {
                        bulkCopy.DestinationTableName = "transactions";
                        for (int i = 0; i < insertDataTable.Columns.Count; i++)
                            bulkCopy.ColumnMappings.Add(i, i + 1);

                        await bulkCopy.WriteToServerAsync(insertDataTable);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occured in DBWriteWorker::insertApiTransactions {0}", ex.ToString());
                if (transactionItems != null)
                    Context.Parent.Tell(transactionItems);
            }
            return transactionItems.Count;
        }
        private async Task<int> updateDepositTransactions(SqlConnection connection)
        {
            List<DepositTransactionUpdateItem> updateItems = await Context.Parent.Ask<List<DepositTransactionUpdateItem>>("PopUpdatedDepositTransactions", TimeSpan.FromSeconds(5));
            if (updateItems == null)
                return 0;

            SqlTransaction transaction = null;
            try
            {
                string strQuery = "UPDATE transactions SET platformtransactionid=@platformtransactionid WHERE transactionid=@transactionid";

                transaction = connection.BeginTransaction();

                foreach (var item in updateItems)
                {
                    SqlCommand command = new SqlCommand(strQuery, connection, transaction);
                    command.Parameters.AddWithValue("@transactionid", item.TransactionID);
                    if (item.PlatformTransID == null)
                        command.Parameters.AddWithValue("@platformtransactionid", "");
                    else
                        command.Parameters.AddWithValue("@platformtransactionid", item.PlatformTransID);

                    await command.ExecuteNonQueryAsync();
                }
                transaction.Commit();
                return updateItems.Count;
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occured in DBGameLogWriteWorker::updateDepositTransactions {0}", ex.ToString());
                try
                {
                    transaction.Rollback();
                }
                catch
                {
                }
                Context.Parent.Tell(updateItems);
            }
            return updateItems.Count;
        }
        private async Task<int> insertFailedTransactions(SqlConnection connection)
        {
            List<FailedTransactionItem> insertItems = await Context.Parent.Ask<List<FailedTransactionItem>>("PopFailedTransactionItems", TimeSpan.FromSeconds(5));
            if (insertItems == null)
                return 0;

            try
            {

                DataTable dataTable = new DataTable();
                dataTable.Columns.Add("agentid", typeof(string));
                dataTable.Columns.Add("userid", typeof(string));
                dataTable.Columns.Add("transactiontype", typeof(int));
                dataTable.Columns.Add("transactionid", typeof(string));
                dataTable.Columns.Add("reftransactionid", typeof(string));
                dataTable.Columns.Add("amount", typeof(decimal));
                dataTable.Columns.Add("gameid", typeof(int));
                dataTable.Columns.Add("timestamp", typeof(DateTime));
                foreach (var item in insertItems)
                    dataTable.Rows.Add(item.AgentID, item.UserID, (int)item.TransactionType, item.TransactionID, item.RefTransactionID, (decimal)Math.Round(item.Amount, 2), item.GameID, item.UpdateTime);

                using (SqlBulkCopy bulkCopy = new SqlBulkCopy(connection))
                {
                    bulkCopy.DestinationTableName = "faildtransactions";
                    for (int i = 0; i < dataTable.Columns.Count; i++)
                        bulkCopy.ColumnMappings.Add(i, i + 1);

                    await bulkCopy.WriteToServerAsync(dataTable);
                }
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occured in DBGameLogWriteWorker::insertFailedTransactions {0}", ex.ToString());
                Context.Parent.Tell(insertItems);
            }
            return insertItems.Count;
        }

    }
}
