using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Akka.Actor;
using Akka.Event;
using System.Data.SqlClient;
using System.Data;

namespace SlotGamesNode.Database
{
    public class DBWriteWorker : ReceiveActor
    {
        private string                      _strConnectionString    = "";
        private ICancelable                 _schedulerCancel        = null;
        private readonly ILoggingAdapter    _logger                 = Logging.GetLogger(Context);

        public DBWriteWorker(string strConnectionString)
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
            return Akka.Actor.Props.Create(() => new DBWriteWorker(strConnString));
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

                        await updateBalances(connection);
                        await updatePlayerStates(connection);
                        await processBonusItems(connection);
                        await insertPGGameHistory(connection);

                    }
                }
                catch (Exception ex)
                {
                    _logger.Error("Exception has been occured in DBWriteWorker::processCommand {0}", ex.ToString());
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
                            int balanceCount    = await updateBalances(connection);
                            int stateCount      = await updatePlayerStates(connection);
                            int bonusCount      = await processBonusItems(connection);
                            int gameLogCount    = await insertPGGameHistory(connection);
                            if (balanceCount == 0 && stateCount == 0 && bonusCount == 0 && gameLogCount == 0)
                                break;
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.Error("Exception has been occured in DBWriteWorker::processCommand {0}", ex.ToString());
                    }
                } while (true);

                _logger.Info("Flush Ended");
            }
        }
        private async Task<int> updatePlayerStates(SqlConnection connection)
        {
            List<UserStateUpdateItem> updatedPlayerStates = null;
            try
            {
                updatedPlayerStates = await Context.Parent.Ask<List<UserStateUpdateItem>>("PopPlayerStates", TimeSpan.FromSeconds(10));
            }
            catch(Exception ex)
            {
                _logger.Error("Exception has been occured in DBWriteWorker while updating player states : {0}", ex.ToString());
            }

            if (updatedPlayerStates == null || updatedPlayerStates.Count == 0)
                return 0;

            List<UserStateUpdateItem> updateLoginItems = new List<UserStateUpdateItem>();
            DataTable updateLoginTable = new DataTable();
            updateLoginTable.Columns.Add("id", typeof(Int32));
            updateLoginTable.Columns.Add("isonline", typeof(Int32));

            try
            {

                foreach (UserStateUpdateItem updateItem in updatedPlayerStates)
                {
                    updateLoginTable.Rows.Add(updateItem.PlayerID, updateItem.IsOnline);
                }
            }
            catch(Exception ex)
            {
                _logger.Error("Exception has been occured in DBWriteWorker while updating player states : {0}", ex.ToString());
                if (updatedPlayerStates != null)
                    Context.Parent.Tell(updatedPlayerStates);
            }

            try
            {
                if (updateLoginTable.Rows.Count > 0)
                {
                    SqlCommand command = new SqlCommand("UpdateUserOnline", connection);
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.Add(new SqlParameter("@tblUpdates", updateLoginTable));
                    await command.ExecuteNonQueryAsync();
                }
            }
            catch(Exception ex)
            {
                _logger.Error("Exception has been occured in DBWriteWorker while updating player states : {0}", ex.ToString());

                
                if (updateLoginItems.Count > 0)
                    Context.Parent.Tell(updateLoginItems);
            }
            return updatedPlayerStates.Count;
        }

        private async Task<int> updateBalances(SqlConnection connection)
        {
            List<PlayerBalanceUpdateItem> balanceUpdates = null;
            try
            {
                balanceUpdates = await Context.Parent.Ask<List<PlayerBalanceUpdateItem>>("PopBalanceUpdates", TimeSpan.FromSeconds(5));
                if (balanceUpdates == null)
                    return 0;

                DataTable dataTable = new DataTable();
                dataTable.Columns.Add("balance",    typeof(decimal));
                dataTable.Columns.Add("id",         typeof(int));

                foreach (PlayerBalanceUpdateItem updateItem in balanceUpdates)
                {
                    if(updateItem.PlayerID <= 0)
                        _logger.Error("DBWriteWorker::updateBalances playerID <= 0 {0} {1}", updateItem.PlayerID, updateItem.BalanceIncrement);

                    dataTable.Rows.Add((decimal)updateItem.BalanceIncrement, updateItem.PlayerID);
                }

                SqlCommand command = new SqlCommand("UpdateBalance", connection);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.Add(new SqlParameter("@tblBalances", dataTable));

                await command.ExecuteNonQueryAsync();
                return dataTable.Rows.Count;
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occured in DBWriteProxy while updating balance : {0}", ex.ToString());

                
                Context.Parent.Tell(balanceUpdates);
                return -1;
            }
        }        
        private async Task<int> processBonusItems(SqlConnection connection)
        {
            int totalCount = 0;
            totalCount += await updateClaimedBonusItems(connection);
            return totalCount;
        }        
        private async Task<int> updateClaimedBonusItems(SqlConnection connection)
        {
            List<BaseClaimedUserBonusUpdateItem>    updateItems = null;
            SqlTransaction                          transaction = null;

            try
            {

                updateItems = await Context.Parent.Ask<List<BaseClaimedUserBonusUpdateItem>>("PopClaimedBonusItems", TimeSpan.FromSeconds(5));
                if (updateItems == null)
                    return 0;

                transaction = connection.BeginTransaction();

                foreach (BaseClaimedUserBonusUpdateItem updateItem in updateItems)
                {
                    if(updateItem is ClaimedGameJackpotItem)
                    {
                        SqlCommand command = new SqlCommand("UPDATE userjackpots SET processed=1, processedtime=@processedtime WHERE id=@id", connection, transaction);
                        command.Parameters.AddWithValue("@id",              (updateItem as ClaimedGameJackpotItem).BonusID);
                        command.Parameters.AddWithValue("@processedtime",   (updateItem as ClaimedGameJackpotItem).ClaimedTime);
                        await command.ExecuteNonQueryAsync();
                    }                    
                }
                transaction.Commit();
                return updateItems.Count;
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occured in DBWriteWorker while updating claimed bonus items : {0}", ex.ToString());
                try
                {
                    transaction.Rollback();
                }
                catch
                {

                }

                
                if(updateItems != null)
                    Context.Parent.Tell(updateItems);
                return updateItems.Count;
            }
        }

        private async Task<int> insertPGGameHistory(SqlConnection connection)
        {
            List<PGGameHistoryDBItem> historyItems = null;
            try
            {
                historyItems = await Context.Parent.Ask<List<PGGameHistoryDBItem>>("PopPGGameHistoryItems", TimeSpan.FromSeconds(5));
                if (historyItems == null)
                    return 0;

                DataTable dataTable = new DataTable();
                dataTable.Columns.Add("userid",         typeof(string));
                dataTable.Columns.Add("gameid",         typeof(int));
                dataTable.Columns.Add("bet",            typeof(decimal));
                dataTable.Columns.Add("profit",         typeof(decimal));
                dataTable.Columns.Add("timestamp",      typeof(long));
                dataTable.Columns.Add("transactionid",  typeof(long));
                dataTable.Columns.Add("data",           typeof(string));

                foreach (PGGameHistoryDBItem item in historyItems)
                    dataTable.Rows.Add(item.UserID, item.GameID, item.Bet, item.Profit, item.Timestamp, item.TransactionID, item.Data);

                using (SqlBulkCopy bulkCopy = new SqlBulkCopy(connection))
                {
                    bulkCopy.DestinationTableName = "pgbethistory";
                    for (int i = 0; i < dataTable.Columns.Count; i++)
                        bulkCopy.ColumnMappings.Add(i, i + 1);

                    await bulkCopy.WriteToServerAsync(dataTable);
                }
                return historyItems.Count;
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occured in DBWriteWorker::insertPGGameHistory {0}", ex.ToString());
                if (historyItems != null && historyItems.Count > 0)
                    Context.Parent.Tell(historyItems);
                return -1;
            }
        }

        protected override void PostStop()
        {
            base.PostStop();
        }

    }
}
