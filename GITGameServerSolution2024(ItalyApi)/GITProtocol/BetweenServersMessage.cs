using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Akka.Actor;
using Akka.Routing;
using Newtonsoft.Json;


/**
 * 
 *      Created by Foresight(2021.03.03)
 *      解决方案的服务器节点之间交换的消息
 *
 */

namespace GITProtocol
{
    
    public enum UserBetTypes
    {
        Normal          = 0,
        PurchaseFree    = 1,
        AnteBet         = 2,
    }
    //游戏入场请求消息
    public class EnterGameRequest
    {
        public int       AgentID    { get; private set; }
        public string    UserID     { get; private set; }
        public int       Currency   { get; private set; }
        public int       GameID     { get; private set; }
        public bool      NewEnter   { get; private set; }
        public IActorRef UserActor  { get; private set; }
        public EnterGameRequest(int gameID,int agentID ,string userID,int currency ,IActorRef userActor, bool newEnter = true)
        {
            this.AgentID    = agentID;
            this.GameID     = gameID;
            this.UserID     = userID;
            this.Currency   = currency;
            this.NewEnter   = newEnter;
            this.UserActor  = userActor;
        }
    }

    //游戏入场响应消息
    public class EnterGameResponse
    {
        public IActorRef        GameActor   { get; private set; }
        public int              GameID      { get; private set; }
        public int              Ack         { get; private set; }       //0: 入场成功, 其他: 入场失败
        public List<GITMessage> SubMessages { get; private set; }       //游戏入场后服务器发送给用户的消息列表

        public EnterGameResponse(int gameID, IActorRef gameActor, int ack)
        {
            this.GameID         = gameID;
            this.Ack            = ack;
            this.GameActor      = gameActor;
            this.SubMessages    = new List<GITMessage>();
        }
    }

    //游戏退出请求消息
    public class ExitGameRequest
    {
        public string   UserID              { get; private set; }
        public int      AgentID             { get; private set; }
        public int      Currency            { get; private set; }
        public double   Balance             { get; private set; }
        public bool     UserRequested       { get; private set; }       //是由用户请求引起的吗？还是由游戏服务器节点的关闭引起的？
        public bool     IsNewServerReady    { get; private set; }

        public ExitGameRequest(string userID, int agentID, int currency, double balance, bool userRequested, bool isNewServerReady)
        {
            this.UserID             = userID;
            this.AgentID            = agentID;
            this.Currency           = currency;
            this.Balance            = balance;
            this.UserRequested      = userRequested;
            this.IsNewServerReady   = isNewServerReady;
        }
    }

    //游戏退出响应消息
    public class ExitGameResponse
    {

    }

    public class CQ9ExitGameResponse : ExitGameResponse
    {
        public ToUserResultMessage ResultMsg { get; set; }
        public CQ9ExitGameResponse(ToUserResultMessage resultMsg)
        {
            this.ResultMsg = resultMsg;
        }
    }

    public class AmaticExitResponse : ExitGameResponse
    {
        public ToUserResultMessage ResultMsg { get; set; }
        public AmaticExitResponse(ToUserResultMessage resultMsg)
        {
            this.ResultMsg = resultMsg;
        }
    }


    //从客户端发送到服务器节点的消息
    public class FromUserMessage
    {
        public string       UserID      { get; private set; }   //用户ID
        public int          AgentID     { get; private set; }   //运营总部标识符
        public GITMessage   Message     { get; private set; }   //客户端发送的消息
        public UserBonus    Bonus       { get; private set; }   //分配给用户的奖励信息
        public double       UserBalance { get; private set; }   //用户余额
        public int          Currency    { get; private set; }   // 货币
        public IActorRef    UserActor   { get; private set; }   //用户角色
        public bool         IsMustLose  { get; private set; }   //用户是否需要输钱？
        public FromUserMessage(string strUserID, int companyID, double userBalance, int currency,IActorRef userActor, GITMessage message, UserBonus bonus, bool isMustLose)
        {
            this.UserID         = strUserID;
            this.AgentID        = companyID;
            this.UserBalance    = userBalance;
            this.Currency       = currency;
            this.UserActor      = userActor;
            this.Message        = message;
            this.Bonus          = bonus;
            this.IsMustLose     = isMustLose;
        }
    }

