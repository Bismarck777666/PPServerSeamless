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
            ReceiveAsync<DBCreateUserRequest>       (onCreateUserRequest);
            ReceiveAsync<DBGetUserInfoRequest>      (onGetUserInfoRequest);
            ReceiveAsync<DBGetAgentInfoRequest>     (onGetAgentInfo);
            ReceiveAsync<DBGetAgentInfoByIDRequest> (onGetAgentInfoByID);
            ReceiveAsync<DBCreateFreeGameRequest>   (onCreateFreeGame);
            ReceiveAsync<DBCancelFreeGameRequest>   (onCancelFreeGame);
        }
        public static Props Props(string strConnString, int poolSize)
        {
            return Akka.Actor.Props.Create(() => new DBProxyReader(strConnString)).WithRouter(new RoundRobinPool(poolSize));
        }
        private async Task onGetAgentInfo(DBGetAgentInfoRequest request)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(_strConnString))
                {
                    await connection.OpenAsync();
                    string strQuery = "SELECT id, username, apitoken, state, currency, language FROM agents WHERE username=@username";
                    SqlCommand command = new SqlCommand(strQuery, connection);
                    command.Parameters.AddWithValue("@username", request.AgentID);
                    using (DbDataReader reader = await command.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            if (reader["apitoken"] is DBNull || (int) reader["state"] == 0)
                            {
                                Sender.Tell(new DBAgentInfoResponse(0, null, null, 0, 0, false));
                            }
                            else
                            {
                                DBAgentInfoResponse response = new DBAgentInfoResponse((int)reader["id"], (string)reader["username"],
                                    (string)reader["apitoken"], (int)reader["currency"], (int)reader["language"], true);
                                Sender.Tell(response);
                            }
                        }
                        else
                        {
                            Sender.Tell(new DBAgentInfoResponse(0, null, null, 0, 0, false));
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
        private async Task onGetAgentInfoByID(DBGetAgentInfoByIDRequest request)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(_strConnString))
                {
                    await connection.OpenAsync();
                    string strQuery = "SELECT id, username, apitoken, state, currency, language FROM agents WHERE id=@id";
                    SqlCommand command = new SqlCommand(strQuery, connection);
                    command.Parameters.AddWithValue("@id", request.AgentID);
                    using (DbDataReader reader = await command.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            if (reader["apitoken"] is DBNull || (int)reader["state"] == 0)
                            {
                                Sender.Tell(new DBAgentInfoResponse(0, null, null, 0, 0, false));
                            }
                            else
                            {
                                DBAgentInfoResponse response = new DBAgentInfoResponse((int)reader["id"], (string)reader["username"],
                                    (string)reader["apitoken"], (int)reader["currency"], (int)reader["language"], true);
                                Sender.Tell(response);
                            }
                        }
                        else
                        {
                            Sender.Tell(new DBAgentInfoResponse(0, null, null, 0, 0, false));
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
        private async Task onCreateFreeGame(DBCreateFreeGameRequest request)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(_strConnString))
                {
                    await connection.OpenAsync();
                    string strQuery = "SELECT id FROM userfreespinevents WHERE agentid=@agentid AND username=@username AND freespinid=@freespinid";
                    SqlCommand command = new SqlCommand(strQuery, connection);
                    command.Parameters.AddWithValue("@agentid",     request.AgentID);
                    command.Parameters.AddWithValue("@username",    request.UserName);
                    command.Parameters.AddWithValue("@freespinid",  request.FreeSpinID);
                    
                    using (DbDataReader reader = await command.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            Sender.Tell(1);
                            return;
                        }
                    }

                    strQuery = "INSERT INTO userfreespinevents (agentid, username, gameid, freespincount, betlevel, freespinid, expiretime, regdate) VALUES " +
                        "(@agentid, @username, @gameid, @freespincount, @betlevel, @freespinid, @expiretime, @regdate)";
                    command = new SqlCommand(strQuery, connection);
                    command.Parameters.AddWithValue("@agentid",         request.AgentID);
                    command.Parameters.AddWithValue("@username",        request.UserName);
                    command.Parameters.AddWithValue("@gameid",          request.GameID);
                    command.Parameters.AddWithValue("@freespincount",   request.FreeSpinCount);
                    command.Parameters.AddWithValue("@betlevel",        request.BetLevel);
                    command.Parameters.AddWithValue("@expiretime",      request.ExpireTime);
                    command.Parameters.AddWithValue("@regdate",         DateTime.UtcNow);
                    command.Parameters.AddWithValue("@freespinid",      request.FreeSpinID);
                    await command.ExecuteNonQueryAsync();
                    Sender.Tell(0);
                    return;
                }
            }
            catch (Exception)
            {
            }
            Sender.Tell(-1);
        }
        private async Task onCancelFreeGame(DBCancelFreeGameRequest request)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(_strConnString))
                {
                    await connection.OpenAsync();
                    string strQuery = "SELECT TOP(1) processed,expiretime FROM userfreespinevents WHERE agentid=@agentid AND username=@username AND freespinid=@freespinid ORDER BY regdate DESC";
                    SqlCommand command = new SqlCommand(strQuery, connection);
                    command.Parameters.AddWithValue("@agentid",         request.AgentID);
                    command.Parameters.AddWithValue("@username",        request.UserName);
                    command.Parameters.AddWithValue("@freespinid",      request.FreeSpinID);
                    do
                    {
                        using (DbDataReader reader = await command.ExecuteReaderAsync())
                        {
                            if (await reader.ReadAsync())
                            {
                                int processed = (int)reader["processed"];
                                if (processed != 0)
                                {
                                    Sender.Tell(processed);
                                    return;
                                }
                            }
                            else
                            {
                                Sender.Tell(3);
                                return;
                            }
                        }
                    }
                    while (false);

                    strQuery = "UPDATE userfreespinevents SET processed=2,updatetime=@updatetime WHERE agentid=@agentid AND username=@username AND freespinid=@freespinid AND processed=0";
                    command = new SqlCommand(strQuery, connection);
                    command.Parameters.AddWithValue("@agentid",     request.AgentID);
                    command.Parameters.AddWithValue("@username",    request.UserName);
                    command.Parameters.AddWithValue("@freespinid",  request.FreeSpinID);
                    command.Parameters.AddWithValue("@updatetime",  DateTime.UtcNow);
                    await command.ExecuteNonQueryAsync();

                    Sender.Tell(0);
                    return;

                }
            }
            catch (Exception ex)
            {
            }
            Sender.Tell(-1);
        }
        private async Task onCreateUserRequest(DBCreateUserRequest request)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(_strConnString))
                {
                    await connection.OpenAsync();
                    string strQuery = "INSERT INTO users (agentid, username, password, agentname, balance, currency, isaffiliate, updatetime, registertime) VALUES " +
                        "(@agentid, @username, @password, @agentname, @balance, @currency, @isaffiliate, @updatetime, @registertime)";
                    SqlCommand command = new SqlCommand(strQuery, connection);
                    command.Parameters.AddWithValue("@agentid",     request.AgentDBID);
                    command.Parameters.AddWithValue("@username",    request.UserName);
                    command.Parameters.AddWithValue("@password",    request.Password);
                    command.Parameters.AddWithValue("@agentname",   request.AgentID);
                    command.Parameters.AddWithValue("@balance",     0.0);
                    command.Parameters.AddWithValue("@currency",    (int) request.Currency);
                    command.Parameters.AddWithValue("@isaffiliate",  request.IsAffiliate ? 1 : 0);
                    command.Parameters.AddWithValue("@updatetime",   DateTime.Now);
                    command.Parameters.AddWithValue("@registertime", DateTime.Now);
                    await command.ExecuteNonQueryAsync();
                    Sender.Tell(true);
                    return;
                }
            }
            catch (Exception)
            {
            }
            Sender.Tell(false);
        }
        private async Task onGetUserInfoRequest(DBGetUserInfoRequest request)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(_strConnString))
                {
                    await connection.OpenAsync();
                    string strQuery = "SELECT username, password, state FROM users WHERE agentid=@agentid and username=@username";
                    SqlCommand command = new SqlCommand(strQuery, connection);
                    command.Parameters.AddWithValue("@agentid",  request.AgentID);
                    command.Parameters.AddWithValue("@username", request.UserName);
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
    }

    public class DBCreateUserRequest
    {
        public string       UserName    { get; private set; }
        public string       Password    { get; private set; }
        public int          AgentDBID   { get; private set; }
        public string       AgentID     { get; private set; }
        public Currencies   Currency    { get; private set; }
        public Languages    Language    { get; private set; }
        public bool         IsAffiliate { get; private set; }

        public DBCreateUserRequest(string strUserName, string strPassword, int agentDBID, string strAgentID, Currencies currency, Languages langugage, bool isAffiliate)
        {
            this.UserName    = strUserName;
            this.Password    = strPassword;
            this.AgentDBID   = agentDBID;
            this.AgentID     = strAgentID;
            this.Currency    = currency;
            this.Language    = langugage;
            this.IsAffiliate = isAffiliate;
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
    public class DBCreateFreeGameRequest
    {
        public int      AgentID         { get; private set; }
        public string   UserName        { get; private set; }
        public int      GameID          { get; private set; }
        public int      FreeSpinCount   { get; private set; }
        public int      BetLevel        { get; private set; }
        public DateTime ExpireTime      { get; private set; }
        public string   FreeSpinID      { get; private set; }
        public DBCreateFreeGameRequest(int agentID, string strUserName, int gameid, int freeSpinCount, int betLevel, DateTime expireTime, string freeSpinID)
        {
            this.AgentID        = agentID;
            this.UserName       = strUserName;
            this.GameID         = gameid;
            this.FreeSpinCount  = freeSpinCount;
            this.BetLevel       = betLevel;
            this.ExpireTime     = expireTime;
            this.FreeSpinID     = freeSpinID;
        }
    }
    public class DBCancelFreeGameRequest
    {
        public int      AgentID         { get; private set; }
        public string   UserName        { get; private set; }
        public string   FreeSpinID      { get; private set; }
        public DBCancelFreeGameRequest(int agentID, string strUserName, string freeSpinID)
        {
            this.AgentID        = agentID;
            this.UserName       = strUserName;
            this.FreeSpinID     = freeSpinID;
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
    public class ApiProvider
    {
        public string vendorid { get; set; }
        public string name { get; set; }
        public string category { get; set; }
    }
    public class ApiGame
    {
        public string   vendorid    { get; set; }
        public int      gameid      { get; set; }
        public string   name        { get; set; }
        public string   symbol      { get; set; }
        public string   iconurl1    { get; set; }
        public string   iconurl2    { get; set; }
        public double   miniBet     { get; set; }
        public int      minlevel    { get; set; }
        public int      maxlevel    { get; set; }
    }
    public class DBGetAgentInfoRequest
    {
        public string AgentID { get; private set; }

        public DBGetAgentInfoRequest(string strAgentID)
        {
            this.AgentID = strAgentID;
        }
    }
    public class DBGetAgentInfoByIDRequest
    {
        public int AgentID { get; private set; }

        public DBGetAgentInfoByIDRequest(int agentID)
        {
            this.AgentID = agentID;
        }
    }
    public class DBAgentInfoResponse
    {
        public string   AgentID     { get; private set; }
        public int      AgentDBID   { get; private set; }
        public string   AuthToken   { get; private set; }
        public int      Currency    { get; private set; }
        public int      Language    { get; private set; }
        public bool     IsEnabled   {  get; private set; }
        public DBAgentInfoResponse(int dbID, string strAgentID, string authToken, int currency, int language, bool isEnabled)
        {
            this.AgentDBID  = dbID;
            this.AgentID    = strAgentID;
            this.AuthToken  = authToken;
            this.Currency   = currency;
            this.Language   = language;
            this.IsEnabled  = isEnabled;
        }
    }
}
