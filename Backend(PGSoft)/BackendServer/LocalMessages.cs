using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Akka.Actor;
using GITProtocol;
using Akka.Routing;
using Newtonsoft.Json;
using System.Runtime;

namespace SlotGamesNode
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
        public UserLoginRequest(string strUserID, string strPassword)
        {
            this.UserID = strUserID;
            this.Password = strPassword;
        }
    }

    public class UserLoginResponse
    {
        public LoginResult  Result      { get; private set; }
        public long         UserDBID    { get; private set; }
        public string       UserID      { get; private set; }
        public double       Balance     { get; private set; }
        public UserLoginResponse(LoginResult resultCode)
        {
            this.Result = resultCode;
        }

        public UserLoginResponse(long userDBID, string strUserID, double balance)
        {
            this.Result     = LoginResult.OK;
            this.UserDBID   = userDBID;
            this.UserID     = strUserID;
            this.Balance    = balance;
        }
    }

    public class CreateNewUserMessage
    {
        public string       SessionToken   { get; private set; }
        public IActorRef    DBReader       { get; private set; }
        public IActorRef    DBWriter       { get; private set; }
        public IActorRef    RedisWriter    { get; private set; }
        public UserLoginResponse LoginResponse { get; private set; }

        public CreateNewUserMessage(string strSessionToken, IActorRef dbReader, IActorRef dbWriter, IActorRef redisWriter, UserLoginResponse loginResponse)
        {
            this.SessionToken   = strSessionToken;
            this.DBReader       = dbReader;
            this.DBWriter       = dbWriter;
            this.RedisWriter    = redisWriter;
            this.LoginResponse  = loginResponse;
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

    public class ClaimedGameJackpotBonus
    {
        public long ID { get; private set; }
        public string UserID { get; private set; }
        public int GameID { get; private set; }
        public ClaimedGameJackpotBonus(long id, int gameID, string strUserID)
        {
            this.ID = id;
            this.UserID = strUserID;
            this.GameID = gameID;
        }
    }

    public class BaseBonusItem
    {
        public long BonusID { get; protected set; }
        public string UserID { get; protected set; }

        public BaseBonusItem(long bonusID, string strUserID)
        {
            this.BonusID = bonusID;
            this.UserID = strUserID;
        }
    }
    public class GameJackpotBonusItem : BaseBonusItem
    {
        public double BonusMoney { get; private set; }
        public int BonusType { get; private set; }
        public GameJackpotBonusItem(long bonusID, string strUserID, double bonusMoney, int bonusType) : base(bonusID, strUserID)
        {
            this.BonusMoney = bonusMoney;
            this.BonusType = bonusType;
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
    public class HTTPOperatorVerifyRequest : IConsistentHashable
    {
        public string UserID        { get; private set; }
        public string PasswordMD5   { get; private set; }
        public int    GameID        { get; private set; }
        public HTTPOperatorVerifyRequest(string strUserID, string strPasswordMD5, int gameID)
        {
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
    public class HTTPVerifyRequest : IConsistentHashable
    {
        public string   UserID    { get; private set; }
        public string   Token     { get; private set; }
        public int      GameID    { get; private set; }
        public HTTPVerifyRequest(string strUserID, string strToken, int gameID)
        {
            this.UserID     = strUserID;
            this.Token      = strToken;
            this.GameID     = gameID;
        }
        public object ConsistentHashKey
        {
            get
            {
                return UserID;
            }
        }
    }

    public class HTTVerifyResponse
    {
        public HttpVerifyResults Result { get; private set; }
        public string   SessionToken    { get; private set; }
        public string   GameStringID    { get; private set; }
        public int      GameID          { get; private set; }
        public string   NickName        { get; private set; }
        public string   UserID          { get; private set; }

        public HTTVerifyResponse()
        {

        }
        public HTTVerifyResponse(HttpVerifyResults result)
        {
            this.Result = result;
        }
        public HTTVerifyResponse(string strToken, string gameStringID, int gameID, string strUserID, string strNickName)
        {
            this.Result         = HttpVerifyResults.OK;
            this.SessionToken   = strToken;
            this.GameStringID   = gameStringID;
            this.GameID         = gameID;
            this.NickName       = strNickName;
            this.UserID         = strUserID;
        }
    }
    public enum HttpVerifyResults
    {
        OK = 0,
        IDPASSWORDERROR     = 1,
        INVALIDGAMEID       = 2,
        SERVERMAINTENANCE   = 3, 
    }

    
    public class HTTPEnterGameRequest : IConsistentHashable
    {
        public string       UserID          { get; private set; }
        public string       SessionToken    { get; private set; }
        public string       GameIdentifier  { get; private set; }

        public HTTPEnterGameRequest(string strUserID, string strSessionToken, string strGameIdentifier)
        {
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
        public string       UserID          { get; private set; }
        public string       SessionToken    { get; private set; }
        public GITMessage   Message         { get; private set; }

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

    public enum ToHTTPSessionMsgResults
    {
        OK = 0,
        INVALIDTOKEN = 1,
        INVALIDACTION = 2,
    }
    public class ToHTTPSessionMessage
    {
        public ToHTTPSessionMsgResults Result   { get; private set; }
        public object                  Response { get; private set; }
        public ToHTTPSessionMessage(ToHTTPSessionMsgResults result)
        {
            this.Result = result;
        }

        public ToHTTPSessionMessage()
        {

        }

        public ToHTTPSessionMessage(object response)
        {
            this.Result     = ToHTTPSessionMsgResults.OK;
            this.Response   = response;
        }
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
    public class PGBetSummaryRequest : IConsistentHashable
    {
        public string   UserID      { get; private set; }
        public int      GameID      { get; private set; }
        public long     StartTime   { get; private set; }
        public long     EndTime     { get; private set; }
        public object ConsistentHashKey
        {
            get
            {
                return UserID;
            }
        }
        public PGBetSummaryRequest(string userID, int gameID, long startTime, long endTime)
        {
            UserID = userID;
            GameID = gameID;
            StartTime = startTime;
            EndTime = endTime;
        }
    }
    public class PGBetHistoryRequest : IConsistentHashable
    {
        public string   UserID          { get; private set; }
        public int      GameID          { get; private set; }
        public long     StartTime       { get; private set; }
        public long     EndTime         { get; private set; }
        public int      PageNumber      { get; private set; }
        public int      CountPerPage    { get; private set; }
        public object ConsistentHashKey
        {
            get
            {
                return UserID;
            }
        }
        public PGBetHistoryRequest(string userID, int gameID, long startTime, long endTime, int pageNumber, int countPerPage)
        {
            UserID = userID;
            GameID = gameID;
            StartTime = startTime;
            EndTime = endTime;
            PageNumber = pageNumber;
            CountPerPage = countPerPage;
        }
    }
    
   
    public class HttpSessionAdded
    {
        public string SessionToken { get; private set; }
        public HttpSessionAdded(string strSessionToken)
        {
            this.SessionToken = strSessionToken;
        }
    }
    public class CloseHttpSession
    {
        public string SessionToken { get; private set; }
        public CloseHttpSession(string strSessionToken)
        {
            this.SessionToken   = strSessionToken;
        }
    }
    public class HttpSessionClosed
    {
        public string SessionToken { get; private set; }
        public HttpSessionClosed(string strSessionToken)
        {
            this.SessionToken = strSessionToken;
        }
    }
    public enum UserBonusType
    {
        GAMEJACKPOT = 0,
    }
    public class UserBonus
    {
        public UserBonusType BonusType { get; protected set; }
        public long BonusID { get; protected set; }
    }
    public class UserGameJackpotBonus : UserBonus
    {
        public byte JackpotType { get; private set; }
        public double BonusMoney { get; private set; }
        public UserGameJackpotBonus()
        {

        }
        public UserGameJackpotBonus(long bonusID, byte jackpotType, double bonusMoney)
        {
            this.BonusID = bonusID;
            this.JackpotType = jackpotType;
            this.BonusMoney = bonusMoney;
            this.BonusType = UserBonusType.GAMEJACKPOT;
        }
    }
    public class ToUserMessage
    {
        public int GameID { get; private set; }
        public List<GITMessage> Messages { get; private set; }
        public bool IsCountAsSpin { get; private set; }
        public bool IsRewardedBonus { get; private set; }
        public double RewardBonusMoney { get; private set; }

        public void addMessage(GITMessage message)
        {
            if (message != null)
                Messages.Add(message);
        }

        public void insertFirstMessage(GITMessage message)
        {
            if (message == null)
                return;

            Messages.Insert(0, message);
        }

        public ToUserMessage()
        {
            IsRewardedBonus = false;
            RewardBonusMoney = 0.0;
        }
        public ToUserMessage(int gameID, GITMessage message)
        {
            this.GameID = gameID;
            this.Messages = new List<GITMessage>();
            if (message != null)
                this.Messages.Add(message);

            IsRewardedBonus = false;
            RewardBonusMoney = 0.0;
        }
        public void setBonusReward(double bonusMoney)
        {
            this.IsRewardedBonus = true;
            this.RewardBonusMoney = bonusMoney;
        }
        public void setCountAsSpin(bool isCountAsSpin)
        {
            this.IsCountAsSpin = isCountAsSpin;
        }
    }
    public class GameLogInfo
    {
        public string GameName { get; private set; }
        public string TableName { get; private set; }
        public string LogString { get; private set; }
        public GameLogInfo()
        {

        }
        public GameLogInfo(string strGameName, string strTableName, string strGameLog)
        {
            this.GameName = strGameName;
            this.TableName = strTableName;
            this.LogString = strGameLog;
        }
    }
    public class ToUserResultMessage : ToUserMessage
    {
        public double       BetMoney { get; private set; }
        public double       WinMoney { get; private set; }
        public double       TurnOver { get; protected set; }
        public GameLogInfo GameLog { get; private set; }
        public ToUserResultMessage()
        {
        }

        public ToUserResultMessage(int gameID, GITMessage message, double betMoney, double winMoney, GameLogInfo gameLog, double turnOver = -1.0) : base(gameID, message)
        {
            this.BetMoney = betMoney;
            this.WinMoney = winMoney;
            this.GameLog = gameLog;

            if (turnOver == -1.0)
                this.TurnOver = betMoney;
            else
                this.TurnOver = turnOver;
        }
    }
    public class FromUserMessage
    {
        public string       UserID      { get; private set; }
        public GITMessage   Message     { get; private set; }
        public UserBonus    Bonus       { get; private set; }
        public double       UserBalance { get; private set; }
        public IActorRef    UserActor   { get; private set; } 
        public FromUserMessage(string strUserID, double userBalance, IActorRef userActor, GITMessage message, UserBonus bonus)
        {
            this.UserID         = strUserID;
            this.UserBalance    = userBalance;
            this.UserActor      = userActor;
            this.Message        = message;
            this.Bonus          = bonus;
        }
    }

    public class EnterGameRequest
    {
        public string       UserID      { get; private set; }
        public int          GameID      { get; private set; }
        public bool         NewEnter    { get; private set; }
        public IActorRef    UserActor   { get; private set; }
        public double       UserBalance { get; private set; }
        public EnterGameRequest(int gameID, string userID, IActorRef userActor, double userBalance, bool newEnter = true)
        {
            this.GameID         = gameID;
            this.UserID         = userID;
            this.NewEnter       = newEnter;
            this.UserBalance    = userBalance;
            this.UserActor      = userActor;
        }
    }
    public class EnterGameResponse
    {
        public      IActorRef   GameActor   { get; private set; }
        public GAMEID           GameID      { get; private set; }
        public int              Ack         { get; private set; }            
        public string           GameConfig  { get; private set; }
        public string           LastResult  { get; private set; }
        public EnterGameResponse(GAMEID gameID, IActorRef gameActor, int ack, string strGameConfig, string strLastResult)
        {
            this.GameID         = gameID;
            this.Ack            = ack;
            this.GameActor      = gameActor;
            this.GameConfig     = strGameConfig;
            this.LastResult     = strLastResult;
        }
    }
    public class ExitGameRequest
    {
        public string   UserID          { get; private set; }
        public double   Balance         { get; private set; }
        public bool     UserRequested   { get; private set; }  

        public ExitGameRequest(string userID, double balance, bool userRequested)
        {
            this.UserID         = userID;
            this.Balance        = balance;
            this.UserRequested  = userRequested;
        }
    }
    public class ExitGameResponse
    {

    }

}
