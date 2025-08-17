using Akka.Actor;
using Akka.Event;
using Akka.Routing;
using GITProtocol;
using Newtonsoft.Json;
using QueenApiNode.HttpService;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq.Expressions;
using System.Runtime.Remoting.Messaging;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace QueenApiNode.Database
{
    public class DBProxyReader : ReceiveActor
    {
        private readonly ILoggingAdapter    _logger         = Context.GetLogger();
        private string                      _strConnString  = "";

        public DBProxyReader(string strConnString)
        {
            _strConnString = strConnString;

            ReceiveAsync<DBCreateUserRequest>       (onCreateUserRequest);
            ReceiveAsync<DBGetUserBalanceRequest>   (onGetUserBalance);
            ReceiveAsync<DBGetUserInfoRequest>      (onGetUserInfoRequest);
            ReceiveAsync<DBUserGameLogRequest>      (onGetUserGameLogRequest);
            ReceiveAsync<DBUserCashFlowRequest>     (onGetUserCashFlowRequest);
            ReceiveAsync<DBGetAllUserBalanceRequest>(onGetAllUserBalanceRequest);
            ReceiveAsync<DBGetAgentInfoRequest>     (onGetAgentInfo);
            ReceiveAsync<DBGetAgentInfoByIDRequest> (onGetAgentInfoByID);
            ReceiveAsync<DBUserDepositRequest>      (onUserDepositRequest);
            ReceiveAsync<ApiWithdrawRequest>        (onUserApiWithdraw);
        }

        public static Props Props(string strConnString, int poolSize)
        {
            return Akka.Actor.Props.Create(() => new DBProxyReader(strConnString)).WithRouter(new RoundRobinPool(poolSize));
        }

        private async Task onUserApiWithdraw(ApiWithdrawRequest request)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(_strConnString))
                {
                    await connection.OpenAsync();
                    string strQuery = "UPDATE users SET balance=balance-@amount, lastscoreid=lastscoreid+1 OUTPUT inserted.balance as after, deleted.balance as before WHERE username=@username and balance >= @amount";
                    if (request.Amount < 0.0)
                        strQuery = "UPDATE users SET balance=0, lastscoreid=lastscoreid+1 OUTPUT inserted.balance as after, deleted.balance as before WHERE username=@username";
                    SqlCommand command = new SqlCommand(strQuery, connection);
                    command.Parameters.AddWithValue("@username", request.UserID);
                    if (request.Amount >= 0.0)
                        command.Parameters.AddWithValue("@amount", request.Amount);

                    using(var reader = await command.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            double afterscore   = (double)(Decimal)reader["after"];
                            double beforescore  = (double)(Decimal)reader["before"];
                            Sender.Tell(new ApiWithdrawResponse(0, Math.Round(beforescore, 2), Math.Round(afterscore, 2)));
                            return;
                        }
                        Sender.Tell(new ApiWithdrawResponse(1, 0.0, 0.0));
                        return;
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occured in DBProxyReader::onUserWithdraw {0}", ex);
            }
            Sender.Tell(new ApiWithdrawResponse(2, 0.0, 0.0));
        }

        private async Task onGetAgentInfo(DBGetAgentInfoRequest request)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(_strConnString))
                {
                    await connection.OpenAsync();

                    string strQuery     = "SELECT id, username, score, authtoken, state, currency, language FROM agents WHERE username=@username";
                    SqlCommand command  = new SqlCommand(strQuery, connection);
                    command.Parameters.AddWithValue("@username", request.AgentID);

                    using(DbDataReader reader = await command.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            if (reader["authtoken"] is DBNull || (int)reader["state"] == 0)
                            {
                                Sender.Tell(new DBAgentInfoResponse(0, null, null, 0.0, 0, 0));
                            }
                            else
                            {
                                DBAgentInfoResponse response = new DBAgentInfoResponse((int)reader["id"], (string)reader["username"], (string)reader["authtoken"], (double)(Decimal)reader["score"], (int)reader["currency"], (int)reader["language"]);
                                Sender.Tell(response);
                            }
                        }
                        else
                            Sender.Tell(new DBAgentInfoResponse(0, null, null, 0.0, 0, 0));
                    }
                    return;
                }
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occured in DBProxyReader::onGetWebsiteInfo {0}", ex);
            }
            Sender.Tell(null);
        }

        private async Task onGetAgentInfoByID(DBGetAgentInfoByIDRequest request)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(_strConnString))
                {
                    await connection.OpenAsync();
                    string      strQuery    = "SELECT id, username, score, authtoken, state, currency, language FROM agents WHERE id=@id";
                    SqlCommand  command     = new SqlCommand(strQuery, connection);
                    command.Parameters.AddWithValue("@id", request.AgentID);

                    using (DbDataReader reader = await command.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            if (reader["authtoken"] is DBNull || (int)reader["state"] == 0)
                            {
                                Sender.Tell(new DBAgentInfoResponse(0, null, null, 0.0, 0, 0));
                            }
                            else
                            {
                                DBAgentInfoResponse response = new DBAgentInfoResponse((int)reader["id"], (string)reader["username"], (string)reader["authtoken"], (double)(Decimal)reader["score"], (int)reader["currency"], (int)reader["language"]);
                                Sender.Tell(response);
                            }
                        }
                        else
                            Sender.Tell(new DBAgentInfoResponse(0, null, null, 0.0, 0, 0));
                    }
                    return;
                }
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occured in DBProxyReader::onGetWebsiteInfo {0}", ex);
            }
            Sender.Tell(null);
        }

        private async Task onUserDepositRequest(DBUserDepositRequest request)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(_strConnString))
                {
                    await connection.OpenAsync();
                    string strQuery     = "UPDATE users SET balance=balance+@amount, lastscorecounter=lastscorecounter+1 OUTPUT inserted.balance as afterscore, deleted.balance as beforescore, inserted.lastscorecounter  WHERE agentid=@agentid and username=@username";
                    SqlCommand command  = new SqlCommand(strQuery, connection);
                    command.Parameters.AddWithValue("@agentid",     request.AgentID);
                    command.Parameters.AddWithValue("@username",    request.UserID);
                    command.Parameters.AddWithValue("@amount",      request.Amount);

                    using(DbDataReader reader = await command.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            double  afterscore          = (double)(Decimal)reader["afterscore"];
                            double  beforescore         = (double)(Decimal)reader["beforescore"];
                            long    lastscorecounter    = (long)reader["lastscorecounter"];
                            Sender.Tell(new DBUserDepositResponse(true, beforescore, afterscore, lastscorecounter));
                        }
                        else
                            Sender.Tell(new DBUserDepositResponse(false, 0.0, 0.0, 0L));
                    }
                    return;
                }
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occured in DBProxyReader::onUserDepositRequest {0}", ex);
            }
            Sender.Tell(new DBUserDepositResponse(false, 0.0, 0.0, 0L));
        }

        private string getMD5Hash(string input)
        {
            try
            {
                byte[] hash = MD5.Create().ComputeHash(Encoding.UTF8.GetBytes(input));
                StringBuilder stringBuilder = new StringBuilder();
                for (int i = 0; i < hash.Length; ++i)
                    stringBuilder.Append(hash[i].ToString("x2"));

                return stringBuilder.ToString();
            }
            catch
            {
                return string.Empty;
            }
        }

        private async Task onCreateUserRequest(DBCreateUserRequest request)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(this._strConnString))
                {
                    await connection.OpenAsync();

                    string      strQuery    = "INSERT INTO users (agentid, username, password, agentname, balance, currency, updatetime, registertime) VALUES (@agentid, @username, @password, @agentname, @balance, @currency, @updatetime, @registertime)";
                    SqlCommand  command     = new SqlCommand(strQuery, connection);
                    command.Parameters.AddWithValue("@agentid",     request.AgentDBID);
                    command.Parameters.AddWithValue("@username",    request.UserName);
                    command.Parameters.AddWithValue("@password",    getMD5Hash(request.Password));
                    command.Parameters.AddWithValue("@agentname",   request.AgentID);
                    command.Parameters.AddWithValue("@balance",     0.0);
                    command.Parameters.AddWithValue("@currency",    (int) request.Currency);
                    command.Parameters.AddWithValue("@updatetime",  DateTime.Now);
                    command.Parameters.AddWithValue("@registertime",DateTime.Now);
                    await command.ExecuteNonQueryAsync();
                    Sender.Tell(true);
                    return;
                }
            }
            catch (Exception ex)
            {
            }
            Sender.Tell(false);
        }

        private async Task onGetUserBalance(DBGetUserBalanceRequest request)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(_strConnString))
                {
                    await connection.OpenAsync();
                    string      strQuery    = "SELECT balance FROM users WHERE agentid=@agentid and username=@username";
                    SqlCommand  command     = new SqlCommand(strQuery, connection);
                    command.Parameters.AddWithValue("@agentid",     request.AgentID);
                    command.Parameters.AddWithValue("@username",    request.UserName);

                    using(DbDataReader reader = await command.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            double userBalance = (double)(Decimal)reader["balance"];
                            Sender.Tell(userBalance);
                        }
                        else
                            Sender.Tell(double.NegativeInfinity);
                    }
                    return;
                }
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occured in DBProxyReader::onGetUserBalance {0}", ex);
            }
            Sender.Tell(double.NegativeInfinity);
        }

        private async Task onGetAllUserBalanceRequest(DBGetAllUserBalanceRequest request)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(_strConnString))
                {
                    await connection.OpenAsync();
                    string      strQuery    = "SELECT username, balance FROM users WHERE agentid=@agentid";
                    SqlCommand  command     = new SqlCommand(strQuery, connection);
                    command.Parameters.AddWithValue("@agentid", request.AgentDBID);

                    List<UserBalanceItem> items = new List<UserBalanceItem>();
                    using (DbDataReader reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            UserBalanceItem item = new UserBalanceItem();
                            item.userid     = (string)reader["username"];
                            item.balance    = Math.Round((double)(Decimal)reader["balance"], 2);
                            items.Add(item);
                        }
                    }

                    Sender.Tell(JsonConvert.SerializeObject(new AllUserBalanceResponse(items)));
                    return;
                }
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occured in DBProxyReader::onGetAllUserBalanceRequest {0}", ex);
            }
            Sender.Tell(JsonConvert.SerializeObject(new QueenResponse(ResponseCodes.GeneralError, "general error")));
        }

        private async Task onGetUserInfoRequest(DBGetUserInfoRequest request)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(this._strConnString))
                {
                    await connection.OpenAsync();
                    string      strQuery    = "SELECT username, password, state FROM users WHERE agentid=@agentid and username=@username";
                    SqlCommand  command     = new SqlCommand(strQuery, connection);
                    command.Parameters.AddWithValue("@agentid",     request.AgentID);
                    command.Parameters.AddWithValue("@username",    request.UserName);

                    using (DbDataReader reader = await command.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            Sender.Tell(new DBGetUserInfoResponse((string)reader["username"], (string)reader["password"], (int)reader["state"] == 1));
                            return;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occured in DBProxyReader::onGetUserInfoRequest {0}", ex);
            }
            Sender.Tell(null);
        }

        private async Task onGetUserCashFlowRequest(DBUserCashFlowRequest request)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(this._strConnString))
                {
                    await connection.OpenAsync();
                    string strQuery = "SELECT COUNT(id) as count FROM userscorelogs WHERE agentname=@agentname and username=@username and updatetime > @begintime and updatetime <= @endtime";
                    if (request.EndTime == new DateTime(1, 1, 1))
                        strQuery = "SELECT COUNT(id) as count FROM userscorelogs WHERE agentname=@agentname and username=@username and updatetime > @begintime";
                    SqlCommand command = new SqlCommand(strQuery, connection);
                    command.Parameters.AddWithValue("@agentname",   request.AgentID);
                    command.Parameters.AddWithValue("@username",    request.UserID);
                    command.Parameters.AddWithValue("@begintime",   request.BeginTime);
                    if (request.EndTime != new DateTime(1, 1, 1))
                        command.Parameters.AddWithValue("@endtime", request.EndTime);

                    int totalCount = 0;
                    using(DbDataReader reader = await command.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                            totalCount = (int)reader["count"];
                    }

                    List<UserCashFlowItem> items = new List<UserCashFlowItem>();
                    if (totalCount > 0)
                    {
                        strQuery = string.Format("SELECT TOP {0} id, changemoney, beforemoney, aftermoney, mode, updatetime FROM userscorelogs WHERE agentname=@agentname and username=@username and updatetime > @begintime and updatetime <= @endtime", (object) request.Count);
                        if (request.EndTime == new DateTime(1, 1, 1))
                            strQuery = string.Format("SELECT TOP {0} id, changemoney, beforemoney, aftermoney, mode, updatetime FROM userscorelogs WHERE agentname=@agentname and username=@username and updatetime > @begintime", (object) request.Count);
                        command = new SqlCommand(strQuery, connection);
                        command.Parameters.AddWithValue("@agentname",   request.AgentID);
                        command.Parameters.AddWithValue("@username",    request.UserID);
                        command.Parameters.AddWithValue("@begintime",   request.BeginTime);
                        if (request.EndTime != new DateTime(1, 1, 1))
                            command.Parameters.AddWithValue("@endtime", request.EndTime);

                        using (DbDataReader reader = await command.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                UserCashFlowItem item = new UserCashFlowItem();
                                item.id         = (long)reader["id"];
                                item.amount     = Math.Round((double)(Decimal)reader["changemoney"], 2);
                                item.beginmoney = Math.Round((double)(Decimal)reader["beforemoney"], 2);
                                item.endmoney   = Math.Round((double)(Decimal)reader["aftermoney"], 2);
                                int mode        = (int)reader["mode"];
                                item.type       = mode != 0 ? "Withdraw" : "Deposit";
                                item.timestamp  = ((DateTime)reader["updatetime"]).ToString("yyyy-MM-dd HH:mm:ss");
                                items.Add(item);
                            }
                        }
                    }

                    UserCashFlowResponse response = new UserCashFlowResponse(totalCount, items);
                    Sender.Tell(JsonConvert.SerializeObject(response));
                }
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occured in DBProxyReader::onGetUserCashFlowRequest {0}", ex);
            }
        }

        private async Task onGetUserGameLogRequest(DBUserGameLogRequest request)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(this._strConnString))
                {
                    await connection.OpenAsync();
                    string strGameLogTableName  = string.Format("gamelog_{0}", request.AgentDBID);
                    string strQuery     = "IF EXISTS(SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME='" + strGameLogTableName + "') SELECT 1 ELSE SELECT 0";
                    SqlCommand command  = new SqlCommand(strQuery, connection);
                    int checkResult = Convert.ToInt32(await command.ExecuteScalarAsync());
                    long totalCount = 0;
                    
                    if (checkResult == 1)
                    {
                        strQuery    = string.Format("SELECT COUNT(id) as count FROM {0} WHERE id > @startid", strGameLogTableName);
                        command     = new SqlCommand(strQuery, connection);
                        command.Parameters.AddWithValue("@startid", request.StartID);

                        using (DbDataReader reader = await command.ExecuteReaderAsync())
                        {
                            if (await reader.ReadAsync())
                                totalCount = (int)reader["count"];
                        }
                    }

                    List<BetHistoryItem> items = new List<BetHistoryItem>();
                    long lastObjectId = request.StartID;
                    if (totalCount > 0L)
                    {
                        strQuery    = string.Format("SELECT TOP {1} id, username, gameid, bet, win, logtime FROM {0} WHERE id > @startid ORDER BY id", strGameLogTableName, request.Count);
                        command     = new SqlCommand(strQuery, connection);
                        command.Parameters.AddWithValue("@startid", request.StartID);

                        using (DbDataReader reader = await command.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                BetHistoryItem item = new BetHistoryItem();
                                item.objectID           = (long)reader["id"];
                                item.userid             = (string)reader["username"];
                                item.gameid             = (int)reader["gameid"];
                                item.betAmount          = Math.Round((double)(Decimal)reader["bet"], 2);
                                item.winAmount          = Math.Round((double)(Decimal)reader["win"], 2);
                                item.transactionTime    = ((DateTime)reader["logtime"]).ToString("yyyy-MM-dd HH:mm:ss");
                                items.Add(item);
                            }
                            
                            if (items.Count > 0)
                                lastObjectId = items[items.Count - 1].objectID;
                        }
                    }
                    Sender.Tell(JsonConvert.SerializeObject(new GetBetHistoryResponse(totalCount, lastObjectId, items)));
                    return;
                }
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occured in DBProxyReader::onGetUserGameLogRequest {0}", ex);
            }
            Sender.Tell(JsonConvert.SerializeObject(new QueenResponse(ResponseCodes.GeneralError, "general error")));
        }
    }

    public class DBCreateUserRequest
    {
        public string       UserName    { get; private set; }
        public string       Password    { get; private set; }
        public int          AgentDBID   { get; private set; }
        public string       AgentID     { get; private set; }
        public Currencies   Currency    { get; private set; }
        public Languages    Language    { get; private set; }

        public DBCreateUserRequest(string strUserName,string strPassword,int agentDBID,string strAgentID,Currencies currency,Languages langugage)
        {
            UserName    = strUserName;
            Password    = strPassword;
            AgentDBID   = agentDBID;
            AgentID     = strAgentID;
            Currency    = currency;
            Language    = langugage;
        }
    }
    public class DBGetUserBalanceRequest
    {
        public int      AgentID     { get; private set; }
        public string   UserName    { get; private set; }

        public DBGetUserBalanceRequest(int agentID, string strUserName)
        {
            AgentID     = agentID;
            UserName    = strUserName;
        }
    }
    public class DBGetUserInfoRequest
    {
        public int      AgentID     { get; private set; }
        public string   UserName    { get; private set; }

        public DBGetUserInfoRequest(int agentID, string strUserName)
        {
            AgentID     = agentID;
            UserName    = strUserName;
        }
    }
    public class DBGetUserInfoResponse
    {
        public string   UserName    { get; private set; }
        public string   Password    { get; private set; }
        public bool     IsEnabled   { get; private set; }

        public DBGetUserInfoResponse(string strUserName, string strPassword, bool isEnabeld)
        {
            UserName    = strUserName;
            Password    = strPassword;
            IsEnabled   = isEnabeld;
        }
    }
    public class DBUserGameLogRequest
    {
        public int  AgentDBID   { get; private set; }
        public long StartID     { get; private set; }
        public int  Count       { get; private set; }

        public DBUserGameLogRequest(int agentID, long startID, int count)
        {
            AgentDBID   = agentID;
            StartID     = startID;
            Count       = count;
        }
    }
    public class DBUserCashFlowRequest
    {
        public string   AgentID     { get; private set; }
        public string   UserID      { get; private set; }
        public DateTime BeginTime   { get; private set; }
        public DateTime EndTime     { get; private set; }
        public int      Count       { get; private set; }

        public DBUserCashFlowRequest(string agentID,string strUserID,DateTime beginTime,DateTime endTime,int count)
        {
            AgentID     = agentID;
            UserID      = strUserID;
            BeginTime   = beginTime;
            EndTime     = endTime;
            Count       = count;
        }
    }
    public class DBGetAllUserBalanceRequest
    {
        public int AgentDBID { get; private set; }
        public DBGetAllUserBalanceRequest(int agentDBID)
        {
            AgentDBID = agentDBID;
        }
    }
    public class ApiProvider
    {
        public string vendorid  { get; set; }
        public string name      { get; set; }
        public string category  { get; set; }
    }
    public class ApiGame
    {
        public string   vendorid    { get; set; }
        public int      gameid      { get; set; }
        public string   name        { get; set; }
        public string   symbol      { get; set; }
        public string   iconurl1    { get; set; }
        public string   iconurl2    { get; set; }
    }
    public class DBGetAgentInfoRequest
    {
        public string AgentID { get; private set; }
        public DBGetAgentInfoRequest(string strAgentID)
        {
            AgentID = strAgentID;
        }
    }
    public class DBGetAgentInfoByIDRequest
    {
        public int AgentID { get; private set; }
        public DBGetAgentInfoByIDRequest(int agentID)
        {
            AgentID = agentID;
        }
    }
    public class DBAgentInfoResponse
    {
        public string   AgentID     { get; private set; }
        public int      AgentDBID   { get; private set; }
        public string   AuthToken   { get; private set; }
        public double   Balance     { get; private set; }
        public int      Currency    { get; private set; }
        public int      Language    { get; private set; }

        public DBAgentInfoResponse(int dbID,string strAgentID,string authToken,double balance,int currency,int language)
        {
            AgentDBID   = dbID;
            AgentID     = strAgentID;
            AuthToken   = authToken;
            Balance     = balance;
            Currency    = currency;
            Language    = language;
        }
    }
    public class DBUserDepositRequest
    {
        public int      AgentID { get; private set; }
        public string   UserID  { get; private set; }
        public double   Amount  { get; private set; }

        public DBUserDepositRequest(int websiteDBID, string strUserID, double amount)
        {
            AgentID = websiteDBID;
            UserID  = strUserID;
            Amount  = amount;
        }
    }
    public class DBUserDepositResponse
    {
        public bool     IsSuccess           { get; private set; }
        public double   Before              { get; private set; }
        public double   After               { get; private set; }
        public long     LastScoreCounter    { get; private set; }

        public DBUserDepositResponse(bool isSuccess,double before,double after,long lastScoreCounter)
        {
            IsSuccess           = isSuccess;
            Before              = before;
            After               = after;
            LastScoreCounter    = lastScoreCounter;
        }
    }

}
