using Akka.Actor;
using Akka.Event;
using Akka.Routing;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using System.Security.Cryptography;
using GITProtocol;
using System.Reflection;
using QueenApiNode.HttpService;
using Newtonsoft.Json;

namespace QueenApiNode.Database
{
    public class DBProxyReader : ReceiveActor
    {
        private readonly ILoggingAdapter _logger = Logging.GetLogger(Context);
        private string _strConnString = "";
        public DBProxyReader(string strConnString)
        {
            _strConnString = strConnString;
            ReceiveAsync<GetGameListRequest>        (onGetGameList);
            ReceiveAsync<DBCreateUserRequest>       (onCreateUserRequest);
            ReceiveAsync<DBGetUserBalanceRequest>   (onGetUserBalance);
            ReceiveAsync<DBGetUserInfoRequest>      (onGetUserInfoRequest);
            ReceiveAsync<DBUserGameLogRequest>      (onGetUserGameLogRequest);
            ReceiveAsync<DBGetAgentInfoRequest>     (onGetAgentInfo);
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
                    using (DbDataReader reader = await command.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            double afterscore = (double)(decimal)reader["after"];
                            double beforescore = (double)(decimal)reader["before"];
                            Sender.Tell(new ApiWithdrawResponse(0, beforescore, afterscore));
                        }
                        else
                        {
                            Sender.Tell(new ApiWithdrawResponse(1, 0.0, 0.0));
                        }
                    }
                    return;
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
                    string strQuery = "SELECT id, username, score, authtoken, whitelist, state, moneymode, currency FROM agents WHERE username=@username";
                    SqlCommand command = new SqlCommand(strQuery, connection);
                    command.Parameters.AddWithValue("@username", request.AgentID);
                    using (DbDataReader reader = await command.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            if (reader["authtoken"] is DBNull || (int) reader["state"] == 0)
                            {
                                Sender.Tell(new DBAgentInfoResponse(0, null, null, null,  0.0, 0, 0));
                            }
                            else
                            {
                                DBAgentInfoResponse response = new DBAgentInfoResponse((int)reader["id"], (string)reader["username"], (string)reader["authtoken"], (string)reader["whitelist"], (double)(decimal)reader["score"], (int)reader["moneymode"], (int)reader["currency"]);
                                Sender.Tell(response);
                            }
                        }
                        else
                        {
                            Sender.Tell(new DBAgentInfoResponse(0, null, null, null,  0.0, 0, 0));
                        }
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
                    string strQuery = "UPDATE users SET balance=balance+@amount, lastscoreid=lastscoreid+1 OUTPUT inserted.balance as afterscore, deleted.balance as beforescore, inserted.lastscoreid  WHERE username=@username and agentid=@agentid";
                    SqlCommand command = new SqlCommand(strQuery, connection);
                    command.Parameters.AddWithValue("@username",    request.UserID);
                    command.Parameters.AddWithValue("@amount",      request.Amount);
                    command.Parameters.AddWithValue("@agentid",     request.AgentID);

