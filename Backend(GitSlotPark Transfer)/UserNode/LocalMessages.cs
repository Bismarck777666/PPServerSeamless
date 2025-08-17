using Akka.Actor;
using Akka.Routing;
using GITProtocol;
using System.Collections.Generic;

namespace UserNode
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
        ACCOUNTDISABLED     = 10, // 0x0000000A
        UNKNOWNERROR        = 11, // 0x0000000B
    }
    public class QuitUserMessage
    {
        public int      AgentID { get; private set; }
        public string   UserID  { get; private set; }
        public string   GlobalUserID => string.Format("{0}_{1}", AgentID, UserID);

        public QuitUserMessage(int agentID, string strUserID)
        {
            AgentID = agentID;
            UserID  = strUserID;
        }
    }
    public class SlotGamesNodeShuttingdown
    {
        public string Path { get; private set; }

        public SlotGamesNodeShuttingdown(string strPath)
        {
            Path = strPath;
        }
    }

    public class RequestPPReplayList
    {
        public string       UserID { get; private set; }
        public int          GameID { get; private set; }
        public IActorRef    Sender { get; private set; }

        public RequestPPReplayList(string strUserID, int gameID, IActorRef sender)
        {
            UserID  = strUserID;
            GameID  = gameID;
            Sender  = sender;
        }
    }
    public class RequestPPReplayLink
    {
        public string       UserID  { get; private set; }
        public int          GameID  { get; private set; }
        public long         RoundID { get; private set; }
        public IActorRef    Sender  { get; private set; }

        public RequestPPReplayLink(string strUserID, int gameID, long roundID, IActorRef sender)
        {
            UserID  = strUserID;
            GameID  = gameID;
            RoundID = roundID;
            Sender  = sender;
        }
    }
    public class HTTPAuthRequest : IConsistentHashable
    {
        public int      AgentID     { get; private set; }
        public string   UserID      { get; private set; }
        public string   PasswordMD5 { get; private set; }
        public string   IPAddress   { get; private set; }

        public HTTPAuthRequest(int agentID,string strUserID,string strPasswordMD5,string strIPAddress)
        {
            AgentID     = agentID;
            UserID      = strUserID;
            PasswordMD5 = strPasswordMD5;
            IPAddress   = strIPAddress;
        }

        public object ConsistentHashKey
        {
            get => string.Format("{0}_{1}", AgentID, UserID);
        }
    }
    public enum HttpAuthResults
    {
        OK                  = 0,
        IDPASSWORDERROR     = 1,
        SERVERMAINTENANCE   = 2,
    }
    public enum HTTPEnterGameResults
    {
        OK              = 0,
        INVALIDTOKEN    = 1,
        INVALIDGAMEID   = 2,
        INVALIDACTION   = 3,
    }
    public class HTTPAuthResponse
    {
        public HttpAuthResults  Result          { get; private set; }
        public string           SessionToken    { get; private set; }
        public string           Currency        { get; private set; }

        public HTTPAuthResponse()
        {
        }

        public HTTPAuthResponse(HttpAuthResults result)
        {
            Result = result;
        }

        public HTTPAuthResponse(string strToken, string currency)
        {
            Result          = HttpAuthResults.OK;
            SessionToken    = strToken;
            Currency        = currency;
        }
    }
    public class HTTPEnterGameRequest : IConsistentHashable
    {
        public string           UserID          { get; private set; }
        public string           SessionToken    { get; private set; }
        public GameProviders    GameType        { get; private set; }
        public string           GameIdentifier  { get; private set; }

        public HTTPEnterGameRequest(string strUserID,string strSessionToken,GameProviders gameType,string strGameIdentifier)
        {
            UserID          = strUserID;
            SessionToken    = strSessionToken;
            GameType        = gameType;
            GameIdentifier  = strGameIdentifier;
        }

        public object ConsistentHashKey => UserID;
    }
    public class FromHTTPSessionMessage : IConsistentHashable
    {
        public string       UserID          { get; private set; }
        public string       SessionToken    { get; private set; }
        public GITMessage   Message         { get; private set; }

        public FromHTTPSessionMessage(string strUserID, string strSessionToken, GITMessage message)
        {
            UserID          = strUserID;
            SessionToken    = strSessionToken;
            Message         = message;
        }

        public object ConsistentHashKey => UserID;
    }
    public class HttpLogoutRequest
    {
        public string Token     { get; private set; }
        public string UserID    { get; private set; }

        public HttpLogoutRequest(string strUserID, string strToken)
        {
            UserID  = strUserID;
            Token   = strToken;
        }
    }
    public enum ToHTTPSessionMsgResults
    {
        OK              = 0,
        INVALIDTOKEN    = 1,
        INVALIDACTION   = 2,
    }
    public class ToHTTPSessionMessage
    {
        public ToHTTPSessionMsgResults  Result  { get; private set; }
        public GITMessage               Message { get; private set; }
        public ToHTTPSessionMessage(ToHTTPSessionMsgResults result)
        {
            Result = result;
        }

        public ToHTTPSessionMessage()
        {
        }

        public ToHTTPSessionMessage(GITMessage message)
        {
            Result  = ToHTTPSessionMsgResults.OK;
            Message = message;
        }
    }
    public class HTTPPPReplayMakeLinkRequest : IConsistentHashable
    {
        public string   UserID  { get; private set; }
        public string   Token   { get; private set; }
        public int      GameID  { get; private set; }
        public int      EnvID   { get; private set; }
        public string   Symbol  { get; private set; }
        public long     RoundID { get; private set; }
        public string   Lang    { get; private set; }

        public HTTPPPReplayMakeLinkRequest(string strUserID,string strToken,int gameID,long roundID,string strSymbol,int envID,string lang)
        {
            UserID  = strUserID;
            Token   = strToken;
            GameID  = gameID;
            RoundID = roundID;
            Symbol  = strSymbol;
            EnvID   = envID;
            Lang    = lang;
        }

        public object ConsistentHashKey => UserID;
    }
    public class HTTPPPReplayMakeLinkResponse
    {
        public string Response { get; private set; }

        public HTTPPPReplayMakeLinkResponse(string strResponse)
        {
            Response = strResponse;
        }
    }
    public class HTTPPPReplayDataRequest : IConsistentHashable
    {
        public string   UserID  { get; private set; }
        public string   Token   { get; private set; }
        public int      GameID  { get; private set; }
        public long     RoundID { get; private set; }

        public HTTPPPReplayDataRequest(string strUserID, string strToken, int gameID, long roundID)
        {
            UserID  = strUserID;
            Token   = strToken;
            GameID  = gameID;
            RoundID = roundID;
        }

        public object ConsistentHashKey => UserID;
    }
    public class HTTPPPReplayDataResponse
    {
        public string Response { get; private set; }

        public HTTPPPReplayDataResponse(string strResponse)
        {
            Response = strResponse;
        }
    }
    public class HTTPPPHistoryGenralSettingRequest : IConsistentHashable
    {
        public string   UserID  { get; private set; }
        public string   Token   { get; private set; }
        public int      GameID  { get; private set; }

        public HTTPPPHistoryGenralSettingRequest(string strUserID, string strToken, int gameID)
        {
            UserID  = strUserID;
            Token   = strToken;
            GameID  = gameID;
        }

        public object ConsistentHashKey => UserID;
    }
    public class HTTPPPHistoryGetLastItemsRequest : IConsistentHashable
    {
        public string   UserID  { get; private set; }
        public string   Token   { get; private set; }
        public int      GameID  { get; private set; }

        public HTTPPPHistoryGetLastItemsRequest(string strUserID, string strToken, int gameID)
        {
            UserID  = strUserID;
            Token   = strToken;
            GameID  = gameID;
        }

        public object ConsistentHashKey => UserID;
    }
    public class HTTPPPHistoryGetItemDetailRequest : IConsistentHashable
    {
        public string   UserID  { get; private set; }
        public string   Token   { get; private set; }
        public int      GameID  { get; private set; }
        public long     RoundID { get; private set; }

        public HTTPPPHistoryGetItemDetailRequest(string strUserID,string strToken,int gameID,long roundID)
        {
            UserID  = strUserID;
            Token   = strToken;
            GameID  = gameID;
            RoundID = roundID;
        }

        public object ConsistentHashKey => UserID;
    }
    public class HTTPPPReplayListRequest : IConsistentHashable
    {
        public string   UserID  { get; private set; }
        public string   Token   { get; private set; }
        public int      GameID  { get; private set; }

        public HTTPPPReplayListRequest(string strUserID, string strToken, int gameID)
        {
            UserID  = strUserID;
            Token   = strToken;
            GameID  = gameID;
        }

        public object ConsistentHashKey => UserID;
    }
    public class HTTPPPReplayListResponse
    {
        public List<PPGameHistoryTopListItem> Items { get; private set; }

        public HTTPPPReplayListResponse(List<PPGameHistoryTopListItem> items)
        {
            Items = items;
        }
    }
    public class PPReplayListResponse
    {
        public int                              error       { get; set; }
        public string                           description { get; set; }
        public List<PPGameHistoryTopListItem>   topList     { get; set; }
    }
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
        public string categorySymbol    { get; set; }
        public string categoryName      { get; set; }
        public List<GameLobbyGameInfo> lobbyGames { get; set; }
    }
    public class GameLobbyGameInfo
    {
        public string   name        { get; set; }
        public string   symbol      { get; set; }
        public int      hasDeveloed { get; set; }
    }



}
