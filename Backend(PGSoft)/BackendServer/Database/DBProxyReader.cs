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
using SlotGamesNode.HTTPService;
using Newtonsoft.Json;

namespace SlotGamesNode.Database
{
    public class DBProxyReader : ReceiveActor
    {
        private readonly ILoggingAdapter _logger                = Logging.GetLogger(Context);
        private string                   _strConnString         = "";
        public DBProxyReader(string strConnString)
        {
            _strConnString = strConnString;

            ReceiveAsync<UserLoginRequest>                  (doLoginRequest);
            ReceiveAsync<GetUserBonusItems>                 (getUserBonusItems);
            ReceiveAsync<ClaimedGameJackpotBonus>           (fetchNewGameJackpotBonus);
            ReceiveAsync<PGBetHistoryRequest>               (onPGGetBetHistoryItems);
            ReceiveAsync<PGBetSummaryRequest>               (onPGBetSummaryRequest);

        }

        public static Props Props(string strConnString, int poolSize)
        {
            return Akka.Actor.Props.Create(() => new DBProxyReader(strConnString)).WithRouter(new RoundRobinPool(poolSize));
        }
        private async Task<GameJackpotBonusItem> fetchGameJackpotBonus(SqlConnection connection, string strUserID)
        {
            string strQuery = "SELECT id, jackpotmoney, jackpottype FROM userjackpots WHERE username=@username and processed=0";
            SqlCommand command = new SqlCommand(strQuery, connection);
            command.Parameters.AddWithValue("@username", strUserID);

            using (DbDataReader reader = await command.ExecuteReaderAsync())
            {
                if (await reader.ReadAsync())
                {
                    long    bonusID         = (long)reader["id"];
                    double  bonusMoney      = (double)(decimal)reader["jackpotmoney"];
                    int     bonusType       = (int)reader["jackpottype"];
                    GameJackpotBonusItem gameJackpotItem = new GameJackpotBonusItem(bonusID, strUserID,bonusMoney, bonusType);
                    return gameJackpotItem;
                }
            }
            return null;
        }
       
