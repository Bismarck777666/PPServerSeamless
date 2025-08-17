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

                        //플레이어들의 게임로그를 넣는다.
                        await insertGameLogs(connection);
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

                            //플레이어들의 게임로그를 넣는다.
                            int insertedLogCount            = await insertGameLogs(connection);
                            _logger.Info("{0} insert game logs  count:{1}, insert tournament bet count:{2}", DateTime.Now, insertedLogCount);

                            if (insertedLogCount == 0)
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

            Dictionary<DateTime, List<GameLogItem>> dicGameLogItems = new Dictionary<DateTime, List<GameLogItem>>();
            foreach (GameLogItem item in insertItems)
            {
                DateTime dateTime = item.LogTime.AddHours(9);
                dateTime = new DateTime(dateTime.Year, dateTime.Month, dateTime.Day);

                if (!dicGameLogItems.ContainsKey(dateTime))
                    dicGameLogItems.Add(dateTime, new List<GameLogItem>());

                dicGameLogItems[dateTime].Add(item);
            }

            int totalCount = 0;
            foreach (KeyValuePair<DateTime, List<GameLogItem>> pair in dicGameLogItems)
            {
                //먼저 해당날자의 표가 이미 창조된것인지를 검사한다.
                if (!WriterSnapshot.Instance.IsGameLogTableCreated(pair.Key))
                {
                    try
                    {
                        SqlCommand command = new SqlCommand("usp_CreateLogTables", connection);
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.Add(new SqlParameter("@date", pair.Key));
                        await command.ExecuteNonQueryAsync();

                        WriterSnapshot.Instance.HasCreatedGameLogTable(pair.Key);
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
                    dataTable.Columns.Add("username", typeof(string));
                    dataTable.Columns.Add("gameid", typeof(int));
                    dataTable.Columns.Add("gamename", typeof(string));
                    dataTable.Columns.Add("tableid", typeof(string));
                    dataTable.Columns.Add("bet", typeof(decimal));
                    dataTable.Columns.Add("win", typeof(decimal));
                    dataTable.Columns.Add("beginmoney", typeof(decimal));
                    dataTable.Columns.Add("endmoney", typeof(decimal));
                    dataTable.Columns.Add("gamelog", typeof(string));
                    dataTable.Columns.Add("logtime", typeof(DateTime));

                    foreach (GameLogItem item in pair.Value)
                        dataTable.Rows.Add(item.UserID, item.GameID, item.GameName, item.TableID, (decimal)Math.Round(item.Bet, 2), (decimal)Math.Round(item.Win, 2), (decimal)Math.Round(item.BeginMoney, 2), (decimal)Math.Round(item.EndMoney, 2), item.GameLog, item.LogTime);


                    using (SqlBulkCopy bulkCopy = new SqlBulkCopy(connection))
                    {
                        bulkCopy.DestinationTableName = string.Format("gamelog{0}", pair.Key.ToString("yyMMdd"));
                        for (int i = 0; i < dataTable.Columns.Count; i++)
                            bulkCopy.ColumnMappings.Add(i, i + 1);

                        await bulkCopy.WriteToServerAsync(dataTable);
                    }

                    totalCount += dataTable.Rows.Count;
                }
                catch (Exception ex)
                {
                    _logger.Error("Exception has been occured in DBWriteWorker while inserting game logs : {0}", ex.ToString());

                    //기록에 실패한 항목들을 다시 넣는다.
                    Context.Parent.Tell(pair.Value);
                    break;
                }
            }

            return totalCount;
        }
    }
}
