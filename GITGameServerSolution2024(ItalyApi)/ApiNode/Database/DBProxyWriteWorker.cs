using Akka.Actor;
using Akka.Event;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QueenApiNode.Database
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

            _schedulerCancel = Context.System.Scheduler.ScheduleTellOnceCancelable(500, Self, "write", ActorRefs.NoSender);
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
                        await updateAgentBalances(connection);
                        await insertAgentMoneyChangeItems(connection);
                        await insertUserMoneyChangeItems(connection);
                    }
                }
                catch (Exception ex)
                {
                    _logger.Error("Exception has been occured in DBProxyWriteWorker::processCommand {0}", ex.ToString());
                }
                _schedulerCancel = Context.System.Scheduler.ScheduleTellOnceCancelable(500, Self, "write", ActorRefs.NoSender);
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
                        int updateCount = await updateAgentBalances(connection);
                        int insertCount = await insertAgentMoneyChangeItems(connection);
                        insertCount    += await insertUserMoneyChangeItems(connection);

                        if (updateCount == 0 && insertCount == 0)
                            break;
                    }
                } while (true);
                _logger.Info("Flush Ended");
            }
        }

        private async Task<int> updateAgentBalances(SqlConnection connection)
        {
            List<AgentScoreUpdateItem> updateItems = null;
            SqlTransaction transaction = null;
            try
            {
                updateItems = await Context.Parent.Ask<List<AgentScoreUpdateItem>>("PopAgentScoreUpdates", TimeSpan.FromSeconds(5));
                if (updateItems == null)
                    return 0;

                transaction = connection.BeginTransaction();
                SqlCommand command = new SqlCommand("UPDATE agents SET score=score+@amount WHERE id=@id", connection, transaction);
                foreach (AgentScoreUpdateItem updateItem in updateItems)
                {
                    command.Parameters.Clear();
                    command.Parameters.AddWithValue("@amount", updateItem.Score);
                    command.Parameters.AddWithValue("@id", updateItem.DBID);
                    await command.ExecuteNonQueryAsync();
                }
                transaction.Commit();
                return updateItems.Count;
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occured in DBProxyWriteBalanceWorker::updateAgentBalances {0}", ex.ToString());
                try
                {
                    transaction.Rollback();
                }
                catch
                {

                }
                //기록에 실패한 항목들을 다시 넣는다.
                Context.Parent.Tell(updateItems);
                return -1;
            }
        }

        private async Task<int> insertAgentMoneyChangeItems(SqlConnection connection)
        {
            List<AgentMoneyChangeItem> insertItems = await Context.Parent.Ask<List<AgentMoneyChangeItem>>("PopAgentMoneyChangeItems", TimeSpan.FromSeconds(5));
            if (insertItems == null)
                return 0;

            try
            {
                DataTable moneyChangeTable = new DataTable();
                moneyChangeTable.Columns.Add("agentid", typeof(string));
                moneyChangeTable.Columns.Add("otheraccount", typeof(string));
                moneyChangeTable.Columns.Add("changemoney", typeof(decimal));
                moneyChangeTable.Columns.Add("beforemoney", typeof(decimal));
                moneyChangeTable.Columns.Add("aftermoney", typeof(decimal));
                moneyChangeTable.Columns.Add("mode", typeof(int));
                moneyChangeTable.Columns.Add("updatetime", typeof(DateTime));

                foreach (AgentMoneyChangeItem item in insertItems)
                    moneyChangeTable.Rows.Add(item.SubjectAccount, item.OtherAccount, item.Money, item.BeforeMoney, item.AfterMoney, (int)item.Mode, item.ProcTime);

                using (SqlBulkCopy bulkCopy = new SqlBulkCopy(connection))
                {
                    bulkCopy.DestinationTableName = "agentscorelogs";
                    for (int i = 0; i < moneyChangeTable.Columns.Count; i++)
                        bulkCopy.ColumnMappings.Add(i, i + 1);

                    await bulkCopy.WriteToServerAsync(moneyChangeTable);
                }
                return moneyChangeTable.Rows.Count;
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occured in DBProxyWriteWorker::insertAgentMoneyChangeItems {0}", ex.ToString());

                //기록에 실패한 항목들을 다시 넣는다.
                Context.Parent.Tell(insertItems);
                return -1;
            }
        }
        private async Task<int> insertUserMoneyChangeItems(SqlConnection connection)
        {
            List<UserMoneyChangeItem> insertItems = await Context.Parent.Ask<List<UserMoneyChangeItem>>("PopUserMoneyChangeItems", TimeSpan.FromSeconds(5));
            if (insertItems == null)
                return 0;

            try
            {
                DataTable moneyChangeTable = new DataTable();
                moneyChangeTable.Columns.Add("agentname",       typeof(string));
                moneyChangeTable.Columns.Add("username",        typeof(string));
                moneyChangeTable.Columns.Add("changemoney",     typeof(decimal));
                moneyChangeTable.Columns.Add("beforemoney",     typeof(decimal));
                moneyChangeTable.Columns.Add("aftermoney",      typeof(decimal));
                moneyChangeTable.Columns.Add("mode",            typeof(int));
                moneyChangeTable.Columns.Add("updatetime",      typeof(DateTime));

                foreach (UserMoneyChangeItem item in insertItems)
                    moneyChangeTable.Rows.Add(item.AgentID, item.UserID, item.Amount, item.BeforeMoney, item.AfterMoney, (int)item.Mode, item.UpdateTime);

                using (SqlBulkCopy bulkCopy = new SqlBulkCopy(connection))
                {
                    bulkCopy.DestinationTableName = "userscorelogs";
                    for (int i = 0; i < moneyChangeTable.Columns.Count; i++)
                        bulkCopy.ColumnMappings.Add(i, i + 1);

                    await bulkCopy.WriteToServerAsync(moneyChangeTable);
                }
                return moneyChangeTable.Rows.Count;
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occured in DBProxyWriteWorker::insertUserMoneyChangeItems {0}", ex.ToString());

                //기록에 실패한 항목들을 다시 넣는다.
                Context.Parent.Tell(insertItems);
                return -1;
            }
        }

    }
}
