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
    public class DBWriteWorker : ReceiveActor
    {
        private string                      _strConnectionString    = "";
        private ICancelable                 _schedulerCancel        = null;
        private readonly ILoggingAdapter    _logger                 = Context.GetLogger();

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

                        await updateCompanyBalances(connection);
                        await updateBalances(connection);
                        await updatePlayerStates(connection);
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
                            
                            int updateAgentBalanceCnt   = await updateCompanyBalances(connection);
                            int updateBalanceCnt        = await updateBalances(connection);
                            int balanceCount = updateAgentBalanceCnt + updateBalanceCnt;
                            int stateCount              = await updatePlayerStates(connection);
                            if (balanceCount == 0 && stateCount == 0)
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

        private async Task<int> updatePlayerStates(SqlConnection connection)
        {
            List<UserStateUpdateItem> updatedPlayerStates = null;
            try
            {
                updatedPlayerStates = await Context.Parent.Ask<List<UserStateUpdateItem>>("PopPlayerStates", TimeSpan.FromSeconds(10.0));
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occured in DBWriteWorker while updating player states : {0}", ex.ToString());
            }
            
            if (updatedPlayerStates == null || updatedPlayerStates.Count == 0)
                return 0;
            
            List<UserStateUpdateItem> updateLoginItems = new List<UserStateUpdateItem>();
            DataTable updateLoginTable = new DataTable();
            updateLoginTable.Columns.Add("id", typeof(int));

            List<UserStateUpdateItem> updateGameItems = new List<UserStateUpdateItem>();
            DataTable updateGameTable = new DataTable();
            updateGameTable.Columns.Add("id",           typeof(int));
            updateGameTable.Columns.Add("isonline",     typeof(int));
            updateGameTable.Columns.Add("lastgameid",   typeof(int));

            List<UserStateUpdateItem> updateLobbyOffItems = new List<UserStateUpdateItem>();
            DataTable updateLobbyOffTable = new DataTable();
            updateLobbyOffTable.Columns.Add("id",           typeof(int));
            updateLobbyOffTable.Columns.Add("lastgameid",   typeof(int));
            updateLobbyOffTable.Columns.Add("balance",      typeof(Decimal));

            try
            {
                foreach (UserStateUpdateItem updateItem in updatedPlayerStates)
                {
                    if (updateItem is UserLoginStateItem)
                    {
                        updateLoginTable.Rows.Add(updateItem.PlayerID);
                        updateLoginItems.Add(updateItem);
                    }
                    else if (updateItem is UserGameStateItem) 
                    {
                        updateGameTable.Rows.Add(updateItem.PlayerID, (updateItem as UserGameStateItem).State, (updateItem as UserGameStateItem).GameID);
                        updateGameItems.Add(updateItem);
                    }
                    else
                    {
                        updateLobbyOffTable.Rows.Add(updateItem.PlayerID, (updateItem as UserOfflineStateItem).GameID, (updateItem as UserOfflineStateItem).BalanceIncrement);
                        updateLobbyOffItems.Add(updateItem);
                    }
                }
            }
            catch (Exception ex)
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
            catch (Exception ex)
            {
                _logger.Error("Exception has been occured in DBWriteWorker while updating player states : {0}", ex.ToString());
                if (updateLoginItems.Count > 0)
                    Context.Parent.Tell(updateLoginItems);
            }

            try
            {
                if (updateGameTable.Rows.Count > 0)
                {
                    SqlCommand command = new SqlCommand("UpdateUserGame", connection);
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
                    SqlCommand command = new SqlCommand("UpdateUserOffline", connection);
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.Add(new SqlParameter("@tblUpdates", updateLobbyOffTable));
                    await command.ExecuteNonQueryAsync();
                }
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occured in DBWriteWorker while updating player states : {0}", ex.ToString());
                if (updateLobbyOffItems.Count > 0)
                    Context.Parent.Tell(updateLobbyOffItems);
            }

            return updatedPlayerStates.Count;
        }

        private async Task<int> updateBalances(SqlConnection connection)
        {
            List<PlayerBalanceUpdateItem> balanceUpdates = null;
            try
            {
                balanceUpdates = await Context.Parent.Ask<List<PlayerBalanceUpdateItem>>("PopBalanceUpdates", TimeSpan.FromSeconds(5.0));
                if (balanceUpdates == null)
                    return 0;

                DataTable dataTable = new DataTable();
                dataTable.Columns.Add("balance",    typeof(Decimal));
                dataTable.Columns.Add("id",         typeof(long));

                foreach (PlayerBalanceUpdateItem updateItem in balanceUpdates)
                {
                    if (updateItem.PlayerID <= 0L)
                        _logger.Error("DBWriteWorker::updateBalances playerID <= 0 {0} {1}", updateItem.PlayerID, updateItem.BalanceIncrement);
                    
                    dataTable.Rows.Add((Decimal)updateItem.BalanceIncrement, updateItem.PlayerID);
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

        private async Task<int> updateCompanyBalances(SqlConnection connection)
        {
            List<CompanyBalanceUpdateItem> balanceUpdates = null;
            try
            {
                balanceUpdates = await Context.Parent.Ask<List<CompanyBalanceUpdateItem>>("PopCompanyBalanceUpdates", TimeSpan.FromSeconds(5.0));
                if (balanceUpdates == null)
                    return 0;
                
                DataTable dataTable = new DataTable();
                dataTable.Columns.Add("balance",    typeof(Decimal));
                dataTable.Columns.Add("id",         typeof(int));

                foreach (CompanyBalanceUpdateItem updateItem in balanceUpdates)
                    dataTable.Rows.Add((Decimal)updateItem.BalanceIncrement, updateItem.CompanyID);

                SqlCommand command = new SqlCommand("UpdateCompanyBalance", connection);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.Add(new SqlParameter("@tblBalances", dataTable));
                await command.ExecuteNonQueryAsync();
                
                return dataTable.Rows.Count;
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occured in DBWriteProxy while updating company balance : {0}", ex.ToString());
                Context.Parent.Tell(balanceUpdates);

                return -1;
            }
        }

        protected override void PostStop()
        {
            base.PostStop();
        }
    }
}
