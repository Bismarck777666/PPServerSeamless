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

        public UserLoginResponse(string agentID, int agentDBID, long userDBID, string strUserID, string strPassToken, double userBalance, long lastScoreCounter, int currency)
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
    public class RequestPPReplayList
    {
        public string UserID { get; private set; }
        public int GameID { get; private set; }
        public IActorRef Sender { get; private set; }

        public RequestPPReplayList(string strUserID, int gameID, IActorRef sender)
        {
            this.UserID = strUserID;
            this.GameID = gameID;
            this.Sender = sender;
        }
    }
    public class RequestPPReplayLink
    {
        public string UserID { get; private set; }
        public int GameID { get; private set; }
        public long RoundID { get; private set; }
        public IActorRef Sender { get; private set; }

        public RequestPPReplayLink(string strUserID, int gameID, long roundID, IActorRef sender)
        {
            this.UserID = strUserID;
            this.GameID = gameID;
            this.RoundID = roundID;
            this.Sender = sender;
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
            this.AgentID        = agentID;
            this.UserID         = strUserID;
            this.PasswordMD5    = strPasswordMD5;
            this.IPAddress      = strIPAddress;
            this.GameSymbol     = strSymbol;
        }
        public object ConsistentHashKey
        {
            get
            {
                return string.Format("{0}_{1}", AgentID, UserID);
            }
        }
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
            this.Result         = HttpAuthResults.OK;
            this.SessionToken   = strToken;
            this.Currency       = currency;
            this.GameName       = gameName;
            this.GameData       = gameData;
        }
    }
    public enum HttpAuthResults
    {
        OK                  = 0,
        IDPASSWORDERROR     = 1,
        SERVERMAINTENANCE   = 2,
        INVALIDGAMESYMBOL   = 3,
    }

    public enum HTTPEnterGameResults
    {
        OK = 0,
        INVALIDTOKEN = 1,
        INVALIDGAMEID = 2,
        INVALIDACTION = 3,
    }
    public class HTTPEnterGameRequest : IConsistentHashable
    {
        public string UserID { get; private set; }
        public string SessionToken { get; private set; }
        public GameProviders     GameType        { get; private set; }
        public string GameIdentifier { get; private set; }

        public HTTPEnterGameRequest(string strUserID, string strSessionToken, GameProviders gameType, string strGameIdentifier)
        {
            this.UserID = strUserID;
            this.SessionToken = strSessionToken;
            this.GameType       = gameType;
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
        public string UserID { get; private set; }
        public string SessionToken { get; private set; }
        public GITMessage Message { get; private set; }

        public FromHTTPSessionMessage(string strUserID, string strSessionToken, GITMessage message)
        {
            this.UserID = strUserID;
            this.SessionToken = strSessionToken;
            this.Message = message;
        }

        public object ConsistentHashKey
        {
            get
            {
                return this.UserID;
            }
        }
    }

    public class HttpLogoutRequest
    {
        public string Token { get; private set; }
        public string UserID { get; private set; }
        public HttpLogoutRequest(string strUserID, string strToken)
        {
            this.UserID = strUserID;
            this.Token = strToken;
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
        public GITMessage Message { get; private set; }
        public ToHTTPSessionMessage(ToHTTPSessionMsgResults result)
        {
            this.Result = result;
        }

        public ToHTTPSessionMessage()
        {

        }

        public ToHTTPSessionMessage(GITMessage message)
        {
            this.Result = ToHTTPSessionMsgResults.OK;
            this.Message = message;
        }
    }

    public class HTTPPPReplayMakeLinkRequest : IConsistentHashable
    {
        public string   UserID      { get; private set; }
        public string   Token       { get; private set; }
        public int      GameID      { get; private set; }
        public int      EnvID       { get; private set; }
        public string   Symbol      { get; private set; }
        public long     RoundID     { get; private set; }
        public string   Lang        { get; private set; }

        public HTTPPPReplayMakeLinkRequest(string strUserID, string strToken, int gameID, long roundID, string strSymbol, int envID, string lang)
        {
            this.UserID     = strUserID;
            this.Token      = strToken;
            this.GameID     = gameID;
            this.RoundID    = roundID;
            this.Symbol     = strSymbol;
            this.EnvID      = envID;
            this.Lang       = lang;
        }
        public object ConsistentHashKey
        {
            get
            {
                return this.UserID;
            }
        }
    }

    public class HTTPPPReplayMakeLinkResponse
    {
        public string Response { get; private set; }
        public HTTPPPReplayMakeLinkResponse(string strResponse)
        {
            this.Response = strResponse;
        }
    }

    public class HTTPPPReplayDataRequest : IConsistentHashable
    {
        public string   UserID              { get; private set; }
        public string   Token               { get; private set; }
        public int      GameID              { get; private set; }
        public long     RoundID             { get; private set; }

        public HTTPPPReplayDataRequest(string strUserID, string strToken, int gameID, long roundID)
        {
            this.UserID         = strUserID;
            this.Token          = strToken;
            this.GameID         = gameID;
            this.RoundID        = roundID;
        }

        public object ConsistentHashKey
        {
            get
            {
                return UserID;
            }
        }
    }

    public class HTTPPPReplayDataResponse
    {
        public string Response { get; private set; }
        public HTTPPPReplayDataResponse(string strResponse)
        {
            this.Response = strResponse;
        }
    }
    public class HTTPPPHistoryGenralSettingRequest : IConsistentHashable
    {
        public string   UserID      { get; private set; }
        public string   Token       { get; private set; }
        public int      GameID      { get; private set; }
        public HTTPPPHistoryGenralSettingRequest(string strUserID, string strToken, int gameID)
        {
            this.UserID     = strUserID;
            this.Token      = strToken;
            this.GameID     = gameID;
        }

        public object ConsistentHashKey
        {
            get
            {
                return this.UserID;
            }
        }
    }
    public class HTTPPPHistoryGetLastItemsRequest : IConsistentHashable
    {
        public string   UserID      { get; private set; }
        public string   Token       { get; private set; }
        public int      GameID      { get; private set; }
        public HTTPPPHistoryGetLastItemsRequest(string strUserID, string strToken, int gameID)
        {
            this.UserID     = strUserID;
            this.Token      = strToken;
            this.GameID     = gameID;
        }

        public object ConsistentHashKey
        {
            get
            {
                return this.UserID;
            }
        }
    }

    public class HTTPPPHistoryGetItemDetailRequest : IConsistentHashable
    {
        public string   UserID      { get; private set; }
        public string   Token       { get; private set; }
        public int      GameID      { get; private set; }
        public long     RoundID     { get; private set; }
        public HTTPPPHistoryGetItemDetailRequest(string strUserID, string strToken, int gameID, long roundID)
        {
            this.UserID         = strUserID;
            this.Token          = strToken;
            this.GameID         = gameID;
            this.RoundID        = roundID;
        }

        public object ConsistentHashKey
        {
            get
            {
                return this.UserID;
            }
        }
    }
    public class HTTPPPReplayListRequest : IConsistentHashable
    {
        public string   UserID      { get; private set; }
        public string   Token       { get; private set; }
        public int      GameID      { get; private set; }
        public HTTPPPReplayListRequest(string strUserID, string strToken, int gameID)
        {
            this.UserID     = strUserID;
            this.Token      = strToken;
            this.GameID     = gameID;
        }

        public object ConsistentHashKey
        {
            get
            {
                return this.UserID;
            }
        }
    }

    public class HTTPPPReplayListResponse
    {
        public List<PPGameHistoryTopListItem> Items { get; private set; }

        public HTTPPPReplayListResponse(List<PPGameHistoryTopListItem> items)
        {
            this.Items = items;
        }
    }

    public class PPReplayListResponse
    {
        public int error { get; set; }
        public string description { get; set; }
        public List<PPGameHistoryTopListItem> topList { get; set; }
    }
    #endregion
         
}