                    using (DbDataReader reader = await command.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {

                            double afterscore          = (double)(decimal)reader["afterscore"];
                            double beforescore         = (double)(decimal)reader["beforescore"];
                            long   lastscorecounter    = (long) reader["lastscoreid"];
                            Sender.Tell(new DBUserDepositResponse(true, beforescore, afterscore, lastscorecounter));
                        }
                        else
                        {
                            Sender.Tell(new DBUserDepositResponse(false, 0.0, 0.0, 0));
                        }
                    }
                    return;
                }
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occured in DBProxyReader::onUserDepositRequest {0}", ex);
            }
            Sender.Tell(new DBUserDepositResponse(false, 0.0, 0.0, 0));
        }

        private string getMD5Hash(string input)
        {
            try
            {
                MD5 md5Hash = MD5.Create();
                byte[] data = md5Hash.ComputeHash(Encoding.UTF8.GetBytes(input));
                StringBuilder sBuilder = new StringBuilder();
                for (int i = 0; i < data.Length; i++)
                {
                    sBuilder.Append(data[i].ToString("x2"));
                }
                return sBuilder.ToString();
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
                using (SqlConnection connection = new SqlConnection(_strConnString))
                {
                    await connection.OpenAsync();
                    string strQuery = "INSERT INTO users (username,password,agentid,agentname, updatetime, registertime) VALUES " +
                        "(@username,@password,@agentid,@agentname, @updatetime, @registertime)";
                    SqlCommand command = new SqlCommand(strQuery, connection);
                    command.Parameters.AddWithValue("@username",    request.UserName);
                    command.Parameters.AddWithValue("@password",    getMD5Hash(request.Password));
                    command.Parameters.AddWithValue("@agentname",   request.AgentID);
                    command.Parameters.AddWithValue("@agentid",     request.AgentDBID);
                    command.Parameters.AddWithValue("@updatetime",   DateTime.Now);
                    command.Parameters.AddWithValue("@registertime", DateTime.Now);

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
                    await       connection.OpenAsync();
                    string      strQuery    = "SELECT balance FROM users WHERE agentid=@agentid and username=@username";
                    SqlCommand  command     = new SqlCommand(strQuery, connection);
                    command.Parameters.AddWithValue("@username", request.UserName);
                    command.Parameters.AddWithValue("@agentid",  request.AgentID);
                    using (DbDataReader reader = await command.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            double userBalance = (double)(decimal)reader["balance"];
                            Sender.Tell(userBalance);
                        }
                        else
                        {
                            Sender.Tell(double.NegativeInfinity);
                        }
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
        private async Task onGetUserInfoRequest(DBGetUserInfoRequest request)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(_strConnString))
                {
                    await connection.OpenAsync();
                    string strQuery = "SELECT username, password, state FROM users WHERE username=@username and agentid=@agentid";
                    SqlCommand command = new SqlCommand(strQuery, connection);
                    command.Parameters.AddWithValue("@username", request.UserName);
                    command.Parameters.AddWithValue("@agentid",  request.AgentID);
                    using (DbDataReader reader = await command.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            Sender.Tell(new DBGetUserInfoResponse((string)reader["username"], (string)reader["password"],
                                ((int)reader["state"] == 1)));
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
        private async Task onGetUserGameLogRequest(DBUserGameLogRequest request)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(_strConnString))
                {
                    await connection.OpenAsync();
                    string strGameLogTableName = string.Format("gamelog_{0}", request.AgentDBID);
                    string strQuery = @"IF EXISTS(SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME='" + strGameLogTableName + "') SELECT 1 ELSE SELECT 0";



                    SqlCommand command  = new SqlCommand(strQuery, connection);
                    int  checkResult     = Convert.ToInt32(await command.ExecuteScalarAsync());
                    long totalCount      = 0;

                    if (checkResult == 1)
                    {
                        strQuery = string.Format("SELECT COUNT(id) as count FROM {0} WHERE id > @startid", strGameLogTableName);
                        command  = new SqlCommand(strQuery, connection);
                        command.Parameters.AddWithValue("@startid", request.StartID);
                        using (DbDataReader reader = await command.ExecuteReaderAsync())
                        {
                            if (await reader.ReadAsync())
                                totalCount = (int)reader["count"];
                        }
                    }

                    List<BetHistoryItem> items  = new List<BetHistoryItem>();
                    long lastObjectId           = request.StartID;
                    if (totalCount > 0)
                    {
                        strQuery = string.Format("SELECT TOP {1} id, username, gameid, tableid, bet, win, gamelog, logtime FROM {0} WHERE id > @startid ORDER BY id", 
                            strGameLogTableName, request.Count);
                        if (!String.IsNullOrEmpty( request.StartDate))
                        {
                            strQuery = $"SELECT TOP {request.Count} id, username, gameid, tableid, bet, win, gamelog, logtime FROM {strGameLogTableName} WHERE id > @startid AND logtime >= '{request.StartDate}' ORDER BY id";
                        }
                        if (!String.IsNullOrEmpty(request.EndDate))
                        {
                            strQuery = $"SELECT TOP {request.Count} id, username, gameid, tableid, bet, win, gamelog, logtime FROM {strGameLogTableName} WHERE id > @startid AND logtime <= '{request.EndDate}' ORDER BY id";
                        }
                        if (!String.IsNullOrEmpty(request.StartDate) && !String.IsNullOrEmpty(request.EndDate))
                        {
                            strQuery = $"SELECT TOP {request.Count} id, username, gameid, tableid, bet, win, gamelog, logtime FROM {strGameLogTableName} " +
                                $"WHERE id > @startid AND logtime >= '{request.StartDate}' " +
                                $"AND logtime <= '{request.EndDate}' ORDER BY id";
                            
                        }

                        command = new SqlCommand(strQuery, connection);
                        command.Parameters.AddWithValue("@startid", request.StartID);

                        using (DbDataReader reader = await command.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                BetHistoryItem item = new BetHistoryItem();
                                item.objectID           = (long) reader["id"];
                                item.userid             = (string)reader["username"];
                                item.gameid             = ((int)reader["gameid"]).ToString();
                                item.betAmount          = Math.Round((double)(decimal)reader["bet"], 2);
                                item.winAmount          = Math.Round((double)(decimal)reader["win"], 2);
                                string tabelId = (string)reader["tableid"];
                                item.roundid            = tabelId.Split(new string[] { "::" }, StringSplitOptions.None)[0];
                                string gamelog = (string)reader["gamelog"];
                                item.isFinished = String.IsNullOrEmpty(gamelog) ? false : true;
                                if (item.isFinished)
                                {
                                    item.isBuyFeature = gamelog.Contains("&pur=0");
                                }
                              
                                item.transactionTime    = ((DateTime)reader["logtime"]).ToString("yyyy-MM-dd HH:mm:ss"); //From UTC To UTC + 9
                                items.Add(item);
                            }
                        }
                        if (items.Count > 0)
                            lastObjectId = items[items.Count - 1].objectID;
                    }
                    Sender.Tell(JsonConvert.SerializeObject(new GetBetHistoryResponse(totalCount, lastObjectId, items)));
                }
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occured in DBProxyReader::onGetUserGameLogRequest {0}", ex);
            }
            Sender.Tell(JsonConvert.SerializeObject(new QueenResponse(ResponseCodes.GeneralError, "general error")));

        }
        private async Task onGetGameList(GetGameListRequest request)
        {
            List<ApiGame> gameList = new List<ApiGame>();
            try
            {
                using (SqlConnection connection = new SqlConnection(_strConnString))
                {
                    await connection.OpenAsync();

                    string  strQuery = "SELECT * FROM gameconfigs WHERE openclose = 1";
                    SqlCommand command = new SqlCommand(strQuery, connection);
                    using (DbDataReader reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            ApiGame apiGame         = new ApiGame();
                            apiGame.title           = (string)reader["title"];
                            apiGame.thumbnail       = string.Format("https://{1}.{0}.com/slots/cover-img/" + (string)reader["coverimg"], ApiConfig.Domain, ApiConfig.SubClientB);
                            apiGame.thumbnail       = apiGame.thumbnail.Replace("\r", "").Replace("\n", "");
                            apiGame.name            = (string)reader["gamename"];
                            apiGame.gameid          = ((int)reader["gameid"]).ToString();
                            apiGame.weekturnover    = (double)(decimal)reader["weekturnover"];
                            apiGame.monthturnover   = (double)(decimal)reader["monthturnover"];
                            apiGame.releasedate     = (DateTime)reader["releasedate"];
                            apiGame.provider        = ApiConfig.Domain.ToUpper();

                            gameList.Add(apiGame);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occured in DBProxyReader::onGetGameList {0}", ex);
            }
            Sender.Tell(gameList);
        }
    }

    public class DBCreateUserRequest
    {
        public string   UserName    { get; private set; }
        public string   Password    { get; private set; }
        public int      AgentDBID   { get; private set; }
        public string   AgentID     { get; private set; }

        public DBCreateUserRequest(string strUserName, string strPassword, int agentDBID, string strAgentID)
        {
            this.UserName   = strUserName;
            this.Password   = strPassword;
            this.AgentDBID  = agentDBID;
            this.AgentID    = strAgentID;
        }
    }
    public class DBGetUserBalanceRequest
    {
        public int      AgentID  { get; private set; }
        public string   UserName { get; private set; }
        public DBGetUserBalanceRequest(int agentID, string strUserName)
        {
            this.AgentID  = agentID;
            this.UserName = strUserName;
        }
    }
    public class DBGetUserInfoRequest
    {
        public int      AgentID     { get; private set; }
        public string   UserName    { get; private set; }
        public DBGetUserInfoRequest(int agentID, string strUserName)
        {
            this.AgentID = agentID;
            this.UserName = strUserName;
        }
    }
    public class DBGetUserInfoResponse
    {
        public string   UserName    { get; private set; }
        public string   Password    { get; private set; }
        public bool     IsEnabled   { get; private set; }
        public DBGetUserInfoResponse(string strUserName, string strPassword, bool isEnabeld)
        {
            this.UserName = strUserName;
            this.Password = strPassword;
            this.IsEnabled = isEnabeld;
        }
    }
    public class DBUserGameLogRequest
    {
        public int      AgentDBID   { get; private set; }
        public long     StartID     { get; private set; }
        public int      Count       { get; private set; }

        public string      StartDate   { get; private set; }
        public string      EndDate     { get; private set; }

        public DBUserGameLogRequest(int agentID, long startID, int count, string startDate, string EndDate)
        {
            this.AgentDBID  = agentID;
            this.StartID    = startID;
            this.Count      = count;
            this.StartDate = startDate;
            this.EndDate = EndDate;
        }
    }
    public class ApiProvider
    {
        public string vendorid { get; set; }
        public string name { get; set; }
        public string category { get; set; }
    }
    public class ApiGame
    {
        public string   title           { get; set; }
        public string   thumbnail       { get; set; }
        public string   provider        { get; set; }
        public string   gameid          { get; set; }
        public string   name            { get; set; }
        public DateTime releasedate     { get; set; }
        public double   weekturnover    { get; set; }
        public double   monthturnover   { get; set; }
    }
        
    public class DBGetAgentInfoRequest
    {
        public string AgentID { get; private set; }

        public DBGetAgentInfoRequest(string strAgentID)
        {
            this.AgentID = strAgentID;
        }
    }
    public class DBAgentInfoResponse
    {
        public string   AgentID     { get; private set; }
        public int      AgentDBID   { get; private set; }
        public string   AuthToken   { get; private set; }
        public string   WhiteList   { get; private set; }
        public double   Balance     { get; private set; }
        public int      MoneyMode   { get; private set; }
        public int      Currency    { get; private set; }
        public DBAgentInfoResponse(int dbID, string strAgentID, string authToken, string whiteList, double balance, int moneyMode, int currency)
        {
            this.AgentDBID  = dbID;
            this.AgentID    = strAgentID;
            this.AuthToken  = authToken;
            this.WhiteList = whiteList;
            this.Balance    = balance;
            this.MoneyMode  = moneyMode;
            this.Currency = currency;
        }
    }

    public class DBUserDepositRequest
    {
        public int AgentID { get; private set; }
        public string UserID { get; private set; }
        public double Amount { get; private set; }

        public DBUserDepositRequest(int websiteDBID, string strUserID, double amount)
        {
            this.AgentID = websiteDBID;
            this.UserID = strUserID;
            this.Amount = amount;
        }
    }
    public class DBUserDepositResponse
    {
        public bool IsSuccess { get; private set; }
        public double Before { get; private set; }
        public double After { get; private set; }
        public long   LastScoreCounter { get; private set; }
        public DBUserDepositResponse(bool isSuccess, double before, double after, long lastScoreCounter)
        {
            this.IsSuccess = isSuccess;
            this.Before = before;
            this.After = after;
            this.LastScoreCounter = lastScoreCounter;
        }

    }


}
