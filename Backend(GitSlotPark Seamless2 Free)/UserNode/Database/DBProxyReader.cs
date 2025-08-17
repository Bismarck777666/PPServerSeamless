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
            ReceiveAsync<GetUserBonusItems>                 (getUserBonusItems);
            ReceiveAsync<UserOfflineStateItem>              (onUserOfflineUpdate);
            ReceiveAsync<ClaimedUserRangeEventMessage>      (fetchNewUserRangeEvent);
            ReceiveAsync<ClaimedUserFreeSpinEventMessage>   (fetchNewUserFreeSpinEvent);            
        }

        public static Props Props(string strConnString, int poolSize)
        {
            return Akka.Actor.Props.Create(() => new DBProxyReader(strConnString)).WithRouter(new RoundRobinPool(poolSize));
        }

        private async Task getUserBonusItems(GetUserBonusItems request)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(_strConnString))
                {
                    await connection.OpenAsync();
                    UserRangeOddEventItem userRangeOddEventItem = await fetchUserRangeOddEventItem(connection, request.AgentID, request.UserID);
                    if (userRangeOddEventItem != null)
                        Sender.Tell(userRangeOddEventItem);

                    List<UserFreeSpinEventItem> userFreeSpinItems = await fetchUserFreeSpinEventItem(connection, request.AgentID, request.UserID);
                    if (userFreeSpinItems != null)
                        Sender.Tell(userFreeSpinItems);
                }
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occured in DBProxyReader::getUserBonusItems {0}", ex);
            }
        }
        private async Task<List<UserFreeSpinEventItem>> fetchUserFreeSpinEventItem(SqlConnection connection, int agentID, string strUserID)
        {
            string strQuery = "SELECT id, gameid, freespincount, betlevel, freespinid, expiretime FROM userfreespinevents WHERE agentid=@agentid and username=@username and processed=0 and expiretime > @nowtime ORDER BY id";
            SqlCommand command = new SqlCommand(strQuery, connection);
            command.Parameters.AddWithValue("@agentid",     agentID);
            command.Parameters.AddWithValue("@username",    strUserID);
            command.Parameters.AddWithValue("@nowtime",     DateTime.UtcNow);

            using (DbDataReader reader = await command.ExecuteReaderAsync())
            {
                List<UserFreeSpinEventItem> userFreeSpinEventItems = new List<UserFreeSpinEventItem>();

                while (await reader.ReadAsync())
                {
                    long eventBonusID   = (long)reader["id"];
                    int freeSpinCount   = (int)reader["freespincount"];
                    int betLevel        = (int)reader["betlevel"];
                    int gameid          = (int)reader["gameid"];
                    DateTime expireTime = (DateTime)reader["expiretime"];
                    string freeSpinID   = (string)reader["freespinid"];

                    UserFreeSpinEventItem userEventItem = new UserFreeSpinEventItem(eventBonusID, agentID, strUserID, gameid, freeSpinCount, betLevel, expireTime, freeSpinID);
                    userFreeSpinEventItems.Add(userEventItem);
                }
                return userFreeSpinEventItems;
            }
        }
        private async Task<UserRangeOddEventItem> fetchUserRangeOddEventItem(SqlConnection connection, int agentID, string strUserID)
        {
            string strQuery = "SELECT TOP (1) id,  minodd, maxodd,maxbet FROM userrangeevents WHERE agentid=@agentid and username=@username and processed=0 ORDER BY id";
            SqlCommand command = new SqlCommand(strQuery, connection);
            command.Parameters.AddWithValue("@agentid",  agentID);
            command.Parameters.AddWithValue("@username", strUserID);
            using (DbDataReader reader = await command.ExecuteReaderAsync())
            {
                if (await reader.ReadAsync())
                {
                    long eventBonusID = (long)reader["id"];
                    double minOdd = (double)(decimal)reader["minodd"];
                    double maxOdd = (double)(decimal)reader["maxodd"];
                    double maxBet = (double)(decimal)reader["maxbet"];

                    UserRangeOddEventItem userEventItem = new UserRangeOddEventItem(eventBonusID, agentID, strUserID, minOdd, maxOdd, maxBet);
                    return userEventItem;
                }
            }
            return null;
        }
        private async Task fetchNewUserFreeSpinEvent(ClaimedUserFreeSpinEventMessage request)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(_strConnString))
                {
                    await connection.OpenAsync();

                    string strQuery = "SELECT id, gameid, freespincount, betlevel, freespinid, expiretime FROM userfreespinevents WHERE agentid=@agentid AND username=@username AND processed=0 AND expiretime > @nowtime AND id>@id ORDER BY id";
                    SqlCommand command = new SqlCommand(strQuery, connection);
                    command.Parameters.AddWithValue("@agentid",     request.AgentID);
                    command.Parameters.AddWithValue("@username",    request.UserID);
                    command.Parameters.AddWithValue("@id",          request.ID);
                    command.Parameters.AddWithValue("@nowtime",     DateTime.UtcNow);
                    using (DbDataReader reader = await command.ExecuteReaderAsync())
                    {
                        List<UserFreeSpinEventItem> userFreeSpinEventItems = new List<UserFreeSpinEventItem>();
                        while (await reader.ReadAsync())
                        {
                            long        eventBonusID    = (long)reader["id"];
                            int         freeSpinCount   = (int)reader["freespincount"];
                            int         betLevel        = (int)reader["betlevel"];
                            int         gameid          = (int)reader["gameid"];
                            DateTime    expireTime      = (DateTime)reader["expiretime"];
                            string      freeSpinID      = (string)reader["freespinid"];

                            UserFreeSpinEventItem userEventItem = new UserFreeSpinEventItem(eventBonusID, request.AgentID, request.UserID, gameid, freeSpinCount, betLevel, expireTime, freeSpinID);
                            userFreeSpinEventItems.Add(userEventItem);
                        }
                        if(userFreeSpinEventItems.Count > 0) 
                            Sender.Tell(userFreeSpinEventItems);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occured in DBProxyReader::fetchNewUserFreeSpinEvent {0}", ex);
            }
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
    }
}
