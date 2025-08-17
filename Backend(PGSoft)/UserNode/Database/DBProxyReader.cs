using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Akka.Actor;
using System.Data.Common;
using Akka.Routing;
using GITProtocol;
using Akka.Event;
using System.Data.SqlClient;
using System.Drawing.Imaging;
using Newtonsoft.Json;

namespace UserNode.Database
{
    public class DBProxyReader : ReceiveActor
    {
        private readonly ILoggingAdapter _logger                = Logging.GetLogger(Context);
        private string                   _strConnString         = "";
        public DBProxyReader(string strConnString)
        {
            _strConnString = strConnString;

            //유저로그인 요청을 처리한다.
            ReceiveAsync<UserOfflineStateItem>              (onUserOfflineUpdate);
            ReceiveAsync<PGBetHistoryRequest>               (onPGGetBetHistoryItems);
            ReceiveAsync<PGBetSummaryRequest>               (onPGBetSummaryRequest);
            ReceiveAsync<PGBetHistoryDetailRequest>         (onPGGetBetHistoryDetail);
            ReceiveAsync<ClaimedUserRangeEventMessage>      (fetchNewUserRangeEvent);
            
        }

        public static Props Props(string strConnString, int poolSize)
        {
            return Akka.Actor.Props.Create(() => new DBProxyReader(strConnString)).WithRouter(new RoundRobinPool(poolSize));
        }

