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

namespace CommNode
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

    public class UserLoginResponse
    {
        public LoginResult  Result              { get; private set; }
        public long         UserDBID            { get; private set; }
        public string       UserID              { get; private set; }
        public double       UserBalance         { get; private set; }
        public int          Currency            { get; private set; }
        public string       PassToken           { get; private set; }
        public int          AgentDBID           { get; private set; }
        public long         LastScoreCounter    { get; private set; }
        public string       GlobalUserID        => string.Format("{0}_{1}", AgentDBID, UserID);

        public UserLoginResponse()
        {
        }

        public UserLoginResponse(LoginResult resultCode)
        {
            this.Result     = resultCode;
        }

        public UserLoginResponse(int agentID,long userDBID,string strUserID,string strPassToken,double userBalance,int currency,long lastScoreCounter)
        {
            Result              = LoginResult.OK;
            AgentDBID           = agentID;
            UserDBID            = userDBID;
            UserID              = strUserID;
            PassToken           = strPassToken;
            UserBalance         = userBalance;
            Currency            = currency;
            LastScoreCounter    = lastScoreCounter;
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
            Connection      = connection;
            DBReader        = dbReader;
            DBWriter        = dbWriter;
            RedisWriter     = redisWriter;
            LoginResponse   = loginResponse;
            PlatformType    = platform;
        }

    }

    public class GetUserClosedGameIDs
    {
        public long UserDBID { get; private set; }
        public GetUserClosedGameIDs(long userID)
        {
            this.UserDBID = userID;
        }
    }

    public class GetUserBonusItems
    {
        public int      AgentID { get; private set; }
        public string   UserID  { get; private set; }
        public GetUserBonusItems(int agentID, string strUserID)
        {
            this.AgentID    = agentID;
            this.UserID     = strUserID;
        }
    }

    public class ClaimedRedPacketMessage
    {
        public long     ID      { get; private set; }
        public string   UserID  { get; private set; }
        public ClaimedRedPacketMessage(long id, string strUserID)
        {
            this.ID     = id;
            this.UserID = strUserID;
        }
    }

    public class ClaimedUserRangeEventMessage
    {
        public long     ID              { get; private set; }
        public int      AgentID         { get; private set; }
        public string   UserID          { get; private set; }
        public double   RewardedMoney   { get; private set; }
        public string   GameName        { get; private set; }
        public ClaimedUserRangeEventMessage(long id,int agentid ,string strUserID, double rewardedMoney, string strGameName)
        {
            this.ID             = id;
            this.AgentID        = agentid;
            this.UserID         = strUserID;
            this.RewardedMoney  = rewardedMoney;
            this.GameName       = strGameName;
        }
    }
    
    public class BaseBonusItem
    {
        public int      AgentID         { get; protected set; }
        public long     BonusID         { get; protected set; }
        public string   UserID          { get; protected set; }
        public string   GlobalUserID    => string.Format("{0}_{1}", AgentID, UserID);

        public BaseBonusItem(long bonusID,int agentID ,string strUserID)
        {
            this.AgentID    = agentID;
            this.BonusID    = bonusID;
            this.UserID     = strUserID;
        }
    }
    
    public class UserMaxWinEventItem : BaseBonusItem
    {
        public double MaxWin    { get; private set; }

        public UserMaxWinEventItem(long bonusID,int agentID ,string strUserID, double maxWin) : base(bonusID,agentID ,strUserID)
        {
            this.MaxWin = maxWin;
        }
    }
    
    public class UserRangeOddEventItem : BaseBonusItem
    {
        public double MinOdd { get; private set; }
        public double MaxOdd { get; private set; }
        public double MaxBet { get; private set; }
        public UserRangeOddEventItem(long bonusID,int agentID ,string strUserID, double minOdd, double maxOdd, double maxBet) : base(bonusID,agentID ,strUserID)
        {
            this.MinOdd = minOdd;
            this.MaxOdd = maxOdd;
            this.MaxBet = maxBet;
        }
    }

    public class UserEventCancelled
    {
        public int      AgentID         { get; private set; }
        public string   UserID          { get; private set; }
        public long     BonusID         { get; private set; }
        public string GlobalUserID      => string.Format("{0}_{1}", AgentID, UserID);

        public UserEventCancelled(int agentID, string strUserID, long bonusID)
        {
            AgentID     = agentID;
            UserID      = strUserID;
            BonusID     = bonusID;
        }
    }

    //유저강퇴메세지
    public class QuitUserMessage
    {
        public int      AgentID         { get; private set; }
        public string   UserID          { get; private set; }
        public string   GlobalUserID    => string.Format("{0}_{1}", AgentID, UserID);
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
        public object       Connection  { get; private set; }
        public GITMessage   Message     { get; private set; }

        public FromConnRevMessage(object connection, GITMessage message)
        {
            this.Connection     = connection;
            this.Message        = message;
        }
    }

    public class QuitAndNotifyMessage
    {
    }

    public class RequestPPReplayList
    {
        public string       UserID  { get; private set; }
        public int          GameID  { get; private set; }
        public IActorRef    Sender  { get; private set; }

        public RequestPPReplayList(string strUserID, int gameID, IActorRef sender)
        {
            this.UserID = strUserID;
            this.GameID = gameID;
            this.Sender = sender;
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
            this.UserID     = strUserID;
            this.GameID     = gameID;
            this.RoundID    = roundID;
            this.Sender     = sender;
        }
    }

    #region Message related to HTTP Session    
    public class HTTPAuthRequest : IConsistentHashable
    {
        public int      AgentID         { get; private set; }
        public string   UserID          { get; private set; }
        public string   PasswordMD5     { get; private set; }
        public string   IPAddress       { get; private set; }
        public object   ConsistentHashKey
        {
            get
            {
                return UserID;
            }
        }

        public HTTPAuthRequest(int agentID, string strUserID, string strPasswordMD5, string strIPAddress)
        {
            AgentID     = agentID;
            UserID      = strUserID;
            PasswordMD5 = strPasswordMD5;
            IPAddress   = strIPAddress;
        }
    }
    
    public class HTTPAuthResponse
    {
        public HttpAuthResults  Result          { get; private set; }
        public string           SessionToken    { get; private set; }

        public HTTPAuthResponse()
        {

        }
        public HTTPAuthResponse(HttpAuthResults result)
        {
            this.Result = result;
        }
        public HTTPAuthResponse(string strToken)
        {
            this.Result         = HttpAuthResults.OK;
            this.SessionToken   = strToken;
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

    public class HTTPEnterGameRequest : IConsistentHashable
    {
        public string       UserID          { get; private set; }
        public string       SessionToken    { get; private set; }
        public GAMETYPE     GameType        { get; private set; }
        public string       GameIdentifier  { get; private set; }
        public object       ConsistentHashKey => UserID;
        
        public HTTPEnterGameRequest(string strUserID, string strSessionToken, GAMETYPE gameType, string strGameIdentifier)
        {
            this.UserID         = strUserID;
            this.SessionToken   = strSessionToken;
            this.GameType       = gameType;
            this.GameIdentifier = strGameIdentifier;
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
            this.Result = result;
        }

        public ToHTTPSessionMessage()
        {

        }

        public ToHTTPSessionMessage(GITMessage message)
        {
            this.Result     = ToHTTPSessionMsgResults.OK;
            this.Message    = message;
        }
    }

    public class HTTPPPReplayMakeLinkRequest : IConsistentHashable
    {
        public string   UserID          { get; private set; }
        public string   Token           { get; private set; }
        public int      GameID          { get; private set; }
        public int      EnvID           { get; private set; }
        public string   Symbol          { get; private set; }
        public long     RoundID         { get; private set; }
        public string   Lang            { get; private set; }
        public object   ConsistentHashKey => this.UserID;

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
        public string   UserID      { get; private set; }
        public string   Token       { get; private set; }
        public int      GameID      { get; private set; }
        public long     RoundID     { get; private set; }

        public object ConsistentHashKey => this.UserID;
        public HTTPPPReplayDataRequest(string strUserID, string strToken, int gameID, long roundID)
        {
            this.UserID     = strUserID;
            this.Token      = strToken;
            this.GameID     = gameID;
            this.RoundID    = roundID;
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
        public string   RoundID     { get; private set; }
        public HTTPPPHistoryGetItemDetailRequest(string strUserID, string strToken, int gameID, string roundID)
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
        public object   ConsistentHashKey => UserID;
        public HTTPPPReplayListRequest(string strUserID, string strToken, int gameID)
        {
            this.UserID = strUserID;
            this.Token  = strToken;
            this.GameID = gameID;
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

    public class CQ9SearchRoundRequest : IConsistentHashable
    {
        public string GlobalUserID  { get; private set; }
        public string RoundID       { get; private set; }

        public CQ9SearchRoundRequest(string strUserID, string strRoundID)
        {
            this.GlobalUserID   = strUserID;
            this.RoundID        = strRoundID;
        }
        public object ConsistentHashKey
        {
            get
            {
                return GlobalUserID;
            }
        }
    }
    
    public class CQ9RoundListRequest : IConsistentHashable
    {
        public string   GlobalUserID    { get; private set; }
        public DateTime BeginTime       { get; private set; }
        public DateTime EndTime         { get; private set; }
        public int      Offset          { get; private set; }
        public int      Count           { get; private set; }

        public CQ9RoundListRequest(string strGlobalUserID, DateTime beginTime, DateTime endTime, int offset, int count)
        {
            this.GlobalUserID = strGlobalUserID;
            this.BeginTime = beginTime;
            this.EndTime = endTime;
            this.Offset = offset;
            this.Count = count;
        }
        public object ConsistentHashKey
        {
            get
            {
                return GlobalUserID;
            }
        }
    }
    
    public class CQ9RoundDetailRequest : IConsistentHashable
    {
        public string GlobalUserID  { get; private set; }
        public string RoundID       { get; private set; }

        public CQ9RoundDetailRequest(string strUserID, string strRoundID)
        {
            this.GlobalUserID   = strUserID;
            this.RoundID        = strRoundID;
        }
        public object ConsistentHashKey
        {
            get
            {
                return GlobalUserID;
            }
        }
    }
    
    public class BNGTransactionListRequest : IConsistentHashable
    {
        public int fetch_size { get; set; }
        public string fetch_state { get; set; }
        public string game_id { get; set; }
        public string player_id { get; set; }

        public object ConsistentHashKey
        {
            get
            {
                return player_id;
            }
        }
    }
    
    public class BNGAggregateRequest : IConsistentHashable
    {
        public string report_type { get; set; }
        public string game_id { get; set; }
        public string player_id { get; set; }

        public object ConsistentHashKey
        {
            get
            {
                return player_id;
            }
        }
    }
    
    public class BNGTransDetailRequest : IConsistentHashable
    {
        public string TransID { get; private set; }
        public object ConsistentHashKey
        {
            get
            {
                return TransID;
            }
        }
        public BNGTransDetailRequest(string strTransID)
        {
            this.TransID = strTransID;
        }
    }
    
    public class BNGTransDetailResponse
    {
        public string   Detail      { get; private set; }
        public int      BNGGameID   { get; private set; }
        public string   RoundID     { get; private set; }
        public string   GameName    { get; private set; }

        public string   DrawVersion { get; private set; }
        public BNGTransDetailResponse(int gameID, string strDetail, string strRoundID, string strGameName, string strDrawVersion)
        {
            this.BNGGameID = gameID;
            this.Detail = strDetail;
            this.RoundID = strRoundID;
            this.GameName = strGameName;
            this.DrawVersion    = strDrawVersion;
        }

    }
    
    public class BNGAggregateResponse
    {
        public string bets { get; set; }
        public string outcome { get; set; }
        public string payout { get; set; }
        public string profit { get; set; }
        public string rounds { get; set; }
        public string transactions { get; set; }
        public string wins { get; set; }

        public BNGAggregateResponse()
        {
            this.bets = "0";
            this.outcome = "0";
            this.payout = "0";
            this.profit = "0";
            this.rounds = "0";
            this.transactions = "0";
            this.wins = "0";

        }

    }
    
    public class HabaneroGetHistoryRequest : IConsistentHashable
    {
        public string   GlobalUserID    { get; private set; }
        public DateTime BeginTime       { get; private set; }
        public DateTime EndTime         { get; private set; }
        public string   Sort            { get; private set; }

        public HabaneroGetHistoryRequest(string strGlobalUserID, DateTime beginTime, DateTime endTime, int sort)
        {
            this.GlobalUserID   = strGlobalUserID;
            this.BeginTime      = beginTime;
            this.EndTime        = endTime;
            this.Sort           = (sort == 1) ? "DESC" : "ASC";
        }
        public object ConsistentHashKey
        {
            get
            {
                return GlobalUserID;
            }
        }
    }
    
    public class HabaneroGetGameDetailRequest : IConsistentHashable
    {
        public string   GameInstanceId      { get; private set; }

        public HabaneroGetGameDetailRequest(string gameinstanceid)
        {
            this.GameInstanceId = gameinstanceid;
        }
        public object ConsistentHashKey
        {
            get
            {
                return GameInstanceId;
            }
        }
    }

    public class PlaysonTransactionListRequest : IConsistentHashable
    {
        public int      fetch_size  { get; set; }
        public string   fetch_state { get; set; }
        public string   game_id     { get; set; }
        public string   player_id   { get; set; }

        public object ConsistentHashKey
        {
            get
            {
                return player_id;
            }
        }
    }
    public class PlaysonAggregateRequest : IConsistentHashable
    {
        public string report_type   { get; set; }
        public string game_id       { get; set; }
        public string player_id     { get; set; }

        public object ConsistentHashKey
        {
            get
            {
                return player_id;
            }
        }
    }
    public class PlaysonAggregateResponse
    {
        public string bets          { get; set; }
        public string outcome       { get; set; }
        public string payout        { get; set; }
        public string profit        { get; set; }
        public string rounds        { get; set; }
        public string transactions  { get; set; }
        public string wins          { get; set; }

        public PlaysonAggregateResponse()
        {
            this.bets           = "0";
            this.outcome        = "0";
            this.payout         = "0";
            this.profit         = "0";
            this.rounds         = "0";
            this.transactions   = "0";
            this.wins           = "0";
        }
    }
    public class PlaysonTransDetailRequest : IConsistentHashable
    {
        public string TransID { get; private set; }
        public object ConsistentHashKey
        {
            get
            {
                return TransID;
            }
        }
        public PlaysonTransDetailRequest(string strTransID)
        {
            this.TransID = strTransID;
        }
    }
    public class PlaysonTransDetailResponse
    {
        public string                       currency        { get; private set; }
        public string                       status          { get; private set; }
        public int                          totalcount      { get; private set; }
        public List<PlaysonTransDetailItem> data            { get; private set; }

        public PlaysonTransDetailResponse(int logTotalCnt,string logCurrency,string logStatus,List<PlaysonTransDetailItem> logData)
        {
            currency    = logCurrency;
            status      = logStatus;
            totalcount  = logTotalCnt;
            data        = logData;
        }
    }
    public class PlaysonTransDetailItem
    {
        public decimal  bet         { get; set; }
        public decimal  cash        { get; set; }
        public string   currency    { get; set; }
        public string   datetime    { get; set; }
        public string   entergame   { get; set; }
        public string   exitgame    { get; set; }
        public int      gameid      { get; set; }
        public string   gametitle   { get; set; }
        public long     id          { get; set; }
        public string   info        { get; set; }
        public string   platform    { get; set; }
        public string   roundnum    { get; set; }
        public string   totalcount  { get; set; }
        public string   userid      { get; set; }
        public long     win         { get; set; }
        public string   wlcode      { get; set; }
    }
    public class PlaysonGetTransRequest : IConsistentHashable
    {
        public string TransID { get; private set; }
        public object ConsistentHashKey
        {
            get
            {
                return TransID;
            }
        }
        public PlaysonGetTransRequest(string strTransID)
        {
            this.TransID = strTransID;
        }
    }
    public class PlaysonGetTransResponse
    {
        public string RoundId   { get; private set; }
        public PlaysonGetTransResponse(string roundId)
        {
            this.RoundId = roundId;
        }
    }

    #region Verify메시지
    public class HTTPPPVerifyGetLastItemRequest : IConsistentHashable
    {
        public string UserID { get; private set; }
        public string Token { get; private set; }
        public HTTPPPVerifyGetLastItemRequest(string strUserID, string strToken)
        {
            this.UserID = strUserID;
            this.Token = strToken;
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
        public string                   status              { get; set; }
        public double                   balance             { get; set; }
        public string                   currency            { get; set; }
        public string                   currencySymbol      { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public List<PPGameVerifyDetail> rounds              { get; set; }
        public PPGameVerifySettingItem  settings            { get; set; }
    }
    
    public class PPGameVerifyDetail
    {
        public long     id          { get; set; }
        public string   name        { get; set; }
        public string   symbol      { get; set; }
        public long     date        { get; set; }
        public double   betAmount   { get; set; }
    }
    
    public class PPGameVerifySettingItem
    {
        public bool displayRoundsEnabled { get; set; }
    }
    #endregion

    #region SendStringOnlyMessageToUser
    public class SendStringMessageToUser
    {
        public string message { get; private set; }
    }
    #endregion
}
