using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Akka.Actor;
using GITProtocol;
using Akka.Routing;
using Newtonsoft.Json;

namespace FrontNode
{
    public enum PlatformTypes
    {
        MOBILE  = 0,
        WEB     = 1,
    }
    public enum LoginResult
    {
        OK                  = 1,
        ALREADYLOGGEDIN     = 3,
        IDPASSWORDMISMATCH  = 5,
        COUNTRYMISMATCH     = 6,
        ACCOUNTDISABLED     = 10,
        UNKNOWNERROR        = 11,
    }

    public class UserLoginResponse
    {
        public LoginResult  Result           { get; private set; }
        public long         UserDBID         { get; private set; }
        public string       UserID           { get; private set; }
        public double       UserBalance      { get; private set; }
        public string       PassToken        { get; private set; }
        public string       AgentID          { get; private set; }
        public int          AgentDBID        { get; private set; }
        public long         LastScoreCounter { get; private set; }
        public Currencies   Currency         { get; private set; }
        public UserLoginResponse()
        {

        }
        public UserLoginResponse(LoginResult result)
        {
            this.Result = result;
        }
        public string       GlobalUserID => string.Format("{0}_{1}", AgentDBID, UserID);

        public UserLoginResponse(string agentID, int agentDBID, long userDBID, string strUserID, string strPassToken, double userBalance, long lastScoreCounter, int currency)
        {
            Result              = LoginResult.OK;
            AgentID             = agentID;
            AgentDBID           = agentDBID;
            UserDBID            = userDBID;
            UserID              = strUserID;
            PassToken           = strPassToken;
            UserBalance         = userBalance;
            LastScoreCounter    = lastScoreCounter;
            Currency            = (Currencies)currency;
        }
    }
    public class UserLoginRequest
    {
        public int              AgentID     { get; private set; }
        public string           UserID      { get; private set; }
        public string           Password    { get; private set; }
        public string           IPAddress   { get; private set; }
        public PlatformTypes    Platform    { get; private set; }
        public UserLoginRequest(int agentID, string strUserID, string strPassword, string strIPAddress, PlatformTypes platform)
        {
            AgentID     = agentID;
            UserID      = strUserID;
            Password    = strPassword;
            IPAddress   = strIPAddress;
            Platform    = platform;
        }
    }                   

    #region Message related to HTTP Session    
    public class HTTPAuthRequest : IConsistentHashable
    {
        public int          AgentID         { get; private set; }
        public string       UserID          { get; private set; }
        public string       PasswordMD5     { get; private set; }
        public string       IPAddress       { get; private set; }
        public string       GameSymbol      { get; private set; }
        public HTTPAuthRequest(int agentID, string strUserID, string strPasswordMD5, string strIPAddress, string strSymbol)
        {
            AgentID         = agentID;
            UserID          = strUserID;
            PasswordMD5     = strPasswordMD5;
            IPAddress       = strIPAddress;
            GameSymbol      = strSymbol;
        }
        public object       ConsistentHashKey => string.Format("{0}_{1}", AgentID, UserID);
    }
    public class HTTPAuthResponse
    {
        public HttpAuthResults  Result       { get; private set; }
        public string           SessionToken { get; private set; }
        public string           Currency     { get; private set; }
        public string           GameName     { get; private set; }
        public string           GameData     { get; private set; }
        public HTTPAuthResponse()
        {

        }
        public HTTPAuthResponse(HttpAuthResults result)
        {
            this.Result = result;
        }
        public HTTPAuthResponse(string strToken, string currency, string gameName, string gameData)
        {
            Result          = HttpAuthResults.OK;
            SessionToken    = strToken;
            Currency        = currency;
            GameName        = gameName;
            GameData        = gameData;
        }
    }
    public enum HttpAuthResults
    {
        OK                  = 0,
        IDPASSWORDERROR     = 1,
        SERVERMAINTENANCE   = 2,
        INVALIDGAMESYMBOL   = 3,
    }
    #endregion

    public class CheckUserPathFromRedis
    {
        public UserLoginResponse Response { get; private set; }

        public CheckUserPathFromRedis(UserLoginResponse response)
        {
            this.Response = response;
        }
    }
}
