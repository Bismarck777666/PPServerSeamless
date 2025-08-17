using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Akka.Actor;
using GITProtocol;
using Akka.Routing;
using Newtonsoft.Json;

namespace CommNode
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

    public class UserLoginRequest
    {
        public string UserID { get; private set; }
        public string Password { get; private set; }
        public string IPAddress { get; private set; }
        public PlatformTypes Platform { get; private set; }
        public UserLoginRequest(string strUserID, string strPassword, string strIPAddress, PlatformTypes platform)
        {
            this.UserID     = strUserID;
            this.Password   = strPassword;
            this.IPAddress  = strIPAddress;
            this.Platform   = platform;
        }
    }
    public class ApiUserLoginRequest
    {
        public string UserID { get; private set; }
        public ApiUserLoginRequest(string strUserID)
        {
            this.UserID = strUserID;
        }
    }
    public class UserLoginResponse
    {
        public LoginResult  Result        { get; private set; }
        public string       UserToken     { get; private set; }
        public long         UserDBID      { get; private set; }
        public string       UserID        { get; private set; }
        public double       Balance       { get; private set; }
        public string       AgentName     { get; private set; }
        public int          AgentID       { get; private set; }
        public long         LastScoreID   { get; private set; }
        public string       IPAddress     { get; private set; }
        public Currencies   AgentCurrency   { get; set; }
        public int          AgentMoneyMode  { get; set; }
        public bool         IPRestriction   { get; set; }
        public UserLoginResponse(LoginResult resultCode)
        {
            this.Result = resultCode;
        }

        public UserLoginResponse(long userDBID, string strUserID, string strUserToken, double balance, string agentName, int agentID, long lastScoreID, string ipAddress)
        {
            this.Result         = LoginResult.OK;
            this.UserDBID       = userDBID;
            this.UserID         = strUserID;
            this.UserToken      = strUserToken;
            this.Balance        = balance;
            this.AgentName      = agentName;
            this.AgentID        = agentID;
            this.LastScoreID    = lastScoreID;
            this.IPAddress      = ipAddress;
        }
    }

    public class CreateNewUserMessage
    {
        public object               Connection      { get; private set; }
        public IActorRef            DBReader        { get; private set; }
        public IActorRef            DBWriter        { get; private set; }
        public IActorRef            RedisWriter     { get; private set; }
        public UserLoginResponse    LoginResponse   { get; private set; }
        public PlatformTypes        PlatformType    { get; private set; }

        public CreateNewUserMessage(object connection, IActorRef dbReader, IActorRef dbWriter, IActorRef redisWriter, UserLoginResponse loginResponse, PlatformTypes platform)
        {
            this.Connection = connection;
            this.DBReader = dbReader;
            this.DBWriter = dbWriter;
            this.RedisWriter = redisWriter;
            this.LoginResponse = loginResponse;
            this.PlatformType = platform;
        }
    }

    public class QuitUserMessage
    {
        public string UserID { get; private set; }
        public QuitUserMessage(string strUserID)
        {
            this.UserID = strUserID;
        }
    }

    public class MaintenanceStartMessage
    {

    }
    public class SlotGamesNodeShuttingdown
    {
        public string Path { get; private set; }
        public SlotGamesNodeShuttingdown(string strPath)
        {
            this.Path = strPath;
        }
    }

    public class SendMessageToUser
    {
        public GITMessage Message { get; private set; }

        public double     Balance { get; private set; }
        public SendMessageToUser(GITMessage message, double balance)
        {
            this.Message = message;
            this.Balance = balance;
        }
    }

    public class CheckUserPathFromRedis
    {
        public UserLoginResponse Response { get; private set; }

        public CheckUserPathFromRedis(UserLoginResponse response)
        {
            this.Response = response;
        }
    }

    public class FromConnRevMessage
    {
        public object Connection { get; private set; }
        public GITMessage Message { get; private set; }

        public FromConnRevMessage(object connection, GITMessage message)
        {
            this.Connection = connection;
            this.Message = message;
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
        public string UserID        { get; private set; }
        public string PasswordMD5   { get; private set; }
        public string IPAddress     { get; private set; }
        public string GameSymbol    {  get; private set; }
        public HTTPAuthRequest(string strUserID, string strPasswordMD5, string strIPAddress, string gameSymbol)
        {
            this.UserID         = strUserID;
            this.PasswordMD5    = strPasswordMD5;
            this.IPAddress      = strIPAddress;
            this.GameSymbol     = gameSymbol;
        }
        public object ConsistentHashKey
        {
            get
            {
                return UserID;
            }
        }
    }
    public class HTTPAuthResponse
    {
        public HttpAuthResults  Result          { get; private set; }
        public string           SessionToken    { get; private set; }
        public int              Currency        { get; private set; }
        public string           GameData        { get; private set; }
        public HTTPAuthResponse()
        {

        }
        public HTTPAuthResponse(HttpAuthResults result)
        {
            this.Result = result;
        }
        public HTTPAuthResponse(string strToken, int currency, string gameData)
        {
            this.Result         = HttpAuthResults.OK;
            this.SessionToken   = strToken;
            this.Currency       = currency;
            this.GameData       = gameData;
        }
    }
    public enum HttpAuthResults
    {
        OK = 0,
        IDPASSWORDERROR     = 1,
        COUNTRYMISMATCH     = 2,
        SERVERMAINTENANCE   = 3, 
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
        public string       UserID          { get; private set; }
        public string       SessionToken    { get; private set; }
        public GAMETYPE     GameType        { get; private set; }
        public string       GameIdentifier  { get; private set; }
        public string       IPAddress       { get; private set; }
        public HTTPEnterGameRequest(string strUserID, string strSessionToken, GAMETYPE gameType, string strGameIdentifier, string strIPAddress)
        {
            this.UserID         = strUserID;
            this.SessionToken   = strSessionToken;
            this.GameType       = gameType;
            this.GameIdentifier = strGameIdentifier;
            this.IPAddress      = strIPAddress;
        }

        public object ConsistentHashKey
        {
            get
            {
                return UserID;
            }
        }
    }

    public class HttpGetMiniLobbyGamesMessage : IConsistentHashable
    {
        public string UserID        { get; set; }
        public string SessionToken  { get; private set; }

        public object ConsistentHashKey
        {
            get
            {
                return this.UserID;
            }
        }

        public HttpGetMiniLobbyGamesMessage(string strUserID, string strSessionToken)
        {
            this.UserID         = strUserID;
            this.SessionToken   = strSessionToken;
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
        public string UserID { get; private set; }
        public string Token { get; private set; }
        public int GameID { get; private set; }
        public int EnvID { get; private set; }
        public string Symbol { get; private set; }
        public long RoundID { get; private set; }

        public HTTPPPReplayMakeLinkRequest(string strUserID, string strToken, int gameID, long roundID, string strSymbol, int envID)
        {
            this.UserID = strUserID;
            this.Token = strToken;
            this.GameID = gameID;
            this.RoundID = roundID;
            this.Symbol = strSymbol;
            this.EnvID = envID;
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
        public string UserID { get; private set; }
        public string Token { get; private set; }
        public int GameID { get; private set; }
        public long RoundID { get; private set; }

        public HTTPPPReplayDataRequest(string strUserID, string strToken, int gameID, long roundID)
        {
            this.UserID = strUserID;
            this.Token = strToken;
            this.GameID = gameID;
            this.RoundID = roundID;
        }
        public object ConsistentHashKey
        {
            get
            {
                return this.UserID;
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
        public string   UserID  { get; private set; }
        public string   Token   { get; private set; }
        public int      GameID  { get; private set; }
        public HTTPPPHistoryGenralSettingRequest(string strUserID, string strToken, int gameID)
        {
            this.UserID = strUserID;
            this.Token = strToken;
            this.GameID = gameID;
        }

        public object ConsistentHashKey
        {
            get
            {
                return this.UserID;
            }
        }
    }
    public class HTTPPPHistoryGetByRoundRequest : IConsistentHashable
    {
        public string AgentID   { get; private set; }
        public string Token     { get; private set; }
        public string Currency  { get; private set; }
        public long   RoundID   { get; private set; }
        public HTTPPPHistoryGetByRoundRequest(string agentID, string strToken, long roundID, string currency)
        {
            this.AgentID    = agentID;
            this.Currency   = currency;
            this.Token      = strToken;
            this.RoundID    = roundID;
        }
        public object ConsistentHashKey
        {
            get
            {
                return this.Token;
            }
        }
    }
    public class HTTPPPHistoryGetLastItemsRequest : IConsistentHashable
    {
        public string   UserID  { get; private set; }
        public string   Token   { get; private set; }
        public int      GameID  { get; private set; }
        public HTTPPPHistoryGetLastItemsRequest(string strUserID, string strToken, int gameID)
        {
            this.UserID = strUserID;
            this.Token = strToken;
            this.GameID = gameID;
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
    public class HTTPPPHistoryGetItemDetailRequestByRoundID : IConsistentHashable
    {
        public string Token   { get; private set; }
        public string AgentID { get; private set; }
        public long   RoundID { get; private set; }
        public string Currency { get; private set; }
        public HTTPPPHistoryGetItemDetailRequestByRoundID(string agentID, string strToken, long roundID, string currency)
        {
            this.Token   = strToken;
            this.RoundID = roundID;
            this.AgentID = agentID;
            this.Currency = currency;
        }

        public object ConsistentHashKey
        {
            get
            {
                return this.Token;
            }
        }
    }

    public class HTTPPPReplayListRequest : IConsistentHashable
    {
        public string UserID { get; private set; }
        public string Token { get; private set; }
        public int GameID { get; private set; }
        public HTTPPPReplayListRequest(string strUserID, string strToken, int gameID)
        {
            this.UserID = strUserID;
            this.Token = strToken;
            this.GameID = gameID;
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
    public class FetchGameLobbyInfo
    {

    }
    public class QuitAndNotifyMessage
    {

    }

    public class GameLobbyInfo
    {
        public int                      error           { get; set; }
        public string                   description     { get; set; }
        public string                   gameLaunchURL   { get; set; }
        public string                   gameIconsURL    { get; set; }
        public List<GameLobbyCategory>  lobbyCategories { get; set; }
    }

    public class GameLobbyCategory
    {
        public string categorySymbol { get; set; }
        public string categoryName { get; set; }
        public List<GameLobbyGameInfo> lobbyGames { get; set; }
    }

    public class GameLobbyGameInfo
    {
        public string name          { get; set; }
        public string symbol        { get; set; }
        public int    hasDeveloed   { get; set; }
    }

    public class WithdrawRequest : IConsistentHashable
    {
        public string UserID    { get; private set; }
        public string Password  { get; private set; }        
        public bool   IsStart   { get; private set; }
        public WithdrawRequest(string strUserID, string strPassword, bool isStart)
        {
            this.UserID     = strUserID;
            this.Password   = strPassword;
            this.IsStart    = isStart;
        }

        public object ConsistentHashKey
        {
            get
            {
                return UserID;
            }
        }
    }
}
