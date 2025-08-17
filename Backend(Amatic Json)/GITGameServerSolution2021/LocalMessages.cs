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
        public string           UserID      { get; private set; }
        public string           Password    { get; private set; }
        public string           IPAddress   { get; private set; }
        public PlatformTypes    Platform    { get; private set; }
        public UserLoginRequest(string strUserID, string strPassword, string strIPAddress, PlatformTypes platform)
        {
            this.UserID     = strUserID;
            this.Password   = strPassword;
            this.IPAddress  = strIPAddress;
            this.Platform   = platform;
        }
    }

    public class UserLoginResponse
    {
        public LoginResult  Result      { get; private set; }
        public string       UserToken   { get; private set; }
        public long         UserDBID    { get; private set; }
        public string       UserID      { get; private set; }
        public double       Balance     { get; private set; }
        public Currencies Currency    { get; private set; }
        public string       AgentName   { get; private set; }
        public int          AgentID     { get; private set; }
        public string       AgentIDs    { get; private set; }
        public long         LastScoreID { get; private set; }
        public string       IPAddress   { get; private set; }
        public string       Country     { get; private set; }
        public double       RollingFee  { get; private set; }
        public UserLoginResponse(LoginResult resultCode)
        {
            this.Result = resultCode;
        }

        public UserLoginResponse(long userDBID, string strUserID, string strUserToken, double balance, Currencies currency, string agentName, int agentID, string agentIDs, long lastScoreID, string ipAddress, string strCountry, double rollingFee)
        {
            this.Result         = LoginResult.OK;
            this.UserDBID       = userDBID;
            this.UserID         = strUserID;
            this.UserToken      = strUserToken;
            this.Balance        = balance;
            this.Currency       = currency;
            this.AgentName      = agentName;
            this.AgentID        = agentID;
            this.AgentIDs       = agentIDs;
            this.LastScoreID    = lastScoreID;
            this.IPAddress      = ipAddress;
            this.Country        = strCountry;
            this.RollingFee     = rollingFee;
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
            this.Connection     = connection;
            this.DBReader       = dbReader;
            this.DBWriter       = dbWriter;
            this.RedisWriter    = redisWriter;
            this.LoginResponse  = loginResponse;
            this.PlatformType   = platform;
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

    public class GetAgentRollingFees
    {
        public List<int> AgentIDs { get; private set; }

        public GetAgentRollingFees(List<int> agentIDs)
        {
            this.AgentIDs = agentIDs;
        }
    }

    public class GetUserBonusItems
    {
        public string UserID { get; private set; }
        public GetUserBonusItems(string strUserID)
        {
            this.UserID = strUserID;
        }
    }

    public class ClaimedUserRangeEventMessage
    {
        public long     ID              { get; private set; }
        public string   UserID          { get; private set; }
        public double   RewardedMoney   { get; private set; }
        public string   GameName        { get; private set; }
        public ClaimedUserRangeEventMessage(long id, string strUserID, double rewardedMoney, string strGameName)
        {
            this.ID             = id;
            this.UserID         = strUserID;
            this.RewardedMoney  = rewardedMoney;
            this.GameName       = strGameName;
        }
    }

    public class BaseBonusItem
    {
        public long     BonusID { get; protected set; }
        public string   UserID  { get; protected set; }

        public BaseBonusItem(long bonusID, string strUserID)
        {
            this.BonusID    = bonusID;
            this.UserID     = strUserID;
        }
    }
    
    public class ServerMaintenanceNotify
    {

    }

    public class UserRangeOddEventItem : BaseBonusItem
    {
        public double MinOdd { get; private set; }
        public double MaxOdd { get; private set; }
        public double MaxBet { get; private set; }
        public UserRangeOddEventItem(long bonusID, string strUserID, double minOdd, double maxOdd, double maxBet) : base(bonusID, strUserID)
        {
            this.MinOdd = minOdd;
            this.MaxOdd = maxOdd;
            this.MaxBet = maxBet;
        }
    }

    public class UserRollingPerUpdated
    {
        public string UserID { get; private set; }
        public double RollingPer { get; private set; }

        public UserRollingPerUpdated(string strUserID, double rollingPer)
        {
            this.UserID = strUserID;
            this.RollingPer = rollingPer;
        }
    }

    public class AgentRollingPerUpdated
    {
        public int AgentID { get; private set; }
        public double RollingPer { get; private set; }

        public AgentRollingPerUpdated(int agentID, double rollingPer)
        {
            this.AgentID = agentID;
            this.RollingPer = rollingPer;
        }
    }
    
    public class SetScoreData
    {
        public SetScoreData(long id, string strUserID, double score)
        {
            this.ID     = id;
            this.UserID = strUserID;
            this.Score  = score;
        }
        public long     ID      { get; set; }
        public string   UserID  { get; set; }
        public double   Score   { get; set; }
    }

    //유저강퇴메세지
    public class QuitUserMessage
    {
        public string UserID { get; private set; }
        public QuitUserMessage(string strUserID)
        {
            this.UserID = strUserID;
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

    //스코변경메세지
    public class AddScoreMessage
    {
        public string   UserID      { get; private set; }
        public long     ScoreID     { get; private set; }
        public double   AddedScore  { get; private set; }

        public AddScoreMessage(string strUserID, long scoreID, double addedScore)
        {
            this.UserID     = strUserID;
            this.ScoreID    = scoreID;
            this.AddedScore = addedScore;
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
            this.Connection = connection;
            this.Message    = message;
        }
    }

    #region Message related to HTTP Session    
    public class HTTPAuthResponse
    {
        public HttpAuthResults Result { get; private set; }
        public string SessionToken { get; private set; }

        public HTTPAuthResponse()
        {

        }
        public HTTPAuthResponse(HttpAuthResults result)
        {
            this.Result = result;
        }
        public HTTPAuthResponse(string strToken)
        {
            this.Result = HttpAuthResults.OK;
            this.SessionToken = strToken;
        }
    }
    public enum HttpAuthResults
    {
        OK = 0,
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
        public GameProviders     GameType        { get; private set; }
        public string       GameIdentifier  { get; private set; }

        public HTTPEnterGameRequest(string strUserID, string strSessionToken, GameProviders gameType, string strGameIdentifier)
        {
            this.UserID         = strUserID;
            this.SessionToken   = strSessionToken;
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
    #endregion
    
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
