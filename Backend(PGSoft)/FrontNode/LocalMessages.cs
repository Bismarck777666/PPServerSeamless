using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Akka.Actor;
using GITProtocol;
using Akka.Routing;
using Newtonsoft.Json;

/****
 * 
 *          Created by Foresight(2021.03.12)
 *          해당 노드내부에서 교환되는 메세지들을 정의한다.
 * 
 */

namespace FrontNode
{


    public enum PlatformTypes
    {
        MOBILE = 0,
        WEB = 1,
    }
    public enum LoginResult
    {
        OK = 1,
        ALREADYLOGGEDIN = 3,
        IDPASSWORDMISMATCH = 5,
        COUNTRYMISMATCH = 6,
        ACCOUNTDISABLED = 10,
        UNKNOWNERROR = 11,
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
        public bool         IsAffiliate      { get; private set; }
        public UserLoginResponse()
        {

        }
        public UserLoginResponse(LoginResult result)
        {
            this.Result = result;
        }

        public string GlobalUserID
        {
            get { return string.Format("{0}_{1}", this.AgentDBID, this.UserID); }
        }

        public UserLoginResponse(string agentID, int agentDBID, long userDBID, string strUserID, string strPassToken, double userBalance, long lastScoreCounter, int currency, bool isAffiliate)
        {
            this.Result             = LoginResult.OK;
            this.AgentID            = agentID;
            this.AgentDBID          = agentDBID;
            this.UserDBID           = userDBID;
            this.UserID             = strUserID;
            this.PassToken          = strPassToken;
            this.UserBalance        = userBalance;
            this.LastScoreCounter   = lastScoreCounter;
            this.Currency           = (Currencies)currency;
            this.IsAffiliate        = isAffiliate;
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
            this.AgentID    = agentID;
            this.UserID     = strUserID;
            this.Password   = strPassword;
            this.IPAddress  = strIPAddress;
            this.Platform   = platform;
        }
    }                   
    
    #region Message related to HTTP Session    
    public class HTTPOperatorVerifyRequest : IConsistentHashable
    {
        public int      AgentID     { get; private set; }
        public string   UserID      { get; private set; }
        public string   PasswordMD5 { get; private set; }
        public int      GameID      { get; private set; }
        public HTTPOperatorVerifyRequest(int agentID, string strUserID, string strPasswordMD5, int gameID)
        {
            this.AgentID        = agentID;
            this.UserID         = strUserID;
            this.PasswordMD5    = strPasswordMD5;
            this.GameID         = gameID;
        }
        public object ConsistentHashKey
        {
            get
            {
                return UserID;
            }
        }
    }
    public class HTTPEnterGameRequest : IConsistentHashable
    {
        public int      AgentID         { get; private set; }
        public string   UserID          { get; private set; }
        public string   SessionToken    { get; private set; }
        public string   GameIdentifier  { get; private set; }

        public HTTPEnterGameRequest(int agentID, string strUserID, string strSessionToken, string strGameIdentifier)
        {
            this.AgentID        = agentID;
            this.UserID         = strUserID;
            this.SessionToken   = strSessionToken;
            this.GameIdentifier = strGameIdentifier;
        }

        public object ConsistentHashKey
        {
            get
            {
                return UserID;
            }
        }
    }

    public class FromHTTPSessionMessage : IConsistentHashable
    {
        public int          AgentID         { get; private set; }
        public string       UserID          { get; private set; }
        public string       SessionToken    { get; private set; }
        public GITMessage   Message         { get; private set; }

        public FromHTTPSessionMessage(int agentID, string strUserID, string strSessionToken, GITMessage message)
        {
            this.AgentID        = agentID;
            this.UserID         = strUserID;
            this.SessionToken   = strSessionToken;
            this.Message        = message;
        }

        public object ConsistentHashKey
        {
            get
            {
                return this.UserID;
            }
        }
    }

    public enum ToHTTPSessionMsgResults
    {
        OK = 0,
        INVALIDTOKEN = 1,
        INVALIDACTION = 2,
    }
    public class ToHTTPSessionMessage
    {
        public ToHTTPSessionMsgResults Result { get; private set; }
        public object Response { get; private set; }
        public ToHTTPSessionMessage(ToHTTPSessionMsgResults result)
        {
            this.Result = result;
        }

        public ToHTTPSessionMessage()
        {

        }

        public ToHTTPSessionMessage(object response)
        {
            this.Result = ToHTTPSessionMsgResults.OK;
            this.Response = response;
        }
    }

    #endregion

}
