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
                        await insertPPGameHistory(connection);
                        await insertPPRecentGameHistory(connection);
                        await insertPPRaceWinners(connection);
                        await insertPPCashbackWinners(connection);
                        await insertPPFreeSpinReports(connection);
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
                        insertCount     += await insertPPRaceWinners(connection);

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
                dataTable.Columns.Add("username",   typeof(string));
                dataTable.Columns.Add("gameid",     typeof(int));
                dataTable.Columns.Add("balance",    typeof(decimal));
                dataTable.Columns.Add("bet",        typeof(decimal));
                dataTable.Columns.Add("win",        typeof(decimal));
                dataTable.Columns.Add("detaillog",  typeof(string));
                dataTable.Columns.Add("timestamp", typeof(long));

                foreach (PPGameRecentHistoryDBItem item in historyItems)
                    dataTable.Rows.Add(item.UserName, item.GameID, item.Balance, item.Bet, item.Win,  item.DetailLog, item.DateTime);


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
                historyItems = await Context.Parent.Ask<List<PPGameHistoryDBItem>>("PopPPGameHistoryItems", TimeSpan.FromSeconds(5));
                if (historyItems == null)
                    return 0;

                DataTable dataTable = new DataTable();
                dataTable.Columns.Add("username",   typeof(string));
                dataTable.Columns.Add("gameid",     typeof(int));
                dataTable.Columns.Add("bet",        typeof(decimal));
                dataTable.Columns.Add("basebet",    typeof(decimal));
                dataTable.Columns.Add("win",        typeof(decimal));
                dataTable.Columns.Add("rtp",        typeof(decimal));
                dataTable.Columns.Add("detaillog",  typeof(string));
                dataTable.Columns.Add("playeddate", typeof(long));

                foreach (PPGameHistoryDBItem item in historyItems)
                    dataTable.Rows.Add(item.UserName, item.GameID, item.Bet, item.BaseBet, item.Win, item.RTP, item.DetailLog, item.PlayedDate);


                SqlCommand command  = new SqlCommand("UpsertPPUserTopGameLog", connection);
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
        private async Task<int> insertPPRaceWinners(SqlConnection connection)
        {
            List<PPRaceWinnerDBItem> historyItems = null;
            try
            {
                historyItems = await Context.Parent.Ask<List<PPRaceWinnerDBItem>>("PopPPRaceWinnerItems", TimeSpan.FromSeconds(5));
                if (historyItems == null)
                    return 0;
                
                DataTable dataTable = new DataTable();
                dataTable.Columns.Add("raceid",         typeof(int));
                dataTable.Columns.Add("prizeid",        typeof(int));
                dataTable.Columns.Add("agentid",        typeof(string));
                dataTable.Columns.Add("username",       typeof(string));
                dataTable.Columns.Add("usertype",       typeof(int));
                dataTable.Columns.Add("country",        typeof(string));
                dataTable.Columns.Add("currency",       typeof(string));
                dataTable.Columns.Add("bet",            typeof(decimal));
                dataTable.Columns.Add("win",            typeof(decimal));
                dataTable.Columns.Add("processed",      typeof(int));
                dataTable.Columns.Add("gamename",       typeof(string));
                dataTable.Columns.Add("type",           typeof(string));
                dataTable.Columns.Add("isagent",        typeof(int));
                dataTable.Columns.Add("updatetime",     typeof(DateTime));
                dataTable.Columns.Add("processedtime",  typeof(DateTime));

                foreach (PPRaceWinnerDBItem item in historyItems)
                    dataTable.Rows.Add(item.RaceID, item.PrizeID, item.AgentID, item.UserName, item.UserType, item.Country, item.Currency, (decimal)item.Bet, (decimal)item.Win, item.Processed,item.GameName, item.PrizeType,item.IsAgent, item.UpdateTime, item.ProcessedTime);


                using (SqlBulkCopy bulkCopy = new SqlBulkCopy(connection))
                {
                    bulkCopy.DestinationTableName = "ppracewinners";
                    for (int i = 0; i < dataTable.Columns.Count; i++)
                        bulkCopy.ColumnMappings.Add(i, i + 1);

                    await bulkCopy.WriteToServerAsync(dataTable);
                }

                return dataTable.Rows.Count;
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occured in DBProxyWriteWorker::insertPPRaceWinners {0}", ex.ToString());

                if (historyItems != null && historyItems.Count > 0)
                    Context.Parent.Tell(historyItems);

                return -1;
            }
        }

        private async Task<int> insertPPCashbackWinners(SqlConnection connection)
        {
            List<PPCashbackWinnerDBItem> historyItems = null;
            try
            {
                historyItems = await Context.Parent.Ask<List<PPCashbackWinnerDBItem>>("PopPPCashbackWinnerItems", TimeSpan.FromSeconds(5));
                if (historyItems == null)
                    return 0;

                DataTable dataTable = new DataTable();
                dataTable.Columns.Add("raceid", typeof(int));                
                dataTable.Columns.Add("agentid", typeof(string));
                dataTable.Columns.Add("username", typeof(string));                
                dataTable.Columns.Add("country", typeof(string));
                dataTable.Columns.Add("currency", typeof(string));
                dataTable.Columns.Add("cashback", typeof(decimal));                
                dataTable.Columns.Add("gamename", typeof(string));                
                dataTable.Columns.Add("isagent", typeof(int));
                dataTable.Columns.Add("period", typeof(int));
                dataTable.Columns.Add("periodkey", typeof(string));
                dataTable.Columns.Add("updatetime", typeof(DateTime));
                

                foreach (PPCashbackWinnerDBItem item in historyItems)
                    dataTable.Rows.Add(item.RaceID, item.AgentID, item.UserName,  item.Country,
                        item.Currency, (decimal)item.Cashback, item.GameName,  item.IsAgent,
                        item.Period, item.PeriodKey, item.UpdateTime);


                using (SqlBulkCopy bulkCopy = new SqlBulkCopy(connection))
                {
                    bulkCopy.DestinationTableName = "ppcashbackreports";
                    for (int i = 0; i < dataTable.Columns.Count; i++)
                        bulkCopy.ColumnMappings.Add(i, i + 1);

                    await bulkCopy.WriteToServerAsync(dataTable);
                }

                return dataTable.Rows.Count;
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occured in DBProxyWriteWorker::insertPPCashbackWinners {0}", ex.ToString());

                if (historyItems != null && historyItems.Count > 0)
                    Context.Parent.Tell(historyItems);

                return -1;
            }
        }
        private async Task<int> insertPPFreeSpinReports(SqlConnection connection)
        {
            List<PPFreeSpinReportDBItem> historyItems = null;
            try
            {
                historyItems = await Context.Parent.Ask<List<PPFreeSpinReportDBItem>>("PopPPFreeSpinReports", TimeSpan.FromSeconds(5));
                if (historyItems == null)
                    return 0;

                DataTable dataTable = new DataTable();
                dataTable.Columns.Add("fsid", typeof(int));                
                dataTable.Columns.Add("agentid", typeof(string));
                dataTable.Columns.Add("username", typeof(string));                                
                dataTable.Columns.Add("currency", typeof(string));
                dataTable.Columns.Add("bet", typeof(decimal));
                dataTable.Columns.Add("win", typeof(decimal));
                dataTable.Columns.Add("awardedcount", typeof(int));
                dataTable.Columns.Add("remaincount", typeof(int));
                dataTable.Columns.Add("games", typeof(string));
                dataTable.Columns.Add("startdate", typeof(DateTime));            
                dataTable.Columns.Add("updatetime", typeof(DateTime));
              

                foreach (PPFreeSpinReportDBItem item in historyItems)
                    dataTable.Rows.Add(item.fsID, item.AgentID, item.Username, item.Currency, (decimal)item.Bet, (decimal)item.Win, item.AwardedCount, item.RemainCount, item.Games, item.Startdate, item.Updatetime);


                using (SqlBulkCopy bulkCopy = new SqlBulkCopy(connection))
                {
                    bulkCopy.DestinationTableName = "ppfreespinreports";
                    for (int i = 0; i < dataTable.Columns.Count; i++)
                        bulkCopy.ColumnMappings.Add(i, i + 1);

                    await bulkCopy.WriteToServerAsync(dataTable);
                }

                return dataTable.Rows.Count;
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occured in DBProxyWriteWorker::PopPPFreeSpinReports {0}", ex.ToString());

                if (historyItems != null && historyItems.Count > 0)
                    Context.Parent.Tell(historyItems);

                return -1;
            }
        }
    }
}
