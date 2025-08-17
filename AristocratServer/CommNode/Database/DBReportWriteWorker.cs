using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Akka.Actor;
using Akka.Event;
using System.Data.SqlClient;
using System.Data;
using System.Data.Common;

namespace CommNode.Database
{
    public class DBReportWriteWorker : ReceiveActor
    {
        private string                      _strConnectionString    = "";
        private ICancelable                 _schedulerCancel        = null;
        private readonly ILoggingAdapter    _logger                 = Logging.GetLogger(Context);
            
        public DBReportWriteWorker(string strConnectionString)
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
            return Akka.Actor.Props.Create(() => new DBReportWriteWorker(strConnString));
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
                        await updateReports(connection);
                        await updateGameReports(connection);
                    }
                }
                catch (Exception ex)
                {
                    _logger.Error("Exception has been occured in DBReportWriteWorker::processCommand {0}", ex.ToString());
                }
                _schedulerCancel = Context.System.Scheduler.ScheduleTellOnceCancelable(1000, Self, "write", ActorRefs.NoSender);
            }
            else if(strCommand == "flush")
            {
                if (_schedulerCancel != null)
                    _schedulerCancel.Cancel();

                _logger.Info("Flush Started");

                do
                {
                    using (SqlConnection connection = new SqlConnection(_strConnectionString))
                    {
                        await connection.OpenAsync();

                        int reportCount         = await updateReports(connection);
                        int gameReportCount     = await updateGameReports(connection);
                        if (reportCount == 0 && gameReportCount == 0)
                            break;
                    }
                } while (true);
                _logger.Info("Flush Ended");
            }
        }

        private async Task<int> updateGameReports(SqlConnection connection)
        {
            List<GameReportItem> reportUpdateItems = null;
            try
            {
                reportUpdateItems = await Context.Parent.Ask<List<GameReportItem>>("PopGameReportUpdates", TimeSpan.FromSeconds(5));
                if (reportUpdateItems == null)
                    return 0;

                DataTable dataTable = new DataTable();
                dataTable.Columns.Add("gameid",         typeof(int));
                dataTable.Columns.Add("currency",       typeof(int));
                dataTable.Columns.Add("reportdate",     typeof(DateTime));
                dataTable.Columns.Add("turnover",       typeof(decimal));

                foreach (GameReportItem updateItem in reportUpdateItems)
                    dataTable.Rows.Add(updateItem.GameID, updateItem.Currency, updateItem.ReportDate, updateItem.Turnover);

                SqlCommand command = new SqlCommand("UpdateGameReports", connection);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.Add(new SqlParameter("@tblGameReports", dataTable));
                await command.ExecuteNonQueryAsync();
                return dataTable.Rows.Count;
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occured in DBReportWriteWorker::updateGameReports while updating game reports : {0}", ex.ToString());

                if (reportUpdateItems != null && reportUpdateItems.Count > 0)
                    Context.Parent.Tell(reportUpdateItems);

                return -1;
            }
        }
        private async Task<int> updateReports(SqlConnection connection)
        {
            List<ReportUpdateItem> reportUpdateItems = null;
            try
            {
                reportUpdateItems = await Context.Parent.Ask<List<ReportUpdateItem>>("PopReportUpdates", TimeSpan.FromSeconds(5));
                if (reportUpdateItems == null)
                    return 0;

                DataTable dataTable = new DataTable();
                dataTable.Columns.Add("username", typeof(string));
                dataTable.Columns.Add("gametype", typeof(int));
                dataTable.Columns.Add("bet", typeof(decimal));
                dataTable.Columns.Add("win", typeof(decimal));
                dataTable.Columns.Add("turnover", typeof(decimal));
                dataTable.Columns.Add("agentid", typeof(int));
                dataTable.Columns.Add("reporttime", typeof(DateTime));

                Dictionary<long, AgentUpdateTable> dicAgentUpdateTable = new Dictionary<long, AgentUpdateTable>();
                foreach (ReportUpdateItem updateItem in reportUpdateItems)
                    //dataTable.Rows.Add(updateItem.Bet, updateItem.Win, updateItem.Turnover, updateItem.UserID, 1, updateItem.ReportDateTime, updateItem.AgentID);
                    dataTable.Rows.Add(updateItem.UserID, 1, updateItem.Bet, updateItem.Win, updateItem.Turnover, updateItem.AgentID, updateItem.ReportDateTime);


                SqlCommand command = new SqlCommand("UpdateReports", connection);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.Add(new SqlParameter("@tblReports", dataTable));
                await command.ExecuteNonQueryAsync();
                return dataTable.Rows.Count;
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occured in DBReportWriteWorker::updateReports while updating report : {0}", ex.ToString());

                if (reportUpdateItems != null && reportUpdateItems.Count > 0)
                    Context.Parent.Tell(reportUpdateItems);

                return -1;
            }
        }
    }

    public class AgentUpdateTable
    {
        public double Bet { get; set; }
        public double Win { get; set; }
        public double Turnover { get; set; }

        public AgentUpdateTable(double bet, double win, double turnover)
        {
            this.Bet = bet;
            this.Win = win;
            this.Turnover = turnover;
        }

        public void mergeUpdates(double bet, double win, double turnover)
        {
            this.Bet += bet;
            this.Win += win;
            this.Turnover += turnover;
        }
    }
}