        private async Task fetchNewGameJackpotBonus(ClaimedGameJackpotBonus request)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(_strConnString))
                {
                    await connection.OpenAsync();

                    string strQuery = "SELECT id,  jackpottype, jackpotmoney FROM userjackpots WHERE username=@username and processed=0 and id>@id";
                    SqlCommand command = new SqlCommand(strQuery, connection);
                    command.Parameters.AddWithValue("@username",    request.UserID);
                    command.Parameters.AddWithValue("@id",          request.ID);
                    using (DbDataReader reader = await command.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            long    bonusID         = (long)reader["id"];
                            double  bonusMoney      = (double)(decimal)reader["jackpotmoney"];
                            int     bonusType       = (int)reader["jackpottype"];
                            GameJackpotBonusItem gameJackpotItem = new GameJackpotBonusItem(bonusID, request.UserID, bonusMoney, bonusType);
                            Sender.Tell(gameJackpotItem);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occured in DBProxyReader::fetchNewGameJackpotBonus {0}", ex);
            }
        }
        private async Task getUserBonusItems(GetUserBonusItems request)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(_strConnString))
                {
                    await connection.OpenAsync();
                    GameJackpotBonusItem gameJackpotItem = await fetchGameJackpotBonus(connection, request.UserID);
                    if(gameJackpotItem != null)
                        Sender.Tell(gameJackpotItem);
                }
            }
            catch(Exception ex)
            {
                _logger.Error("Exception has been occured in DBProxyReader::getUserBonusItems {0}", ex);
            }
        }

        private async Task onPGBetSummaryRequest(PGBetSummaryRequest request)
        {
            GetBetSummaryResponseData response = new GetBetSummaryResponseData();
            try
            {
                using (SqlConnection connection = new SqlConnection(_strConnString))
                {
                    await connection.OpenAsync();

                    string strQuery = "SELECT SUM(bet) as totalbet, SUM(profit) as totalprofit, COUNT(id) as count FROM pgbethistory WHERE userid=@userid and gameid=@gameid and timestamp >= @starttime and timestamp < @endtime";
                    SqlCommand command = new SqlCommand(strQuery, connection);
                    command.Parameters.AddWithValue("@userid",      request.UserID);
                    command.Parameters.AddWithValue("@gameid",      request.GameID);
                    command.Parameters.AddWithValue("@starttime",   request.StartTime);
                    command.Parameters.AddWithValue("@endtime",     request.EndTime);

                    using (DbDataReader reader = await command.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            if((int)reader["count"] > 0)
                            {
                                response.bs         = new BetSummaryData();
                                response.bs.btba    = (double)(decimal)reader["totalbet"];
                                response.bs.btwla   = (double)(decimal)reader["totalprofit"];
                                response.bs.bc      = (int)reader["count"];
                            }
                        }
                    }
                    if(response.bs == null)
                    {
                        Sender.Tell(response);
                        return;
                    }
                    strQuery = "SELECT TOP 1 timestamp, transactionid FROM pgbethistory WHERE userid=@userid and gameid=@gameid and timestamp >= @starttime and timestamp < @endtime ORDER BY timestamp DESC";
                    command = new SqlCommand(strQuery, connection);
                    command.Parameters.AddWithValue("@userid", request.UserID);
                    command.Parameters.AddWithValue("@gameid", request.GameID);
                    command.Parameters.AddWithValue("@starttime", request.StartTime);
                    command.Parameters.AddWithValue("@endtime", request.EndTime);
                    using (DbDataReader reader = await command.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            response.bs.lbid = (long) reader["transactionid"];
                            response.lut     = (long)reader["timestamp"];
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

        private async Task onPGGetBetHistoryItems(PGBetHistoryRequest request)
        {
            GetBetHistoryResponseData responseData = new GetBetHistoryResponseData();
            try
            {
                using (SqlConnection connection = new SqlConnection(_strConnString))
                {
                    await connection.OpenAsync();

                    int startIndex = (request.PageNumber - 1) * request.CountPerPage;
                    string strQuery = string.Format("SELECT data FROM pgbethistory WHERE userid=@userid and gameid=@gameid and timestamp >= @starttime and timestamp < @endtime ORDER BY timestamp DESC " +
                        "OFFSET {0} ROWS FETCH NEXT {1} ROWS ONLY", startIndex, request.CountPerPage);

                    SqlCommand command = new SqlCommand(strQuery, connection);
                    command.Parameters.AddWithValue("@userid",      request.UserID);
                    command.Parameters.AddWithValue("@gameid",      request.GameID);
                    command.Parameters.AddWithValue("@starttime",   request.StartTime);
                    command.Parameters.AddWithValue("@endtime",     request.EndTime);

                    using (DbDataReader reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            string strData  = (string)reader["data"];
                            dynamic data    = JsonConvert.DeserializeObject<dynamic>(strData);

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
        private async Task doLoginRequest(UserLoginRequest request)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(_strConnString))
                {
                    await connection.OpenAsync();
                    string      strQuery    = "SELECT * FROM players WHERE userid=@userid";
                    SqlCommand  command     = new SqlCommand(strQuery, connection);
                    command.Parameters.AddWithValue("@userid", request.UserID);

                    UserLoginResponse response = null;
                    LoginResult resultCode = LoginResult.IDPASSWORDMISMATCH;

                    long dbID               = 0;
                    string  strUserID       = "";
                    double  balance         = 0.0;
                    do
                    {
                        using (DbDataReader reader = await command.ExecuteReaderAsync())
                        {
                            if (!await reader.ReadAsync())
                                break;

                            dbID            = (long)                reader["id"];
                            strUserID       = (string)              reader["userid"];
                            balance         = (double) (decimal)    reader["balance"];

                            string strPassword = (string)reader["password"];
                            if (!MD5Utils.CompareMD5Hash(strPassword, request.Password))
                                break;

                        }
                        response = new UserLoginResponse(dbID, strUserID, balance);
                    } while (false);

                    if (response == null)
                        response = new UserLoginResponse(resultCode);
                    Sender.Tell(response);
                }
            }
            catch(Exception ex)
            {
                _logger.Error("Exception has been occured in DBProxyReader::doLoginRequest {0}", ex);
            }
        }        

    }
    public class PPHistoryItemDetail
    {
        public string cr { get; set; }  //client request
        public string sr { get; set; }  //server response
    }
}