    public class GameLogInfo
    {
        public string GameName      { get; private set; }
        public string TableName     { get; private set; }
        public string LogString     { get; private set; }
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
        public bool             IsCountAsSpin       { get; private set; }
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

        public ToUserMessage()
        {
            IsRewardedBonus = false;
            RewardBonusMoney = 0.0;
        }
        public ToUserMessage(int gameID, GITMessage message)
        {
            this.GameID   = gameID;
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

    public class ToUserResultMessage : ToUserMessage
    {
        public double       BetMoney    { get; private set; }
        public double       WinMoney    { get; private set; }
        public double       TurnOver    { get; protected set; }
        public GameLogInfo  GameLog     { get; private set; }
        public UserBetTypes BetType     { get; private set; }
        public ToUserResultMessage()
        {
        }

        public ToUserResultMessage(int gameID, GITMessage message, double betMoney, double winMoney, GameLogInfo gameLog,UserBetTypes betType,double turnOver = -1.0) : base(gameID, message)
        {
            this.BetMoney   = betMoney;
            this.WinMoney   = winMoney;
            this.GameLog    = gameLog;
            this.BetType    = betType;

            if (turnOver == -1.0)
                this.TurnOver = betMoney;
            else
                this.TurnOver = turnOver;
        }
    }

    public class ToUserSpecialResultMessage : ToUserResultMessage
    {
        public double   RealBet     { get; set; }
        public bool     IsJustBet   { get; set; }

        public ToUserSpecialResultMessage()
        {

        }
        
        public ToUserSpecialResultMessage(int gameID, GITMessage message, double betMoney) : base(gameID, message, 0.0, 0.0, null,UserBetTypes.Normal)
        {
            this.IsJustBet  = true;
            this.RealBet    = betMoney;
            this.TurnOver   = betMoney;
        }

        public ToUserSpecialResultMessage(int gameID, GITMessage message, double realBet, double betMoney, double winMoney, GameLogInfo gameLog, double turnOver = -1.0) : base(gameID, message, betMoney, winMoney, gameLog,UserBetTypes.Normal ,turnOver)
        {
            this.RealBet    = realBet;
            this.IsJustBet  = false;
            this.TurnOver   = realBet;
        }
    }
    public enum UserBonusType
    {
        GAMEJACKPOT = 0,
        USEREVENT   = 1,
        REPACKET    = 2,
        RACEPRIZE   = 3,

    }
   
    public class UserBonus
    {
        public UserBonusType    BonusType   { get; protected set; }
        public long             BonusID     { get; protected set; }
    }

    public class UserRangeOddEventBonus : UserBonus
    {
        public double MinOdd { get; private set; }
        public double MaxOdd { get; private set; }
        public double MaxBet { get; private set; }
        public UserRangeOddEventBonus(long bonusID, double minOdd, double maxOdd, double maxBet)
        {
            this.BonusID    = bonusID;
            this.MinOdd     = minOdd;
            this.MaxOdd     = maxOdd;
            this.MaxBet     = maxBet;
            this.BonusType  = UserBonusType.USEREVENT;
        }
    }
    public class UserEventBonus : UserBonus
    {
        public int      MaxOdd  { get; private set; }
        public double   BaseBet { get; private set; }
        public UserEventBonus()
        {

        }
        public UserEventBonus(long bonusID, int maxOdd, double baseBet)
        {
            this.BonusID    = bonusID;
            this.MaxOdd     = maxOdd;
            this.BaseBet    = baseBet;
            this.BonusType  = UserBonusType.USEREVENT;
        }
    }

    public class UserPPRacePrizeBonus : UserBonus
    {
        public int    RaceID                    { get; private set; }
        public string PrizeType                 { get; private set; }
        public double MinBetLimit               { get; private set; }
        public double BetMultiplier             { get; private set; }
        public double MaxBetLimitByMultiplier   { get; private set; }

        public UserPPRacePrizeBonus()
        {

        }

        public UserPPRacePrizeBonus(long bonusID, int raceID, string prizeType, double betMultiplier, double minBetLimit, double maxBetLimitByMultiplier)
        {
            this.BonusID                = bonusID;
            this.RaceID                 = raceID;
            this.BonusType              = UserBonusType.RACEPRIZE;
            this.PrizeType              = prizeType;
            this.BetMultiplier          = betMultiplier;
            this.MinBetLimit            = minBetLimit;
            this.MaxBetLimitByMultiplier = maxBetLimitByMultiplier;
        }

    }
    public class SocketConnectionAdded
    {

    }
    public class SocketConnectionClosed
    {

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

    public class UserManagementShuttingDownMsg
    {

    }

   

    public class PPRace
    {
        public int                                      id              { get; set; }
        public string                                   name            { get; set; }
        public int                                      startDate       { get; set; }
        public int                                      endDate         { get; set; }
        public string                                   status          { get; set; }
        public bool                                     showWinnersList { get; set; }
        public string                                   clientStyle     { get; set; }
        public bool                                     optin           { get; set; }
        public string                                   clientMode      { get; set; }
        public string                                   htmlRules       { get; set; }
        public string                                   shortHtmlRules  { get; set; }
        public PPRacePrizePool                          prizePool       { get; set; }
        public List<PPRaceWinner>                       winners         { get; set; }
        public Dictionary<long, bool>                   dicWinners      { get; set; }
        public List<PPRaceWinner>                       pendingPrizes   { get; set; }
        public string                                   lbGuid          { get; set; }

    }

    public class PPRacePrizePool
    {
        public string               currency                        { get; set; }
        public string               currencyOriginal                { get; set; }
        public double               maxBetLimitByMultiplier         { get; set; }
        public double               minBetLimit                     { get; set; }
        public List<PPRacePrize>    prizesList                      { get; set; }
        public double               totalPrizeAmount                { get; set; }
    }
    public class PPRacePrize
    {
        public int      prizeID         { get; set; }
        public int      count           { get; set; }
        public string   type            { get; set; }
        public double   betMultiplier   { get; set; }
    }

    public class PPRaceWinner
    {
        public long     id                              { get; set; }
        public int      prizeID                         { get; set; }
        public string   playerID                        { get; set; }
        public string   countryID                       { get; set; }
        public string   memberCurrency                  { get; set; }
        public double   bet                             { get; set; }
        public double   effectiveBetForBetMultiplier    { get; set; }
        public double   effectiveBetForFreeRounds       { get; set; }

    }
    public class PPTournament
    {
        public int                          id                      { get; set; }
        public int                          startDate               { get; set; }
        public int                          endDate                 { get; set; }
        public string                       name                    { get; set; }
        public List<string>                 optJurisdiction         { get; set; }
        public string                       status                  { get; set; }
        public string                       clientMode              { get; set; }
        public string                       htmlRules               { get; set; }
        public string                       shortHtmlRules          { get; set; }
        public List<PPTournamentLeaderItem> leaderBoard             { get; set; }
        public PPTournamentPrizePool        prizePool               { get; set; }

    }

    public class PPTournamentLeaderItem
    {
        public int      position                        { get; set; }
        public string   playerID                        { get; set; }
        public double   score                           { get; set; }
        public double   scoreBet                        { get; set; }
        public string   memberCurrency                  { get; set; }
        public double   effectiveBetForFreeRounds       { get; set; }
        public double   effectiveBetForBetMultiplier    { get; set; }
        public string   countryID                       { get; set; }
    }

    public class PPTournamentPrizePool
    {
        public string currency                  { get; set; }
        public string currencyOriginal          { get; set; }
        public double minBetLimit               { get; set; }
        public double totalPrizeAmount          { get; set; }
        public List<PPTournamentPrize> prizesList { get; set; }
        
    }
    public class PPTournamentPrize
    {
        public int      placeFrom    { get; set; }
        public int      placeTo      { get; set; }
        public long     amount       { get; set; }
        public string   type        { get; set; }
    }

    public class RequirePromoSnapshot
    {

    }

    public class PPGameRecentHistoryDetailItem
    {
        public string                       roundId         { get; set; }
        public string                       configHash      { get; set; }
        public string                       currency        { get; set; }
        public string                       currencySymbol  { get; set; }
        public Dictionary<string, string>   request         { get; set; }
        public Dictionary<string, string>   response        { get; set; }

    }
    public class PPGameRecentHistoryItem
    {

        public string balance       { get; set; }
        public string bet           { get; set; }
        public string win           { get; set; }
        public long   dateTime      { get; set; }
        public string roundDetails  { get; set; }
        public string roundId       { get; set; }
        public string hash          { get; set; }
    }


    public class PPGameHistoryTopListItem
    {
        public long     roundID     { get; private set; }
        public double   bet         { get; private set; }
        public double   base_bet    { get; private set; }
        public double   win         { get; private set; }
        public double   rtp         { get; private set; }
        public string   sharedLink  { get; private set; }
        public long     playedDate  { get; private set; }

        public PPGameHistoryTopListItem(long roundID, double bet, double baseBet, double win, double rtp, string sharedLink, long playedDate)
        {
            this.roundID    = roundID;
            this.bet        = bet;
            this.base_bet   = baseBet;
            this.win        = win;
            this.rtp        = rtp;
            this.sharedLink = sharedLink;
            this.playedDate = playedDate;
        }
    }

    public class SubtractEventMoneyRequest : IConsistentHashable
    {
        public int      AgentID             { get; private set; }
        public string   UserID              { get; private set; }
        public double   EventMoney          { get; private set; }
        public object   ConsistentHashKey   => AgentID;

        public SubtractEventMoneyRequest(int websiteID, string strUserID, double eventMoney)
        {
            AgentID     = websiteID;
            UserID      = strUserID;
            EventMoney  = eventMoney;
        }
    }

    public class AddEventLeftMoneyRequest : IConsistentHashable
    {
        public int      AgentID             { get; private set; }
        public string   UserID              { get; private set; }
        public double   LeftMoney           { get; private set; }
        public object   ConsistentHashKey   => AgentID;

        public AddEventLeftMoneyRequest(int websiteID, string strUserID, double leftMoney)
        {
            AgentID     = websiteID;
            UserID      = strUserID;
            LeftMoney   = leftMoney;
        }
    }

    public class ApiDepositMessage
    {
        public int      AgentID             { get; private set; }
        public string   UserID              { get; private set; }
        public double   Amount              { get; private set; }
        public long     LastScoreCounter    { get; private set; }
        public string   GlobalUserID        => string.Format("{0}_{1}", AgentID, UserID);

        public ApiDepositMessage(int agentID, string strUserID, double amount, long lastScoreCounter)
        {
            AgentID             = agentID;
            UserID              = strUserID;
            Amount              = amount;
            LastScoreCounter    = lastScoreCounter;
        }
    }

    public class ApiWithdrawRequest : IConsistentHashable
    {
        public int      AgentID             { get; private set; }
        public string   UserID              { get; private set; }
        public double   Amount              { get; private set; }
        public string   GlobalUserID        => string.Format("{0}_{1}", AgentID, UserID);
        public object   ConsistentHashKey   => GlobalUserID;

        public ApiWithdrawRequest(int agentID, string strUserID, double amount)
        {
            AgentID     = agentID;
            UserID      = strUserID;
            Amount      = amount;
        }
    }

    public class ApiWithdrawResponse
    {
        public int      Result      { get; private set; }
        public double   BeforeScore { get; private set; }
        public double   AfterScore  { get; private set; }

        public ApiWithdrawResponse(int result, double beforeScore, double afterScore)
        {
            Result      = result;
            BeforeScore = beforeScore;
            AfterScore  = afterScore;
        }
    }
}