        private async Task onUserOfflineUpdate(UserOfflineStateItem request)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(_strConnString))
                {
                    await connection.OpenAsync();

                    string strQuery = "UPDATE users SET balance=balance+@balanceupdate, isonline=0, lastgameid=@lastgameid WHERE id=@id";
                    SqlCommand command = new SqlCommand(strQuery, connection);
                    command.Parameters.AddWithValue("@id", request.PlayerID);
                    command.Parameters.AddWithValue("@balanceupdate", request.BalanceIncrement);
                    command.Parameters.AddWithValue("@lastgameid", request.GameID);

                    await command.ExecuteNonQueryAsync();

                    Sender.Tell(true);
                    return;
                }
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occured in DBProxyReader::onUserOfflineUpdate {0}", ex);
            }
            Sender.Tell(false);
        }
        
        private Dictionary<string, string> splitFormUrlEncoded(string strContent)
        {
            string[] strParts = strContent.Split(new string[] { "&" }, StringSplitOptions.RemoveEmptyEntries);

            Dictionary<string, string> dicReturn = new Dictionary<string, string>();
            for(int i = 0; i < strParts.Length; i++)
            {
                string[] strParamValues = strParts[i].Split(new string[] { "=" }, StringSplitOptions.RemoveEmptyEntries);

                if (strParamValues.Length == 1)
                    dicReturn.Add(strParamValues[0], null);
                else if(strParamValues.Length == 2)
                    dicReturn.Add(strParamValues[0], strParamValues[1]);
            }
            return dicReturn;
        }
        private bool splitGlobalUserID(string strGlobalUserID, out int agentID, out string strUserID)
        {
            agentID = 0;
            strUserID = null;
            try
            {
                int index = strGlobalUserID.IndexOf("_");
                if (index <= 0 || index >= strGlobalUserID.Length - 1)
                    return false;

                agentID = int.Parse(strGlobalUserID.Substring(0, index));
                strUserID = strGlobalUserID.Substring(index + 1);
                return true;
            }
            catch (Exception)
            {
                return false;
            }

        }

        private async Task onPGGetBetHistoryItems(PGBetHistoryRequest request)
        {
            GetBetHistoryResponseData responseData = new GetBetHistoryResponseData();
            try
            {
                using (SqlConnection connection = new SqlConnection(_strConnString))
                {
                    await connection.OpenAsync();

                    int startIndex = (request.PageNumber - 1) * request.CountPerPage;
                    string strQuery = string.Format("SELECT data FROM pgbethistory WHERE agentid=@agentid and userid=@userid and gameid=@gameid and timestamp >= @starttime and timestamp < @endtime ORDER BY timestamp DESC " +
                        "OFFSET {0} ROWS FETCH NEXT {1} ROWS ONLY", startIndex, request.CountPerPage);

                    SqlCommand command = new SqlCommand(strQuery, connection);
                    command.Parameters.AddWithValue("@agentid",     request.AgentID);
                    command.Parameters.AddWithValue("@userid",      request.UserID);
                    command.Parameters.AddWithValue("@gameid",      request.GameID);
                    command.Parameters.AddWithValue("@starttime",   request.StartTime);
                    command.Parameters.AddWithValue("@endtime",     request.EndTime);

                    using (DbDataReader reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            string strData = (string)reader["data"];
                            dynamic data = JsonConvert.DeserializeObject<dynamic>(strData);

                            responseData.bh.Add(data);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occured in DBProxyReader::onPGGetBetHistoryItems {0}", ex);
            }
            Sender.Tell(responseData);
        }
        private async Task onPGGetBetHistoryDetail(PGBetHistoryDetailRequest request)
        {
            GetBetHistoryDetailResponseData responseData = new GetBetHistoryDetailResponseData();
            try
            {
                using (SqlConnection connection = new SqlConnection(_strConnString))
                {
                    await connection.OpenAsync();

                    string strQuery = string.Format("SELECT data FROM pgbethistory WHERE agentid=@agentid AND userid=@userid AND gameid=@gameid AND transactionid=@transactionid");

                    SqlCommand command = new SqlCommand(strQuery, connection);
                    command.Parameters.AddWithValue("@agentid",         request.AgentID);
                    command.Parameters.AddWithValue("@userid",          request.UserID);
                    command.Parameters.AddWithValue("@gameid",          request.GameID);
                    command.Parameters.AddWithValue("@transactionid",   request.SpinRoundID);

                    using (DbDataReader reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            string strData = (string)reader["data"];
                            dynamic data = JsonConvert.DeserializeObject<dynamic>(strData);

                            responseData.bh = data;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occured in DBProxyReader::onPGGetBetHistoryDetail {0}", ex);
            }
            Sender.Tell(responseData);
        }
        private async Task onPGBetSummaryRequest(PGBetSummaryRequest request)
        {
            GetBetSummaryResponseData response = new GetBetSummaryResponseData();
            try
            {
                using (SqlConnection connection = new SqlConnection(_strConnString))
                {
                    await connection.OpenAsync();

                    string strQuery = "SELECT SUM(bet) as totalbet, SUM(profit) as totalprofit, COUNT(id) as count FROM pgbethistory WHERE agentid=@agentid and userid=@userid and gameid=@gameid and timestamp >= @starttime and timestamp < @endtime";
                    SqlCommand command = new SqlCommand(strQuery, connection);
                    command.Parameters.AddWithValue("@agentid",     request.AgentID);
                    command.Parameters.AddWithValue("@userid",      request.UserID);
                    command.Parameters.AddWithValue("@gameid",      request.GameID);
                    command.Parameters.AddWithValue("@starttime",   request.StartTime);
                    command.Parameters.AddWithValue("@endtime",     request.EndTime);

                    using (DbDataReader reader = await command.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            if ((int)reader["count"] > 0)
                            {
                                response.bs = new BetSummaryData();
                                response.bs.btba = (double)(decimal)reader["totalbet"];
                                response.bs.btwla = (double)(decimal)reader["totalprofit"];
                                response.bs.bc = (int)reader["count"];
                                response.bs.gid = request.GameID;

                            }
                        }
                    }
                    if (response.bs == null)
                    {
                        Sender.Tell(response);
                        return;
                    }
                    strQuery = "SELECT TOP 1 timestamp, transactionid FROM pgbethistory WHERE agentid=@agentid and userid=@userid and gameid=@gameid and timestamp >= @starttime and timestamp < @endtime ORDER BY timestamp DESC";
                    command = new SqlCommand(strQuery, connection);
                    command.Parameters.AddWithValue("@agentid",     request.AgentID);
                    command.Parameters.AddWithValue("@userid",      request.UserID);
                    command.Parameters.AddWithValue("@gameid",      request.GameID);
                    command.Parameters.AddWithValue("@starttime",   request.StartTime);
                    command.Parameters.AddWithValue("@endtime",     request.EndTime);
                    using (DbDataReader reader = await command.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            response.bs.lbid    = (long)reader["transactionid"];
                            response.lut        = (long)reader["timestamp"];
                        }
                    }
                    Sender.Tell(response);
                    return;
                }

            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occured in DBProxyReader::onRequestPPHistoryList {0}", ex);
            }
            Sender.Tell(response);
        }
        private async Task fetchNewUserRangeEvent(ClaimedUserRangeEventMessage request)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(_strConnString))
                {
                    await connection.OpenAsync();

                    string      strQuery = "SELECT id,  minodd, maxodd,maxbet FROM userrangeevents WHERE agentid=@agentid and username=@username AND processed=0 AND id>@id";
                    SqlCommand  command  = new SqlCommand(strQuery, connection);
                    command.Parameters.AddWithValue("@agentid",     request.AgentID);
                    command.Parameters.AddWithValue("@username",    request.UserID);
                    command.Parameters.AddWithValue("@id",          request.ID);

                    using (DbDataReader reader = await command.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            long eventBonusID = (long)reader["id"];
                            double minOdd = (double)(decimal)reader["minodd"];
                            double maxOdd = (double)(decimal)reader["maxOdd"];
                            double maxBet = (double)(decimal)reader["maxbet"];

                            UserRangeOddEventItem userEventItem = new UserRangeOddEventItem(eventBonusID, request.AgentID, request.UserID, minOdd, maxOdd, maxBet);
                            Sender.Tell(userEventItem);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occured in DBProxyReader::fetchNewUserRangeEvent {0}", ex);
            }
        }
        
    }
}
