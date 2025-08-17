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

namespace ApiIntegration
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
        public UserLoginResponse()
        {

        }
        public UserLoginResponse(LoginResult result)
        {
            this.Result = result;
        }

        public UserLoginResponse(long userDBID, string strUserID, double userBalance)
        {
            this.Result             = LoginResult.OK;
            this.UserDBID           = userDBID;
            this.UserID             = strUserID;
            this.UserBalance        = userBalance;
        }
    }
    public class UserLoginRequest
    {
        public string           UserID      { get; private set; }
        public UserLoginRequest(string strUserID)
        {
            this.UserID     = strUserID;
        }
    }
            
    //유저강퇴메세지
    public class QuitUserMessage
    {
        public int    AgentID   { get; private set; }
        public string UserID    { get; private set; }

        public string GlobalUserID
        {
            get { return string.Format("{0}_{1}", AgentID, UserID); }
        }
        public QuitUserMessage(int agentID, string strUserID)
        {
            this.AgentID    = agentID;
            this.UserID     = strUserID;
        }
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
        public GITMessage Message   { get; private set; }
        public double     Balance   { get; private set; }
        public double     Delay     { get; private set; }
        public SendMessageToUser(GITMessage message, double balance, double delay)
        {
            this.Message = message;
            this.Balance = balance;
            this.Delay   = delay;
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
        public string       SessionToken { get; private set; }
        public GITMessage   Message      { get; private set; }

        public FromConnRevMessage(string token, GITMessage message)
        {
            this.SessionToken = token;
            this.Message      = message;
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

        public HTTPAuthRequest(int agentID, string strUserID, string strPasswordMD5, string strIPAddress)
        {
            this.AgentID        = agentID;
            this.UserID         = strUserID;
            this.PasswordMD5    = strPasswordMD5;
            this.IPAddress      = strIPAddress;
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

        public HTTPAuthResponse()
        {

        }
        public HTTPAuthResponse(HttpAuthResults result)
        {
            this.Result = result;
        }
        public HTTPAuthResponse(string strToken, string currency)
        {
            this.Result         = HttpAuthResults.OK;
            this.SessionToken   = strToken;
            this.Currency       = currency;
        }
    }
    public enum HttpAuthResults
    {
        OK = 0,
        IDPASSWORDERROR = 1,
        SERVERMAINTENANCE = 2
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
    public class FetchGameLobbyInfo
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
    public class QuitAndNotifyMessage
    {

    }
    
    #region Verify메시지
    public class HTTPPPVerifyGetLastItemRequest : IConsistentHashable
    {
        public string UserID    { get; private set; }
        public string Token     { get; private set; }
        public HTTPPPVerifyGetLastItemRequest(string strUserID, string strToken)
        {
            this.UserID     = strUserID;
            this.Token      = strToken;
        }
        public object ConsistentHashKey
        {
            get
            {
                return this.UserID;
            }
        }
    }
    public class BalanceFromUserRequest
    {
    }
    public class BalanceFromUserResponse
    {
        public double balance { get; set; }
        public BalanceFromUserResponse(double _balance)
        {
            this.balance = _balance;
        }

    }
    public class PPGameVerifyLastItem
    {
        public string status { get; set; }
        public double balance { get; set; }
        public string currency { get; set; }
        public string currencySymbol { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public List<PPGameVerifyDetail> rounds { get; set; }
        public PPGameVerifySettingItem settings { get; set; }
    }
    public class PPGameVerifyDetail
    {
        public long id { get; set; }
        public string name { get; set; }
        public string symbol { get; set; }
        public long date { get; set; }
        public double betAmount { get; set; }
    }
    public class PPGameVerifySettingItem
    {
        public bool displayRoundsEnabled { get; set; }
    }
    #endregion
   
}
