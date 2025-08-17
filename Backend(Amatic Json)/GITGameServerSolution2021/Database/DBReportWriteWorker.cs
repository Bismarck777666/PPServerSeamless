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


                        //플레이어 리포트변경을 디비에 기록한다.
                        await updateReports(connection);

                        await updateUserRolling(connection);

                        await updateAgentRolling(connection);

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

                        var stopWatch = new System.Diagnostics.Stopwatch();
                        stopWatch.Start();

                        //플레이어 리포트변경을 디비에 기록한다.
                        int reportCount         = await updateReports(connection);

                        int userRollingCount    = await updateUserRolling(connection);

                        int agentRollingCount   = await updateAgentRolling(connection);

                        int gameReportCount     = await updateGameReports(connection);

                        stopWatch.Stop();

                        double elapsed = stopWatch.Elapsed.TotalMilliseconds;

                        _logger.Info("report updated at {0}, report count: {1}, user rolling: {2}, agent rolling: {3}, game report: {4}, elapsed time:{5}", DateTime.Now, reportCount, userRollingCount,
                            agentRollingCount, gameReportCount, elapsed);

                        if (reportCount == 0 && userRollingCount == 0 && agentRollingCount == 0 && gameReportCount == 0)
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
                dataTable.Columns.Add("agentid",        typeof(int));
                dataTable.Columns.Add("reportdate",     typeof(DateTime));
                dataTable.Columns.Add("turnover",       typeof(decimal));

                foreach (GameReportItem updateItem in reportUpdateItems)
                    dataTable.Rows.Add(updateItem.GameID, updateItem.AgentID, updateItem.ReportDate, updateItem.Turnover);


                SqlCommand command = new SqlCommand("UpdateGameReports", connection);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.Add(new SqlParameter("@tblGameReports", dataTable));
                await command.ExecuteNonQueryAsync();
                return dataTable.Rows.Count;
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occured in DBReportWriteWorker::updateGameReports while updating game reports : {0}", ex.ToString());

                //기록에 실패한 항목들을 다시 넣는다.
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
                dataTable.Columns.Add("bet", typeof(decimal));
                dataTable.Columns.Add("win", typeof(decimal));
                dataTable.Columns.Add("turnover", typeof(decimal));
                dataTable.Columns.Add("username", typeof(string));
                dataTable.Columns.Add("gametype", typeof(Int16));
                dataTable.Columns.Add("reporttime", typeof(DateTime));
                dataTable.Columns.Add("agentid", typeof(int));

                Dictionary<long, AgentUpdateTable> dicAgentUpdateTable = new Dictionary<long, AgentUpdateTable>();
                foreach (ReportUpdateItem updateItem in reportUpdateItems)
                    dataTable.Rows.Add(updateItem.Bet, updateItem.Win, updateItem.Turnover, updateItem.UserID, 1, updateItem.ReportDateTime, updateItem.AgentID);


                SqlCommand command = new SqlCommand("UpdateReports", connection);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.Add(new SqlParameter("@tblReports", dataTable));
                await command.ExecuteNonQueryAsync();
                return dataTable.Rows.Count;
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occured in DBReportWriteWorker::updateReports while updating report : {0}", ex.ToString());

                //기록에 실패한 항목들을 다시 넣는다.
                if (reportUpdateItems != null && reportUpdateItems.Count > 0)
                    Context.Parent.Tell(reportUpdateItems);

                return -1;
            }
        }

        private async Task<int> updateUserRolling(SqlConnection connection)
        {
            List<UserRollingAdded> userRollingAdds = null;
            try
            {
                userRollingAdds = await Context.Parent.Ask<List<UserRollingAdded>>("PopUserRollingAdds", TimeSpan.FromSeconds(5));
                if (userRollingAdds == null)
                    return 0;

                DataTable dataTable = new DataTable();
                dataTable.Columns.Add("rollpoint",  typeof(decimal));
                dataTable.Columns.Add("id",         typeof(long));

                foreach (UserRollingAdded userRolling in userRollingAdds)
                    dataTable.Rows.Add(userRolling.RollPoint, userRolling.UserDBID);


                SqlCommand command = new SqlCommand("AddUserRolling", connection);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.Add(new SqlParameter("@tblRolling", dataTable));
                await command.ExecuteNonQueryAsync();
                return dataTable.Rows.Count;
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occured in DBReportWriteWorker::updateUserRolling while updating user rolling : {0}", ex.ToString());

                //기록에 실패한 항목들을 다시 넣는다.
                if (userRollingAdds != null && userRollingAdds.Count > 0)
                    Context.Parent.Tell(userRollingAdds);

                return -1;
            }
        }

        private async Task<int> updateAgentRolling(SqlConnection connection)
        {
            List<AgentRollingAdded> agentRollingAdds = null;
            try
            {
                agentRollingAdds = await Context.Parent.Ask<List<AgentRollingAdded>>("PopAgentRollingAdds", TimeSpan.FromSeconds(5));
                if (agentRollingAdds == null)
                    return 0;

                DataTable dataTable = new DataTable();
                dataTable.Columns.Add("rollpoint",  typeof(decimal));
                dataTable.Columns.Add("id",         typeof(int));

                foreach (AgentRollingAdded userRolling in agentRollingAdds)
                    dataTable.Rows.Add(userRolling.RollPoint, userRolling.AgentDBID);


                SqlCommand command = new SqlCommand("AddAgentRolling", connection);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.Add(new SqlParameter("@tblRolling", dataTable));
                await command.ExecuteNonQueryAsync();
                return dataTable.Rows.Count;
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occured in DBReportWriteWorker::updateAgentRolling while updating agent rolling : {0}", ex.ToString());

                //기록에 실패한 항목들을 다시 넣는다.
                if (agentRollingAdds != null && agentRollingAdds.Count > 0)
                    Context.Parent.Tell(agentRollingAdds);

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
