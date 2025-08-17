using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Akka.Actor;
using Akka.Event;
using System.Data.SqlClient;
using System.Data;

namespace CommNode.Database
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

                        await insertDBInsertItems(connection);

                        await processBonusItems(connection);

                        await updateUserBetMoney(connection);
                        await resetBalances(connection);

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

                            int balanceCount = await updateBalances(connection);

                            int stateCount  = await updatePlayerStates(connection);

                            int insertItems = await insertDBInsertItems(connection);

                            int bonusCount  = await processBonusItems(connection);

                            if (balanceCount == 0 && stateCount == 0 && bonusCount == 0 && insertItems == 0)
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
            updateLoginTable.Columns.Add("lastplatform", typeof(Int32));

            List<UserStateUpdateItem> updateGameItems = new List<UserStateUpdateItem>();
            DataTable updateGameTable = new DataTable();
            updateGameTable.Columns.Add("id", typeof(Int32));
            updateGameTable.Columns.Add("isonline", typeof(Int32));
            updateGameTable.Columns.Add("lastgame", typeof(Int32));

            List<UserStateUpdateItem> updateLobbyOffItems = new List<UserStateUpdateItem>();
            DataTable updateLobbyOffTable = new DataTable();
            updateLobbyOffTable.Columns.Add("id", typeof(Int32));
            updateLobbyOffTable.Columns.Add("isonline", typeof(Int32));
            updateLobbyOffTable.Columns.Add("lastgame", typeof(Int32));
            updateLobbyOffTable.Columns.Add("balance", typeof(decimal));

            try
            {

                foreach (UserStateUpdateItem updateItem in updatedPlayerStates)
                {
                    if (updateItem is UserLoginStateItem)
                    {
                        updateLoginTable.Rows.Add(updateItem.PlayerID, (updateItem as UserLoginStateItem).Platform);
                        updateLoginItems.Add(updateItem);
                    }
                    else if (updateItem is UserGameStateItem)
                    {
                        updateGameTable.Rows.Add(updateItem.PlayerID, (updateItem as UserGameStateItem).State, (updateItem as UserGameStateItem).GameID);
                        updateGameItems.Add(updateItem);
                    }
                    else
                    {
                        updateLobbyOffTable.Rows.Add(updateItem.PlayerID, 0, (updateItem as UserOfflineStateItem).GameID, (updateItem as UserOfflineStateItem).BalanceIncrement);
                        updateLobbyOffItems.Add(updateItem);
                    }
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
#if FORTEST
                    SqlCommand command = new SqlCommand("UpdateTestUserOnline", connection);
#else
                    SqlCommand command = new SqlCommand("UpdateUserOnline", connection);
#endif
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

            try
            {
                if (updateGameTable.Rows.Count > 0)
                {
#if FORTEST
                    SqlCommand command  = new SqlCommand("UpdateTestPlayerGame", connection);
#else
                    SqlCommand command = new SqlCommand("UpdateUserGame", connection);
#endif
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.Add(new SqlParameter("@tblUpdates", updateGameTable));
                    await command.ExecuteNonQueryAsync();
                }
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occured in DBWriteWorker while updating player states : {0}", ex.ToString());

                if (updateGameItems.Count > 0)
                    Context.Parent.Tell(updateGameItems);
            }

            try
            {
                if (updateLobbyOffTable.Rows.Count > 0)
                {
#if FORTEST
                    SqlCommand command = new SqlCommand("UpdateTestUserOffline", connection);
#else
                    SqlCommand command = new SqlCommand("UpdateUserOffline", connection);
#endif
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.Add(new SqlParameter("@tblUpdates", updateLobbyOffTable));
                    await command.ExecuteNonQueryAsync();
                }
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occured in DBWriteWorker while updating player states : {0}", ex.ToString());

                if(updateLobbyOffItems.Count > 0)
                    Context.Parent.Tell(updateLobbyOffItems);
            }
            return updatedPlayerStates.Count;
        }
        private async Task<int> updateUserBetMoney(SqlConnection connection)
        {
            List<UserBetMoneyUpdateItem> betMoneyUpdates = null;
            try
            {
                betMoneyUpdates = await Context.Parent.Ask<List<UserBetMoneyUpdateItem>>("PopUserBetMoneyUpdates", TimeSpan.FromSeconds(5));
                if (betMoneyUpdates == null)
                    return 0;

                DataTable dataTable = new DataTable();
                dataTable.Columns.Add("id", typeof(long));
                dataTable.Columns.Add("betmoney", typeof(decimal));

                foreach (UserBetMoneyUpdateItem updateItem in betMoneyUpdates)
                    dataTable.Rows.Add(updateItem.UserDBID, updateItem.UserBetMoney);

                SqlCommand command = new SqlCommand("UpdateUserBetMoney", connection);

                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.Add(new SqlParameter("@tblBetMoney", dataTable));

                await command.ExecuteNonQueryAsync();
                return dataTable.Rows.Count;
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occured in DBWriteProxy while updating user betmoney : {0}", ex.ToString());

                Context.Parent.Tell(betMoneyUpdates);
                return -1;
            }
        }
        
        private async Task<int> resetBalances(SqlConnection connection)
        {
            List<PlayerBalanceResetItem> balanceResets = null;
            try
            {
                balanceResets = await Context.Parent.Ask<List<PlayerBalanceResetItem>>("PopBalanceResets", TimeSpan.FromSeconds(5));
                if (balanceResets == null)
                    return 0;

                DataTable dataTable = new DataTable();
                dataTable.Columns.Add("balance", typeof(decimal));
                dataTable.Columns.Add("id",     typeof(int));

                foreach (PlayerBalanceResetItem updateItem in balanceResets)
                {
                    if (updateItem.PlayerID <= 0)
                        _logger.Error("DBWriteWorker::resetBalances playerID <= 0 {0} {1}", updateItem.PlayerID, updateItem.Balance);

                    dataTable.Rows.Add((decimal)updateItem.Balance, updateItem.PlayerID);
                }
#if FORTEST
                SqlCommand command = new SqlCommand("UpdateTestBalance", connection);
#else
                SqlCommand command = new SqlCommand("ResetBalance", connection);
#endif
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.Add(new SqlParameter("@tblBalances", dataTable));

                await command.ExecuteNonQueryAsync();
                return dataTable.Rows.Count;
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occured in DBWriteProxy while resetting balance : {0}", ex.ToString());

                Context.Parent.Tell(balanceResets);
                return -1;
            }
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
                dataTable.Columns.Add("id", typeof(int));
                dataTable.Columns.Add("balance",    typeof(decimal));

                foreach (PlayerBalanceUpdateItem updateItem in balanceUpdates)
                {
                    if(updateItem.PlayerID <= 0)
                        _logger.Error("DBWriteWorker::updateBalances playerID <= 0 {0} {1}", updateItem.PlayerID, updateItem.BalanceIncrement);

                    dataTable.Rows.Add(updateItem.PlayerID, (decimal)updateItem.BalanceIncrement);
                }
#if FORTEST
                SqlCommand command = new SqlCommand("UpdateTestBalance", connection);
#else
                SqlCommand command = new SqlCommand("UpdateBalance", connection);
#endif
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
        private async Task<int> insertDBInsertItems(SqlConnection connection)
        {
            List<BaseInsertItem> insertItems = await Context.Parent.Ask<List<BaseInsertItem>>("PopInsertItems", TimeSpan.FromSeconds(5));
            if (insertItems == null)
                return 0;

            try
            {

                DataTable loginIPDatatable = new DataTable();
                loginIPDatatable.Columns.Add("username",   typeof(string));
                loginIPDatatable.Columns.Add("ip",         typeof(string));
                loginIPDatatable.Columns.Add("logindata",  typeof(string));
                loginIPDatatable.Columns.Add("logintime",  typeof(DateTime));

                foreach (BaseInsertItem insertItem in insertItems)
                {
                    if(insertItem is LoginIPInsertItem)
                    {
                        LoginIPInsertItem loginIPItem = insertItem as LoginIPInsertItem;
                        loginIPDatatable.Rows.Add(loginIPItem.UserID, loginIPItem.IPAddress, loginIPItem.LoginData, loginIPItem.LoginTime);
                    }
                }


                using (SqlBulkCopy bulkCopy = new SqlBulkCopy(connection))
                {
                    bulkCopy.DestinationTableName = "loginiplogs";
                    for (int i = 0; i < loginIPDatatable.Columns.Count; i++)
                        bulkCopy.ColumnMappings.Add(i, i + 1);

                    await bulkCopy.WriteToServerAsync(loginIPDatatable);
                }
                return loginIPDatatable.Rows.Count;
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occured in DBWriteWorker while inserting loginIP logs : {0}", ex.ToString());

                Context.Parent.Tell(insertItems);
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
                        SqlCommand command = new SqlCommand("UPDATE userjackpots SET processed=1, gameid=@gameid, processedtime=@processedtime WHERE id=@id", connection, transaction);
                        command.Parameters.AddWithValue("@id",              (updateItem as ClaimedGameJackpotItem).BonusID);
                        command.Parameters.AddWithValue("@processedtime",   (updateItem as ClaimedGameJackpotItem).ClaimedTime);
                        command.Parameters.AddWithValue("@gameid",          (updateItem as ClaimedGameJackpotItem).GameID);
                        await command.ExecuteNonQueryAsync();
                    }
                    else if(updateItem is ClaimedUserEventItem)
                    {
                        SqlCommand command = new SqlCommand("UPDATE userevents SET amount=@amount, gamename=@gamename, processed=1, processeddate=@processeddate WHERE id=@id", connection, transaction);
                        command.Parameters.AddWithValue("@id",          (updateItem as ClaimedUserEventItem).EventID);
                        command.Parameters.AddWithValue("@amount",      (updateItem as ClaimedUserEventItem).RewardedMoney);
                        command.Parameters.AddWithValue("@gamename",    (updateItem as ClaimedUserEventItem).GameName);
                        command.Parameters.AddWithValue("@processeddate",(updateItem as ClaimedUserEventItem).ClaimedTime);
                        await command.ExecuteNonQueryAsync();
                    }
                    else if (updateItem is ClaimedUserRangeEventItem)
                    {
                        SqlCommand command = new SqlCommand("UPDATE userrangeevents SET amount=@amount, gamename=@gamename, processed=1, processeddate=@processeddate WHERE id=@id", connection, transaction);
                        command.Parameters.AddWithValue("@id",              (updateItem as ClaimedUserRangeEventItem).EventID);
                        command.Parameters.AddWithValue("@amount",          (updateItem as ClaimedUserRangeEventItem).RewardedMoney);
                        command.Parameters.AddWithValue("@gamename",        (updateItem as ClaimedUserRangeEventItem).GameName);
                        command.Parameters.AddWithValue("@processeddate",   (updateItem as ClaimedUserRangeEventItem).ClaimedTime);
                        await command.ExecuteNonQueryAsync();
                    }
                    else if(updateItem is ClaimedRedPacketItem)
                    {
                        SqlCommand command = new SqlCommand("UPDATE userredpackets SET processed=1, processedtime=@processedtime WHERE id=@id", connection, transaction);
                        command.Parameters.AddWithValue("@id",            (updateItem as ClaimedRedPacketItem).RedPacketID);
                        command.Parameters.AddWithValue("@processedtime", (updateItem as ClaimedRedPacketItem).ClaimedTime);
                        command.ExecuteNonQuery();
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
        protected override void PostStop()
        {
            base.PostStop();
        }

    }
}
