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
using Newtonsoft.Json;

namespace CommNode.Database
{
    public class DBProxyReader : ReceiveActor
    {
        private readonly ILoggingAdapter _logger                = Logging.GetLogger(Context);
        private string                   _strConnString         = "";
        public DBProxyReader(string strConnString)
        {
            _strConnString = strConnString;

            //유저로그인 요청을 처리한다.
            ReceiveAsync<UserLoginRequest>                  (doLoginRequest);
            ReceiveAsync<GetUserBonusItems>                 (getUserBonusItems);
            ReceiveAsync<ClaimedUserRangeEventMessage>      (fetchNewUserRangeEvent);
            ReceiveAsync<GetAgentRollingFees>               (onGetAgentRollingFees);
        }

        public static Props Props(string strConnString, int poolSize)
        {
            return Akka.Actor.Props.Create(() => new DBProxyReader(strConnString)).WithRouter(new RoundRobinPool(poolSize));
        }
        
        private async Task<UserRangeOddEventItem> fetchUserRangeOddEventItem(SqlConnection connection, string strUserID)
        {
            string strQuery = "SELECT TOP (1) id,  minodd, maxodd, maxbet FROM userrangeevents WHERE username=@username and processed=0 ORDER BY id";
            SqlCommand command = new SqlCommand(strQuery, connection);
            command.Parameters.AddWithValue("@username", strUserID);

            using (DbDataReader reader = await command.ExecuteReaderAsync())
            {
                if (await reader.ReadAsync())
                {
                    long eventBonusID = (long)reader["id"];
                    double minOdd = (double)(decimal)reader["minodd"];
                    double maxOdd = (double)(decimal)reader["maxodd"];
                    double maxBet = (double)(decimal)reader["maxbet"];

                    UserRangeOddEventItem userEventItem = new UserRangeOddEventItem(eventBonusID, strUserID, minOdd, maxOdd,maxBet);
                    return userEventItem;
                }
            }
            return null;
        }
        
        private async Task fetchNewUserRangeEvent(ClaimedUserRangeEventMessage request)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(_strConnString))
                {
                    await connection.OpenAsync();

                    string strQuery = "SELECT id,  minodd, maxodd,maxbet FROM userrangeevents WHERE username=@username and processed=0 and id>@id";
                    SqlCommand command = new SqlCommand(strQuery, connection);
                    command.Parameters.AddWithValue("@username", request.UserID);
                    command.Parameters.AddWithValue("@id",       request.ID);

                    using (DbDataReader reader = await command.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            long eventBonusID   = (long)reader["id"];
                            double minOdd       = (double)(decimal)reader["minodd"];
                            double maxOdd       = (double)(decimal)reader["maxodd"];
                            double maxBet       = (double)(decimal)reader["maxbet"];

                            UserRangeOddEventItem userEventItem = new UserRangeOddEventItem(eventBonusID, request.UserID, minOdd, maxOdd,maxBet);
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
        
        private async Task getUserBonusItems(GetUserBonusItems request)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(_strConnString))
                {
                    await connection.OpenAsync();
                    
                    UserRangeOddEventItem userRangeOddEventItem = await fetchUserRangeOddEventItem(connection, request.UserID);
                    if (userRangeOddEventItem != null)
                        Sender.Tell(userRangeOddEventItem);

                }
            }
            catch(Exception ex)
            {
                _logger.Error("Exception has been occured in DBProxyReader::getUserBonusItems {0}", ex);
            }
        }

        private async Task onGetAgentRollingFees(GetAgentRollingFees request)
        {
            try
            {
                Dictionary<int, double> agentRollingFees = new Dictionary<int, double>();
                using (SqlConnection connection = new SqlConnection(_strConnString))
                {
                    await connection.OpenAsync();

                    string      strQuery    = string.Format("SELECT id, rollfee FROM agents WHERE id in ({0})", string.Join(",", request.AgentIDs.ToArray()));
                    SqlCommand  command     = new SqlCommand(strQuery, connection);
                    using (DbDataReader reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            int     agentID     = (int)                 reader["id"];
                            double  rollingFee  = (double) (decimal)    reader["rollfee"];

                            agentRollingFees.Add(agentID, rollingFee);
                        }
                    }
                }
                Sender.Tell(agentRollingFees);
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occurred in DBProxyReader::onGetAgentRollingFees {0}", ex);
                Sender.Tell(new Dictionary<int, double>());
            }
        }
        
        private async Task doLoginRequest(UserLoginRequest request)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(_strConnString))
                {
                    await connection.OpenAsync();
                    string strQuery = "SELECT * FROM users WHERE username=@username";
                    SqlCommand command = new SqlCommand(strQuery, connection);
                    command.Parameters.AddWithValue("@username", request.UserID);

                    UserLoginResponse response = null;
                    LoginResult resultCode = LoginResult.IDPASSWORDMISMATCH;

                    long dbID               = 0;
                    string  strUserID       = "";
                    double  balance         = 0.0;
                    string  strAgentName    = "";
                    int     agentID         = 0;
                    string  strAgentIDs     = "";
                    long    lastScoreID     = 0;
                    string  strCountry      = null;
                    int     currency        = 0;
                    string  strUserToken    = "";
                    double  rollingPer      = 0.0;
                    do
                    {
                        using (DbDataReader reader = await command.ExecuteReaderAsync())
                        {
                            if (!await reader.ReadAsync())
                                break;

                            dbID            = (long)                reader["id"];
                            strUserID       = (string)              reader["username"];
                            balance         = (double) (decimal)    reader["balance"];
                            currency        = (int)                 reader["currency"];
                            strAgentName    = (string)              reader["agentname"];
                            agentID         = (int)                 reader["agentid"];
                            strAgentIDs     = (string)              reader["agentids"];
                            lastScoreID     = (long)                reader["lastscoreid"];
                            rollingPer      = (double) (decimal)    reader["rollfee"];

                            string strPasswordMD5 = (string)reader["password"];
                            if (!MD5Utils.CompareMD5Hashes(request.Password, strPasswordMD5))
                                break;

                            strUserToken = string.Format("{0}@{1}", strUserID, strPasswordMD5);
                            int state = (int)reader["state"];
                            if (state != 1)
                            {
                                resultCode = LoginResult.ACCOUNTDISABLED;
                                break;
                            }
                        }

                        response = new UserLoginResponse(dbID, strUserID, strUserToken, balance, (Currencies)currency, strAgentName, agentID, strAgentIDs, lastScoreID, request.IPAddress, strCountry, rollingPer);
                    } while (false);

                    if (response == null)
                        response = new UserLoginResponse(resultCode);

                    //응답을 보낸다.
                    Sender.Tell(response);
                }
            }
            catch(Exception ex)
            {
                _logger.Error("Exception has been occured in DBProxyReader::doLoginRequest {0}", ex);
            }
        }        
    }
}
