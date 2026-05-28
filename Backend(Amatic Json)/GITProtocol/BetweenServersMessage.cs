using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Akka.Actor;
using Akka.Routing;
using Newtonsoft.Json;


namespace GITProtocol
{
    //游戏入场请求消息
    public class EnterGameRequest
    {
        public string    UserID     { get; private set; }
        public int       GameID     { get; private set; }
        public bool      NewEnter   { get; private set; }
        public IActorRef UserActor  { get; private set; }
        public EnterGameRequest(int gameID, string userID, IActorRef userActor, bool newEnter = true)
        {
            this.GameID     = gameID;
            this.UserID     = userID;
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
        public string       UserID              { get; private set; }
        public int          CompanyID           { get; private set; }
        public double       Balance             { get; private set; }
        public bool         UserRequested       { get; private set; }       //是否由用户请求？还是由游戏服务器节点关闭导致？
        public Currencies   Currency            { get; private set; }   //货币
        public bool         IsNewServerReady    { get; private set; }

        public ExitGameRequest(string userID, int companyID, double balance, Currencies currency, bool userRequested, bool isNewServerReady)
        {
            this.UserID             = userID;
            this.CompanyID          = companyID;
            this.Balance            = balance;
            this.UserRequested      = userRequested;
            this.Currency           = currency;
            this.IsNewServerReady   = isNewServerReady;
        }
    }

    //游戏退出响应消息
    public class ExitGameResponse
    {

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
        public int          CompanyID   { get; private set; }   //运营公司标识符
        public GITMessage   Message     { get; private set; }   //客户端发送的消息
        public UserBonus    Bonus       { get; private set; }   //分配给用户的奖励信息
        public double       UserBalance { get; private set; }   //用户余额
        public Currencies Currency    { get; private set; }   //货币
        public IActorRef    UserActor   { get; private set; }   //用户角色
        public FromUserMessage(string strUserID, int companyID, double userBalance, Currencies currency, IActorRef userActor, GITMessage message, UserBonus bonus)
        {
            this.UserID         = strUserID;
            this.CompanyID      = companyID;
            this.UserBalance    = userBalance;
            this.Currency       = currency;
            this.UserActor      = userActor;
            this.Message        = message;
            this.Bonus          = bonus;
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
            this.GameName   = strGameName;
            this.TableName  = strTableName;
            this.LogString  = strGameLog;
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

    public enum UserBetTypes
    {
        Normal          = 0,
        PurchaseFree    = 1,
        AnteBet         = 2,
    }

    public class ToUserResultMessage : ToUserMessage
    {
        public double       BetMoney    { get; private set; }
        public double       WinMoney    { get; private set; }
        public double       TurnOver    { get; protected set; }
        public GameLogInfo  GameLog     { get; private set; }
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

    public class ToUserSpecialResultMessage : ToUserResultMessage
    {
        public double RealBet { get; set; }
        public bool IsJustBet { get; set; }

        public ToUserSpecialResultMessage()
        {

        }
        public ToUserSpecialResultMessage(int gameID, GITMessage message, double betMoney) : base(gameID, message, 0.0, 0.0, null)
        {
            this.IsJustBet = true;
            this.RealBet = betMoney;
            this.TurnOver = betMoney;
        }

        public ToUserSpecialResultMessage(int gameID, GITMessage message, double realBet, double betMoney, double winMoney, GameLogInfo gameLog, double turnOver = -1.0) : base(gameID, message, betMoney, winMoney, gameLog, turnOver)
        {
            this.RealBet = realBet;
            this.IsJustBet = false;
            this.TurnOver = realBet;
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
    
    public class SocketConnectionAdded
    {

    }
    
    public class SocketConnectionClosed
    {

    }
    
    public class SlotsNodeShuttingDownMsg
    {

    }

    public class SubtractEventMoneyRequest : IConsistentHashable
    {
        public int      AgentID             { get; private set; }
        public string   UserID              { get; private set; }
        public double   EventMoney          { get; private set; }
        public object   ConsistentHashKey   => AgentID;
        public SubtractEventMoneyRequest(int websiteID, string strUserID, double eventMoney)
        {
            this.AgentID    = websiteID;
            this.UserID     = strUserID;
            this.EventMoney = eventMoney;
        }
    }
    
    public class AddEventLeftMoneyRequest : IConsistentHashable
    {
        public int      AgentID     { get; private set; }
        public string   UserID      { get; private set; }
        public double   LeftMoney   { get; private set; }

        public object   ConsistentHashKey => AgentID;

        public AddEventLeftMoneyRequest(int websiteID, string strUserID, double leftMoney)
        {
            this.AgentID    = websiteID;
            this.UserID     = strUserID;
            this.LeftMoney  = leftMoney;
        }
    }
}
