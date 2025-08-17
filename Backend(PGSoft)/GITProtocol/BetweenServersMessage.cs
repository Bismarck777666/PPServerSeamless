using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Akka.Actor;
using Akka.Routing;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;


/**
 * 
 *      Created by Foresight(2021.03.03)
 *      솔루션의 서버노드들사이에 교환되는 메세지들
 *
 */

namespace GITProtocol
{
    public class CreateNewUserMessage
    {
        public string       SessionToken        { get; private set; }
        public long         UserDBID            { get; private set; }
        public string       UserID              { get; private set; }
        public double       UserBalance         { get; private set; }
        public string       PassToken           { get; private set; }
        public string       AgentID             { get; private set; }
        public int          AgentDBID           { get; private set; }
        public long         LastScoreCounter    { get; private set; }
        public Currencies   Currency            { get; private set; }
        public bool         IsAffiliate         { get; private set; }

        public CreateNewUserMessage(string sessionToken, long userDBID, string strUserID, double userBalance, string passToken,
            int agentDBID, string agentID, long lastScoreCounter, Currencies currency, bool isAffiliate)
        {
            this.SessionToken       = sessionToken;
            this.UserID             = strUserID;
            this.UserDBID           = userDBID;
            this.UserBalance        = userBalance;
            this.PassToken          = passToken;
            this.AgentDBID          = agentDBID;
            this.LastScoreCounter   = lastScoreCounter;
            this.Currency           = currency;
            this.AgentID            = agentID;
            this.IsAffiliate        = isAffiliate;
        }
        public string GlobalUserID
        {
            get { return string.Format("{0}_{1}", this.AgentDBID, this.UserID); }
        }
    }
    public class FromConnRevMessage
    {
        public string SessionToken { get; private set; }
        public GITMessage Message { get; private set; }

        public FromConnRevMessage(string token, GITMessage message)
        {
            this.SessionToken = token;
            this.Message = message;
        }
    }

    public class SendMessageToUser
    {
        public GITMessage Message { get; private set; }
        public double Balance { get; private set; }
        public double Delay { get; private set; }
        public SendMessageToUser(GITMessage message, double balance, double delay)
        {
            this.Message = message;
            this.Balance = balance;
            this.Delay = delay;
        }
    }

    //게임입장요청메세지
    public class EnterGameRequest
    {
        public int          AgentID     { get; private set; }
        public string       UserID      { get; private set; }
        public int          GameID      { get; private set; }
        public bool         NewEnter    { get; private set; }
        public IActorRef    UserActor   { get; private set; }
        public double       UserBalance { get; private set; }
        public Currencies   Currency    { get; private set; }   //유저의 화페단위

        public EnterGameRequest(int gameID, int agentID, string userID, IActorRef userActor, double userBalance, Currencies currency, bool newEnter = true)
        {
            this.AgentID     = agentID;
            this.GameID      = gameID;
            this.UserID      = userID;
            this.NewEnter    = newEnter;
            this.UserActor   = userActor;
            this.UserBalance = userBalance;
            this.Currency    = currency;
        }
    }

    //게임입장응답메세지
    public class EnterGameResponse
    {
        public IActorRef    GameActor       { get; private set; }
        public GAMEID       GameID          { get; private set; }
        public int          Ack             { get; private set; }       //0: 입장성공, 기타: 입장실패
        public string       GameConfig      { get; private set; }
        public string       LastResult      { get; private set; }

        public EnterGameResponse(GAMEID gameID, IActorRef gameActor, int ack, string strGameConfig, string strLastResult)
        {
            this.GameID     = gameID;
            this.Ack        = ack;
            this.GameActor  = gameActor;
            this.GameConfig = strGameConfig;
            this.LastResult = strLastResult;
        }
    }

    //게임탈퇴요청메세지
    public class ExitGameRequest
    {
        public string   UserID           { get; private set; }
        public int      WebsiteID        { get; private set; }
        public double   Balance          { get; private set; }
        public bool     UserRequested    { get; private set; }       //유저요청에 의한것인가? 아님 게임서버노드의 shutdown으로 인한것인가?
        public bool     IsNewServerReady { get; private set; }

