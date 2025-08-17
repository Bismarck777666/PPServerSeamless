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
    //게임입장요청메세지
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

    //게임입장응답메세지
    public class EnterGameResponse
    {
        public IActorRef        GameActor   { get; private set; }
        public int              GameID      { get; private set; }
        public int              Ack         { get; private set; }       //0: 입장성공, 기타: 입장실패
        public List<GITMessage> SubMessages { get; private set; }       //게임입장후에 서버에서 유저에게 보낼 메세지들

        public EnterGameResponse(int gameID, IActorRef gameActor, int ack)
        {
            this.GameID         = gameID;
            this.Ack            = ack;
            this.GameActor      = gameActor;
            this.SubMessages    = new List<GITMessage>();
        }
    }

    //게임탈퇴요청메세지
    public class ExitGameRequest
    {
        public string       UserID              { get; private set; }
        public int          CompanyID           { get; private set; }
        public double       Balance             { get; private set; }
        public bool         UserRequested       { get; private set; }       //유저요청에 의한것인가? 아님 게임서버노드의 shutdown으로 인한것인가?
        public Currencies   Currency            { get; private set; }   //화페
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

    //게임탙퇴응답메세지
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

    //클라이언트에서 서버노드에로 보내는 메세지
    public class FromUserMessage
    {
        public string       UserID      { get; private set; }   //유저아이디
        public int          CompanyID   { get; private set; }   //운영본사식별자
        public GITMessage   Message     { get; private set; }   //클라에서 보낸 메세지
        public UserBonus    Bonus       { get; private set; }   //유저에게 할당된 보너스정보
        public double       UserBalance { get; private set; }   //유저잔고
        public Currencies Currency    { get; private set; }   //화페
        public IActorRef    UserActor   { get; private set; }   //유저액터
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
