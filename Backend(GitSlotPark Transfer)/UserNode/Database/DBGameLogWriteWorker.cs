using Akka.Actor;
using Akka.Event;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace UserNode.Database
{
    public class DBGameLogWriteWorker : ReceiveActor
    {
        private string                      _strConnectionString    = "";
        private ICancelable                 _schedulerCancel        = null;
        private readonly ILoggingAdapter    _logger                 = Context.GetLogger();

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
                        await insertApiTransactions(connection);
                        await insertGameLogs(connection);
                    }
                }
                catch (Exception ex)
                {
                    _logger.Error("Exception has been occured in DBProxyWriter {0}", ex.ToString());
                }
                _schedulerCancel = Context.System.Scheduler.ScheduleTellOnceCancelable(1000, Self, "write", ActorRefs.NoSender);
            }
            else if (strCommand == "flush") 
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
                            int transactionCount = await insertApiTransactions(connection);
                            int insertedLogCount = await insertGameLogs(connection);
                            _logger.Info("{0} insert game logs  count:{1}", DateTime.Now, (insertedLogCount + transactionCount));
                            if (insertedLogCount == 0 && transactionCount == 0)
                                break;
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.Error("Exception has been occured in DBProxyWriter {0}", ex.ToString());
                    }
                }
                while (true);
                _logger.Info("Flush Ended");
            }
        }

        private async Task<int> insertApiTransactions(SqlConnection connection)
        {
            List<ApiTransactionUpsertItem> transactionItems = null;
            SqlTransaction transaction = null;
            try
            {
                transactionItems = await Context.Parent.Ask<List<ApiTransactionUpsertItem>>("PopApiTransactionItems", TimeSpan.FromSeconds(5.0));
                if (transactionItems == null || transactionItems.Count == 0)
                    return 0;
                
                DataTable insertDataTable = new DataTable();
                insertDataTable.Columns.Add("userid",               typeof(string));
                insertDataTable.Columns.Add("transactionid",        typeof(long));
                insertDataTable.Columns.Add("relatedtransactionid", typeof(long));
                insertDataTable.Columns.Add("transactiontype",      typeof(int));
                insertDataTable.Columns.Add("typeid",               typeof(int));
                insertDataTable.Columns.Add("gameid",               typeof(int));
                insertDataTable.Columns.Add("withdrawamount",       typeof(Decimal));
                insertDataTable.Columns.Add("depositamount",        typeof(Decimal));
                insertDataTable.Columns.Add("rollbacked",           typeof(int));
                insertDataTable.Columns.Add("timestamp",            typeof(DateTime));

                DataTable updateDataTable = new DataTable();
                updateDataTable.Columns.Add("userid",               typeof(string));
                updateDataTable.Columns.Add("transid",              typeof(long));
                
                foreach (ApiTransactionUpsertItem upsertItem in transactionItems)
                {
                    if (upsertItem is ApiTransactionItem)
                    {
                        ApiTransactionItem item = upsertItem as ApiTransactionItem;
                        insertDataTable.Rows.Add(item.UserID, item.TransactionID, item.RelTransactionID, (int)item.TransactionType, (int)item.TransTypeID, item.GameID, item.WithAmount, item.DepAmount, 0, item.Timestamp);
                    }
                    else
                    {
                        ApiTransactionUpdateItem updateItem = upsertItem as ApiTransactionUpdateItem;
                        updateDataTable.Rows.Add(updateItem.UserID, updateItem.TransID);
                    }
                }
                
                transaction = connection.BeginTransaction();
                if (insertDataTable.Rows.Count > 0)
                {
                    using (SqlBulkCopy bulkCopy = new SqlBulkCopy(connection, SqlBulkCopyOptions.Default, transaction))
                    {
                        bulkCopy.DestinationTableName = "apitransactions";
                        for (int i = 0; i < insertDataTable.Columns.Count; i++)
                            bulkCopy.ColumnMappings.Add(i, i + 1);

                        await bulkCopy.WriteToServerAsync(insertDataTable);
                    }
                }
                if (updateDataTable.Rows.Count > 0)
                {
                    using (SqlCommand command = new SqlCommand("RollbackTransations", connection, transaction))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@tblUpdates", updateDataTable);
                        
                        await command.ExecuteNonQueryAsync();
                    }
                }
                transaction.Commit();
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occured in DBWriteWorker while upserting transaction items : {0}", ex.ToString());
                if (transaction != null)
                {
                    try
                    {
                        transaction.Rollback();
                    }
                    catch
                    {
                    }
                }
                
                if (transactionItems != null)
                    Context.Parent.Tell(transactionItems);
            }
            return transactionItems.Count;
        }

        private async Task<int> insertGameLogs(SqlConnection connection)
        {
            List<GameLogItem> insertItems = await Context.Parent.Ask<List<GameLogItem>>("PopGameLogItems", TimeSpan.FromSeconds(5.0));
            if (insertItems == null)
                return 0;
            
            Dictionary<int, List<GameLogItem>> dicApiGameLogItems = new Dictionary<int, List<GameLogItem>>();
            foreach (GameLogItem item in insertItems)
            {
                DateTime dateTime = item.LogTime;
                dateTime = new DateTime(dateTime.Year, dateTime.Month, dateTime.Day);

                if (!dicApiGameLogItems.ContainsKey(item.AgentID))
                    dicApiGameLogItems.Add(item.AgentID, new List<GameLogItem>());

                dicApiGameLogItems[item.AgentID].Add(item);
            }

            int totalCount = 0;
            foreach (KeyValuePair<int, List<GameLogItem>> pair in dicApiGameLogItems)
            {
                //먼저 해당에이전트의 표가 이미 창조된것인지를 검사한다.
                if (!WriterSnapshot.Instance.IsAgentGameLogTableCreated(pair.Key))
                {
                    try
                    {
                        SqlCommand command = new SqlCommand("usp_CreateAgentLogTables", connection);
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.Add(new SqlParameter("@agentid", pair.Key));
                        await command.ExecuteNonQueryAsync();
                        
                        WriterSnapshot.Instance.HasCreatedAgentGameLogTable(pair.Key);
                    }
                    catch (Exception ex)
                    {
                        _logger.Error("Create GameLog Table {0} failed : {1}", pair.Key, ex.ToString());
                    }
                }

                //게임로그표에 게임로그들을 삽입한다.
                try
                {
                    DataTable dataTable = new DataTable();
                    dataTable.Columns.Add("username",   typeof(string));
                    dataTable.Columns.Add("gameid",     typeof(int));
                    dataTable.Columns.Add("gamename",   typeof(string));
                    dataTable.Columns.Add("bettype",    typeof(int));
                    dataTable.Columns.Add("bet",        typeof(Decimal));
                    dataTable.Columns.Add("win",        typeof(Decimal));
                    dataTable.Columns.Add("beginmoney", typeof(Decimal));
                    dataTable.Columns.Add("endmoney",   typeof(Decimal));
                    dataTable.Columns.Add("gamelog",    typeof(string));
                    dataTable.Columns.Add("logtime",    typeof(DateTime));

                    foreach (GameLogItem item in pair.Value)
                        dataTable.Rows.Add(item.UserID, item.GameID, item.GameName, item.BetType, (Decimal)Math.Round(item.Bet, 2), (Decimal)Math.Round(item.Win, 2), (Decimal)Math.Round(item.BeginMoney, 2), (Decimal)Math.Round(item.EndMoney, 2), item.GameLog, item.LogTime);
                    
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
    }
}