        public ExitGameRequest(string userID, int websiteID, double balance, bool userRequested, bool isNewServerReady)
        {
            this.UserID             = userID;
            this.WebsiteID          = websiteID;
            this.Balance            = balance;
            this.UserRequested      = userRequested;
            this.IsNewServerReady   = isNewServerReady;
        }
    }

    //게임탙퇴응답메세지
    public class ExitGameResponse
    {

    }
    public class FromUserMessage
    {
        public string       UserID          { get; private set; }   //유저아이디
        public int          WebsiteID       { get; private set; }   //웹사이트아이디
        public GITMessage   Message         { get; private set; }   //클라에서 보낸 메세지
        public UserBonus    Bonus           { get; private set; }   //유저에게 할당된 보너스정보
        public double       UserBalance     { get; private set; }   //유저잔고
        public IActorRef    UserActor       { get; private set; }   //유저액터
        public Currencies   Currency        { get; private set; }   //유저의 화페단위
        public bool         IsAffiliate     { get; private set; }
        public FromUserMessage(string strUserID, int websiteID, double userBalance,IActorRef userActor, GITMessage message,UserBonus bonus, Currencies currency, bool isAffiliate)
        {
            this.UserID         = strUserID;
            this.WebsiteID      = websiteID;
            this.UserBalance    = userBalance;
            this.UserActor      = userActor;
            this.Message        = message;
            this.Bonus          = bonus;
            this.Currency       = currency;
            this.IsAffiliate    = isAffiliate;
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

    public class ToUserMessage
    {
        public int              GameID              { get; private set; }
        public List<GITMessage> Messages            { get; private set; }
        public bool             IsRewardedBonus     { get; private set; }
        public double           RewardBonusMoney    { get; private set; }
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
        public void setBonusReward(double bonusMoney)
        {
            this.IsRewardedBonus    = true;
            this.RewardBonusMoney   = bonusMoney;
        }

        public ToUserMessage()
        {
            IsRewardedBonus     = false;
            RewardBonusMoney    = 0.0;
        }
        public ToUserMessage(int gameID, GITMessage message)
        {
            this.GameID     = gameID;
            this.Messages   = new List<GITMessage>();
            if (message != null)
                this.Messages.Add(message);

            IsRewardedBonus     = false;
            RewardBonusMoney    = 0.0;
        }
    }

    public enum UserBetTypes
    {
        Normal          = 0,
        PurchaseFree    = 1,
        AnteBet         = 2,
    }
    public class ToUserResultMessage : ToUserMessage
    {
        public double       BetMoney            { get; private set; }
        public double       WinMoney            { get; private set; }
        public double       TurnOver            { get; protected set; }
        public GameLogInfo  GameLog             { get; private set; }
        public UserBetTypes BetType             { get; private set; }        
        public string       BetTransactionID    { get; set; }
        public string       TransactionID       { get; set; }
        public string       RoundID             { get; set; }
        public bool         EndTransaction      { get; set; }
        public PGGameHistoryDBItem HistoryItem  { get; set; }

        public ToUserResultMessage()
        {
            this.EndTransaction = false;
            this.HistoryItem    = null;
        }

        public ToUserResultMessage(int gameID, GITMessage message, double betMoney, double winMoney, GameLogInfo gameLog, UserBetTypes betType, double turnOver = -1.0) : base(gameID, message)
        {
            this.BetMoney = betMoney;
            this.WinMoney = winMoney;
            this.GameLog = gameLog;
            this.BetType = betType;
            if (turnOver == -1.0)
                this.TurnOver = betMoney;
            else
                this.TurnOver = turnOver;
            this.EndTransaction = false;
        }
    }

    public class ToUserSpecialResultMessage : ToUserResultMessage
    {
        public double RealBet { get; set; }
        public bool IsJustBet { get; set; }

        public ToUserSpecialResultMessage()
        {

        }
        public ToUserSpecialResultMessage(int gameID, GITMessage message, double betMoney) : base(gameID, message, 0.0, 0.0, null, UserBetTypes.Normal)
        {
            this.IsJustBet = true;
            this.RealBet = betMoney;
            this.TurnOver = betMoney;
        }

        public ToUserSpecialResultMessage(int gameID, GITMessage message, double realBet, double betMoney, double winMoney, GameLogInfo gameLog, double turnOver = -1.0) : base(gameID, message, betMoney, winMoney, gameLog, UserBetTypes.Normal, turnOver)
        {
            this.RealBet = realBet;
            this.IsJustBet = false;
            this.TurnOver = realBet;
        }
    }
    public enum UserBonusType
    {
        GAMEJACKPOT = 0,
        USEREVENT = 1,
        REPACKET = 2,
        RACEPRIZE = 3,

    }
    public class UserBonus
    {
        public UserBonusType BonusType  { get; protected set; }
        public long BonusID             { get; protected set; }
    }
    public class UserRangeOddEventBonus : UserBonus
    {
        public double MinOdd { get; private set; }
        public double MaxOdd { get; private set; }
        public double MaxBet { get; private set; }
        public UserRangeOddEventBonus(long bonusID, double minOdd, double maxOdd, double maxBet)
        {
            this.BonusID = bonusID;
            this.MinOdd = minOdd;
            this.MaxOdd = maxOdd;
            this.MaxBet = maxBet;
            this.BonusType = UserBonusType.USEREVENT;
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
            this.SessionToken = strSessionToken;
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
    public class SlotsNodeShuttingDownMsg
    {

    }
    public class ForceLogoutMesssage
    {
        public int    AgentID { get; private set; }
        public string UserID  { get; private set; }

        public string GlobalUserID
        {
            get
            {
                return string.Format("{0}_{1}", AgentID, UserID);
            }
        }
        public ForceLogoutMesssage(string strUserID)
        {
            this.UserID = strUserID;
        }
    }
    public class SubtractEventMoneyRequest : IConsistentHashable
    {
        public int AgentID { get; private set; }
        public string UserID { get; private set; }
        public double EventMoney { get; private set; }
        public object ConsistentHashKey
        {
            get
            {
                return AgentID;
            }
        }
        public SubtractEventMoneyRequest(int websiteID, string strUserID, double eventMoney)
        {
            this.AgentID = websiteID;
            this.UserID = strUserID;
            this.EventMoney = eventMoney;
        }
    }
    public class AddEventLeftMoneyRequest : IConsistentHashable
    {
        public int      AgentID   { get; private set; }
        public string   UserID      { get; private set; }
        public double   LeftMoney   { get; private set; }

        public object ConsistentHashKey
        {
            get
            {
                return AgentID;
            }
        }

        public AddEventLeftMoneyRequest(int websiteID, string strUserID, double leftMoney)
        {
            this.AgentID  = websiteID;
            this.UserID     = strUserID;
            this.LeftMoney  = leftMoney;
        }
    }

    public class UserSpinItem : IConsistentHashable
    {
        public int      AgentID         { get; private set; }
        public string   UserID          { get; private set; }
        public string   Symbol          { get; private set; }
        public double   BetPerLine      { get; private set; }
        public int      LineCount       { get; private set; }
        public int      MoreBetID       { get; private set; } 
        public int      PurchaseID      { get; private set; }
        public double   BetMoney        { get; private set; }

        public UserSpinItem(int agentID, string strUserID, string symbol, double betPerLine, int lineCount, int moreBetID, int purchaseID, double betMoney)
        {
            AgentID     = agentID;
            UserID      = strUserID;
            Symbol      = symbol;
            BetPerLine  = betPerLine;
            LineCount   = lineCount;
            MoreBetID   = moreBetID;
            PurchaseID  = purchaseID;
            BetMoney    = betMoney;
        }
        public object ConsistentHashKey
        {
            get
            {
                return string.Format("{0}_{1}", AgentID, UserID);
            }
        }
    }

    public class UserProductCertRequest : IConsistentHashable
    {
        public int      AgentID { get; private set; }
        public string   UserID  { get; private set; }

        public object ConsistentHashKey
        {
            get
            {
                return string.Format("{0}_{1}", AgentID, UserID);
            }
        }

        public UserProductCertRequest(int agentID, string userID)
        {
            AgentID = agentID;
            UserID  = userID;
        }
    }
    public class UserEnteredGameItem : IConsistentHashable
    {
        public int      AgentID     { get; private set; }
        public string   UserID      { get; private set; }        
        public string   GameSymbol  { get; private set; }
        public object ConsistentHashKey
        {
            get
            {
                return string.Format("{0}_{1}", AgentID, UserID);
            }
        }

        public UserEnteredGameItem(int agentID, string userID, string gameSymbol)
        {
            AgentID     = agentID;
            UserID      = userID;
            GameSymbol  = gameSymbol;
        }
    }

    public class UserVerifyRequest
    {
        public string Token         { get; private set; }
        public int    GameID        { get; private set; }
        public string GameString    { get; private set; }

        public UserVerifyRequest(string token, int gameID, string gameString)
        {
            Token       = token;
            GameID      = gameID;
            GameString  = gameString;
        }
    }
    public class HTTPVerifyRequest : IConsistentHashable
    {
        public int      AgentID { get; private set; }
        public string   UserID  { get; private set; }
        public string   Token   { get; private set; }
        public int      GameID  { get; private set; }
        public HTTPVerifyRequest(int agentID, string strUserID, string strToken, int gameID)
        {
            this.UserID = strUserID;
            this.AgentID = agentID;
            this.Token = strToken;
            this.GameID = gameID;
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
        public string SessionToken { get; private set; }
        public string GameStringID { get; private set; }
        public int GameID { get; private set; }
        public string NickName { get; private set; }
        public string UserID { get; private set; }
        public string Currency { get; private set; }

        public HTTVerifyResponse()
        {

        }
        public HTTVerifyResponse(HttpVerifyResults result)
        {
            this.Result = result;
        }
        public HTTVerifyResponse(string strToken, string gameStringID, int gameID, string strUserID, string strNickName, string currency)
        {
            this.Result         = HttpVerifyResults.OK;
            this.SessionToken   = strToken;
            this.GameStringID   = gameStringID;
            this.GameID         = gameID;
            this.NickName       = strNickName;
            this.UserID         = strUserID;
            this.Currency       = currency;
        }
    }
    public enum HttpVerifyResults
    {
        OK = 0,
        IDPASSWORDERROR = 1,
        INVALIDGAMEID = 2,
        SERVERMAINTENANCE = 3,
    }

    public enum Currencies
    {
        BRL     = 0,    //1:1
        THB     = 1,    //5:1
        USD     = 2,    //1:1
        EUR     = 3,    //1:1
        TND     = 4,    //1:1
        TRY     = 5,    //10:1
        MYR     = 6,    //1:1
        IDR     = 7,    //1000:1
        RT      = 8,    //5:1
        JC      = 9,    //5:1
        GC      = 10,   //5:1
        CHF     = 11,   //1:1
        MAD     = 12,   //1:1
        AED     = 13,   //1:1
        SYP     = 14,   //1000:1
        IQD     = 15,   //1000:1
        ARS     = 16,   //1000:1
        EGP     = 17,   //10:1
        BWP     = 18,   //10:1
        ZAR     = 19,   //10:1
        RUB     = 20,   //10:1
        UAH     = 21,   //10:1
        NZD     = 22,   //1:1 
        AUD     = 23,   //1:1
        CAD     = 24,   //1:1
        DKK     = 25,   //1:1
        SEK     = 26,   //1:1
        RON     = 27,   //1:1
        ILS     = 28,   //1:1
        NAD     = 29,   //1:1
        GHS     = 30,   //1:1
        PEN     = 31,   //1:1
        SZL     = 32,   //1:1
        MXN     = 33,   //1:1
        HKD     = 34,   //1:1
        SGD     = 35,   //1:1
        PHP     = 36,   //1:1
        AZN     = 37,   //1:1
        GBP     = 38,   //1:1
        BOB     = 39,   //1:1
        GEL     = 40,   //1:1
        KWD     = 41,   //1:1
        PLN     = 42,   //1:1
        TMT     = 43,   //1:1
        TVD     = 44,   //1:1
        BAM     = 45,   //1:1
        LBP     = 46,   //10000:1
        MNT     = 47,   //500:1
        INR     = 48,   //50:1
        JPY     = 49,   //5:1
        MMK     = 50,   //500:1
        KRW     = 51,   //500:1
        VND     = 52,   //5000:1
        LKR     = 53,   //10:1
        KZT     = 54,   //50:1
        PKR     = 55,   //50:1
        KES     = 56,   //50:1
        BYN     = 57,   //1:1


        COUNT   = 58,
    }

    public enum Languages
    {
        EN    = 0,
        FR    = 1,
        DE    = 2,
        IT    = 3,
        KO    = 4,
        TR    = 5,
        COUNT = 6,
    }

    public class WinThresolds
    {
        public double bw { get; set; }      //bigWin
        public double mgw { get; set; }     //megaWin
        public double mw { get; set; }      //mediumWin
        public double smgw { get; set; }    //superMegaWin

    }

    public class FeatureBuyConfig
    {
        public int bm { get; set; } //betMultiplier

        [JsonProperty(PropertyName = "is")]
        public bool isSupported { get; set; } //isSupported

        public int t { get; set; } //thresold
    }
    public class GSCConfig
    {
        public AnteBetConfig ab {  get; set; }

        public GSCConfig()
        {
            this.ab = new AnteBetConfig();
        }
    }
    public class AnteBetConfig
    {
        public int          bm          { get; set; }   //betMultiplier
        public List<double> bms         { get; set; }   //betMultipliers

        [JsonProperty(PropertyName = "is")]
        public bool         isSupported { get; set; }   //isSupported
        public int          t           { get; set; }   //thresold
    }
    public class PGGameConfig
    {
        public List<int>        ml      { get; set; }   //level list
        public List<double>     cs      { get; set; }   //betSizeList
        public int              mxl     { get; set; }   //max line number
        public bool             inwe    { get; set; }   //wallet notification
        public bool             iuwe    { get; set; }   //WalletReminder
        public WinThresolds     wt      { get; set; }   //winThresholdFactor
        public FeatureBuyConfig fb      { get; set; }   //FeatureBuy
        public GSCConfig        gcs     { get; set; }   //AnteBet

        public PGGameConfig()
        {
            this.ml     = new List<int>();
            this.cs     = new List<double>();
            this.wt     = new WinThresolds();
            this.fb     = new FeatureBuyConfig();
            this.gcs    = new GSCConfig();
        }
    }
    public class ErrorMsgData
    {
        public string cd { get; set; }
        public string msg { get; set; }
        public string tid { get; set; }

        public ErrorMsgData(int cd, string msg, string tid)
        {
            this.cd = cd.ToString();
            this.msg = msg;
            this.tid = tid;
        }
        public ErrorMsgData()
        {

        }
    }
    public class GetGameNameResponse
    {
        public Dictionary<string, string>   dt  { get; set; }
        public ErrorMsgData                 err { get; set; }

        public GetGameNameResponse()
        {

        }
        public GetGameNameResponse(Dictionary<string, string> dt)
        {
            this.dt = dt;
            this.err = null;
        }
        public GetGameNameResponse(int errorCode, string strErrMsg, string tid)
        {
            this.dt = null;
            this.err = new ErrorMsgData(errorCode, strErrMsg, tid);
        }
    }

    public class GetGameWalletResponseData
    {
        public string       dt  { get; set; }
        public ErrorMsgData err { get; set; }
        public GetGameWalletResponseData(string dt)
        {
            this.dt = dt;
            this.err = null;
        }
        public GetGameWalletResponseData()
        {

        }
    }
    public class GetByResourcesResponseData
    {
        public string       dt  { get; set; }
        public ErrorMsgData err { get; set; }

        public GetByResourcesResponseData(string dt)
        {
            this.dt     = dt;
            this.err    = null;
        }
        public GetByResourcesResponseData()
        {

        }
    }
    public class GetBetHistoryResponseData
    {
        public List<dynamic> bh { get; set; }

        public GetBetHistoryResponseData()
        {
            this.bh = new List<dynamic>();
        }
    }
    public class GetBetHistoryResponse
    {
        public GetBetHistoryResponseData dt  { get; set; }
        public ErrorMsgData              err { get; set; }

        public GetBetHistoryResponse(GetBetHistoryResponseData dt)
        {
            this.dt = dt;
            this.err = null;
        }
        public GetBetHistoryResponse(int errorCode, string strErrMsg, string tid)
        {
            this.dt = null;
            this.err = new ErrorMsgData(errorCode, strErrMsg, tid);
        }
        public GetBetHistoryResponse()
        {

        }
    }
    public class GetBetHistoryResponseMessage
    {
        public string Data { get; set; }

        public GetBetHistoryResponseMessage(string data)
        {
            this.Data = data;
        }
    }
    public class GetBetHistoryDetailResponseData
    {
        public dynamic bh { get; set; }

        public GetBetHistoryDetailResponseData()
        {
            this.bh = new JObject();
        }
    }
    public class GetBetHistoryDetailResponse
    {
        public GetBetHistoryDetailResponseData  dt  { get; set; }
        public ErrorMsgData                     err { get; set; }

        public GetBetHistoryDetailResponse(GetBetHistoryDetailResponseData dt)
        {
            this.dt     = dt;
            this.err    = null;
        }
        public GetBetHistoryDetailResponse(int errorCode, string strErrMsg, string tid)
        {
            this.dt = null;
            this.err = new ErrorMsgData(errorCode, strErrMsg, tid);
        }
        public GetBetHistoryDetailResponse()
        {

        }
    }
    public class GetBetHistoryDetailResponseMessage
    {
        public string Data { get; set; }

        public GetBetHistoryDetailResponseMessage(string data)
        {
            this.Data = data;
        }
    }
    public class GetBetSummaryResponse
    {
        public GetBetSummaryResponseData dt { get; set; }
        public ErrorMsgData err { get; set; }
        public GetBetSummaryResponse(GetBetSummaryResponseData dt)
        {
            this.dt = dt;
            this.err = null;
        }
        public GetBetSummaryResponse(int errorCode, string strErrMsg, string tid)
        {
            this.dt = null;
            this.err = new ErrorMsgData(errorCode, strErrMsg, tid);
        }
        public GetBetSummaryResponse()
        { 
        }

    }
    public class GetBetSummaryResponseData
    {
        public BetSummaryData bs { get; set; }
        public long lut { get; set; }   //last unix timestamp
    }
    public class BetSummaryData
    {
        public int      bc          { get; set; } //betCount
        public double   btba        { get; set; } //batchTotalBetAmount
        public double   btwla       { get; set; } //batchTotalWinLossAmount
        public int      gid         { get; set; }
        public long     lbid        { get; set; } //lastBetId
    }
    public class SpinResponseData
    {
        public string       dt  { get; set; }
        public ErrorMsgData err { get; set; }

        public SpinResponseData(int errorCode, string strErrMsg, string tid)
        {
            this.dt     = null;
            this.err    = new ErrorMsgData(errorCode, strErrMsg, tid);
        }
        public SpinResponseData(string dt)
        {
            this.dt = dt;
            this.err = null;
        }
        public SpinResponseData()
        {

        }

    }
    public class SpinResponse
    {
        public dynamic dt           { get; set; }
        public ErrorMsgData err     { get; set; }

        public SpinResponse(int errorCode, string strErrMsg, string tid)
        {
            this.dt = null;
            this.err = new ErrorMsgData(errorCode, strErrMsg, tid);
        }
        public SpinResponse(dynamic dt)
        {
            this.dt = dt;
            this.err = null;
        }
        public SpinResponse()
        {

        }
    }
    public enum HTTPEnterGameResults
    {
        OK = 0,
        INVALIDTOKEN = 1,
        INVALIDGAMEID = 2,
        INVALIDACTION = 3,
    }
    public class HTTPEnterGameResponse
    {
        public HTTPEnterGameResults Result      { get; set; }
        public PGGameConfig         GameConfig  { get; set; }
        public string               LastResult  { get; set; }
        public double               Balance     { get; set; }
        public Currencies           Currency    { get; set; }

        public HTTPEnterGameResponse()
        {

        }
        public HTTPEnterGameResponse(HTTPEnterGameResults result)
        {
            this.Result = result;
        }
        public HTTPEnterGameResponse(PGGameConfig config, string result, double balance, Currencies currency)
        {
            this.Result     = HTTPEnterGameResults.OK;
            this.LastResult = result;
            this.Balance    = balance;
            this.GameConfig = config;
            this.Currency   = currency;
        }
    }
    public class GetGameInfoResponse
    {
        public GetGameInfoResponseData dt { get; set; }
        public ErrorMsgData err { get; set; }
        public GetGameInfoResponse(int errorCode, string strErrMsg, string tid)
        {
            this.dt = null;
            this.err = new ErrorMsgData(errorCode, strErrMsg, tid);
        }
        public GetGameInfoResponse(HTTPEnterGameResponse response)
        {
        
            this.dt         = new GetGameInfoResponseData();
            this.err        = null;
            this.dt.bl      = response.Balance;
            this.dt.cs      = response.GameConfig.cs;
            this.dt.fb      = response.GameConfig.fb;
            this.dt.gcs     = response.GameConfig.gcs;
            this.dt.cc      = response.Currency.ToString();
            this.dt.ml      = response.GameConfig.ml;
            this.dt.ls      = JsonConvert.DeserializeObject<dynamic>(response.LastResult);
            this.dt.inwe    = response.GameConfig.inwe;
            this.dt.iuwe    = response.GameConfig.iuwe;
            this.dt.mxl     = response.GameConfig.mxl;
            this.dt.wt      = response.GameConfig.wt;
            this.err        = null;
        }
        public GetGameInfoResponse()
        {

        }

    }
    public class GetGameInfoResponseData
    {
        public double           bl      { get; set; }
        public string           cc      { get; set; }
        public dynamic          ls      { get; set; } //last result
        public List<int>        ml      { get; set; } //level list
        public List<double>     cs      { get; set; } //betSizeList
        public int              mxl     { get; set; } //max line number
        public bool             inwe    { get; set; } //wallet notification
        public bool             iuwe    { get; set; } //WalletReminder
        public WinThresolds     wt      { get; set; } //winThresholdFactor
        public FeatureBuyConfig fb      { get; set; }
        public GSCConfig        gcs     { get; set; }   //AnteBet
    }

    public class GetGameRuleResponse
    {
        public dynamic      dt { get; set; }
        public ErrorMsgData err { get; set; }

        public GetGameRuleResponse()
        {

        }
        public GetGameRuleResponse(dynamic dt)
        {
            this.dt = dt;
            this.err = null;
        }
        public GetGameRuleResponse(int errorCode, string strErrMsg, string tid)
        {
            this.dt = null;
            this.err = new ErrorMsgData(errorCode, strErrMsg, tid);
        }
    }
    public class PGGameHistoryDBItem
    {
        public int      AgentID         { get; set; }
        public string   UserID          { get; set; }
        public int      GameID          { get; set; }
        public double   Bet             { get; set; }
        public double   Profit          { get; set; }
        public long     TransactionID   { get; set; }
        public long     Timestamp       { get; set; }
        public string   Data            { get; set; }

        public PGGameHistoryDBItem()
        {

        }
        public PGGameHistoryDBItem(int agentID, string strUserName, int gameID, double bet, double profit, string transactionID, long timestamp, string strData)
        {
            this.AgentID    = agentID;
            this.UserID     = strUserName;
            this.GameID     = gameID;
            this.Bet        = bet;
            this.Profit     = profit;
            this.Timestamp  = timestamp;
            this.TransactionID = long.Parse(transactionID);
            this.Data = strData;
        }
    }
}