using Akka.Actor;
using Akka.Routing;
using System.Collections.Generic;

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

        public CreateNewUserMessage(string sessionToken,long userDBID,string strUserID,double userBalance,string passToken,int agentDBID,string agentID,long lastScoreCounter,Currencies currency)
        {
            SessionToken        = sessionToken;
            UserID              = strUserID;
            UserDBID            = userDBID;
            UserBalance         = userBalance;
            PassToken           = passToken;
            AgentDBID           = agentDBID;
            LastScoreCounter    = lastScoreCounter;
            Currency            = currency;
            AgentID             = agentID;
        }

        public string GlobalUserID => string.Format("{0}_{1}", AgentDBID, UserID);
    }
    public class FromConnRevMessage
    {
        public string       SessionToken    { get; private set; }
        public GITMessage   Message         { get; private set; }

        public FromConnRevMessage(string token, GITMessage message)
        {
            SessionToken    = token;
            Message         = message;
        }
    }
    public class SendMessageToUser
    {
        public GITMessage   Message     { get; private set; }
        public double       Balance     { get; private set; }
        public double       Delay       { get; private set; }

        public SendMessageToUser(GITMessage message, double balance, double delay)
        {
            Message = message;
            Balance = balance;
            Delay   = delay;
        }
    }
    public class EnterGameRequest
    {
        public int          AgentID     { get; private set; }
        public string       UserID      { get; private set; }
        public int          GameID      { get; private set; }
        public bool         NewEnter    { get; private set; }
        public IActorRef    UserActor   { get; private set; }

        public EnterGameRequest(int gameID,int agentID,string userID,IActorRef userActor,bool newEnter = true)
        {
            AgentID     = agentID;
            GameID      = gameID;
            UserID      = userID;
            NewEnter    = newEnter;
            UserActor   = userActor;
        }
    }
    public class EnterGameResponse
    {
        public IActorRef        GameActor   { get; private set; }
        public int              GameID      { get; private set; }
        public int              Ack         { get; private set; }
        public List<GITMessage> SubMessages { get; private set; }

        public EnterGameResponse(int gameID, IActorRef gameActor, int ack)
        {
            GameID      = gameID;
            Ack         = ack;
            GameActor   = gameActor;
            SubMessages = new List<GITMessage>();
        }
    }
    public class ExitGameRequest
    {
        public string   UserID              { get; private set; }
        public int      WebsiteID           { get; private set; }
        public double   Balance             { get; private set; }
        public bool     UserRequested       { get; private set; }
        public bool     IsNewServerReady    { get; private set; }

        public ExitGameRequest(string userID,int websiteID,double balance,bool userRequested,bool isNewServerReady)
        {
            UserID              = userID;
            WebsiteID           = websiteID;
            Balance             = balance;
            UserRequested       = userRequested;
            IsNewServerReady    = isNewServerReady;
        }
    }
    public class ExitGameResponse
    {
    }
    public class FromUserMessage
    {
        public string       UserID      { get; private set; }
        public int          WebsiteID   { get; private set; }
        public GITMessage   Message     { get; private set; }
        public UserBonus    Bonus       { get; private set; }
        public double       UserBalance { get; private set; }
        public IActorRef    UserActor   { get; private set; }
        public Currencies   Currency    { get; private set; }

        public FromUserMessage(string strUserID,int websiteID,double userBalance,IActorRef userActor,GITMessage message,UserBonus bonus,Currencies currency)
        {
            UserID      = strUserID;
            WebsiteID   = websiteID;
            UserBalance = userBalance;
            UserActor   = userActor;
            Message     = message;
            Bonus       = bonus;
            Currency    = currency;
        }
    }
    public class GameLogInfo
    {
        public string   GameName    { get; private set; }
        public string   TableName   { get; private set; }
        public string   LogString   { get; private set; }

        public GameLogInfo()
        {
        }

        public GameLogInfo(string strGameName, string strTableName, string strGameLog)
        {
            GameName    = strGameName;
            TableName   = strTableName;
            LogString   = strGameLog;
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
            if (message != null)
                Messages.Insert(0, message);
        }

        public ToUserMessage()
        {
            IsRewardedBonus     = false;
            RewardBonusMoney    = 0.0;
        }

        public ToUserMessage(int gameID, GITMessage message)
        {
            GameID      = gameID;
            Messages    = new List<GITMessage>();
            if (message != null)
                Messages.Add(message);

            IsRewardedBonus     = false;
            RewardBonusMoney    = 0.0;
        }

        public void setBonusReward(double bonusMoney)
        {
            IsRewardedBonus     = true;
            RewardBonusMoney    = bonusMoney;
        }

        public void setCountAsSpin(bool isCountAsSpin)
        {
            IsCountAsSpin = isCountAsSpin;
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

        public ToUserResultMessage()
        {
        }

        public ToUserResultMessage(int gameID,GITMessage message,double betMoney,double winMoney,GameLogInfo gameLog,UserBetTypes betType,double turnOver = -1.0) : base(gameID, message)
        {
            BetMoney    = betMoney;
            WinMoney    = winMoney;
            GameLog     = gameLog;
            BetType     = betType;
            if (turnOver == -1.0)
                TurnOver = betMoney;
            else
                TurnOver = turnOver;
        }
    }
    public class ToUserSpecialResultMessage : ToUserResultMessage
    {
        public double   RealBet     { get; set; }
        public bool     IsJustBet   { get; set; }

        public ToUserSpecialResultMessage()
        {
        }

        public ToUserSpecialResultMessage(int gameID, GITMessage message, double betMoney)
          : base(gameID, message, 0.0, 0.0, null, UserBetTypes.Normal)
        {
            IsJustBet   = true;
            RealBet     = betMoney;
            TurnOver    = betMoney;
        }

        public ToUserSpecialResultMessage(int gameID,GITMessage message,double realBet,double betMoney,double winMoney,GameLogInfo gameLog,double turnOver = -1.0)
          : base(gameID, message, betMoney, winMoney, gameLog, UserBetTypes.Normal, turnOver)
        {
            RealBet     = realBet;
            IsJustBet   = false;
            TurnOver    = realBet;
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
    public class UserMaxWinEventBonus : UserBonus
    {
        public double MaxWin { get; private set; }

        public UserMaxWinEventBonus(long bonusID, double maxWin)
        {
            BonusID     = bonusID;
            MaxWin      = maxWin;
            BonusType   = UserBonusType.USEREVENT;
        }
    }
    public class UserRangeOddEventBonus : UserBonus
    {
        public double MinOdd { get; private set; }
        public double MaxOdd { get; private set; }
        public double MaxBet { get; private set; }

        public UserRangeOddEventBonus(long bonusID, double minOdd, double maxOdd, double maxBet)
        {
            BonusID     = bonusID;
            MinOdd      = minOdd;
            MaxOdd      = maxOdd;
            MaxBet      = maxBet;
            BonusType   = UserBonusType.USEREVENT;
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
            BonusID     = bonusID;
            MaxOdd      = maxOdd;
            BaseBet     = baseBet;
            BonusType   = UserBonusType.USEREVENT;
        }
    }
    public class UserPPRacePrizeBonus : UserBonus
    {
        public int      RaceID                  { get; private set; }
        public string   PrizeType               { get; private set; }
        public double   MinBetLimit             { get; private set; }
        public double   BetMultiplier           { get; private set; }
        public double   MaxBetLimitByMultiplier { get; private set; }

        public UserPPRacePrizeBonus()
        {
        }

        public UserPPRacePrizeBonus(long bonusID,int raceID,string prizeType,double betMultiplier,double minBetLimit,double maxBetLimitByMultiplier)
        {
            BonusID                 = bonusID;
            RaceID                  = raceID;
            BonusType               = UserBonusType.RACEPRIZE;
            PrizeType               = prizeType;
            BetMultiplier           = betMultiplier;
            MinBetLimit             = minBetLimit;
            MaxBetLimitByMultiplier = maxBetLimitByMultiplier;
        }
    }
    public class HttpSessionAdded
    {
        public string SessionToken { get; private set; }
        public HttpSessionAdded(string strSessionToken)
        {
            SessionToken = strSessionToken;
        }
    }
    public class CloseHttpSession
    {
        public string SessionToken { get; private set; }

        public CloseHttpSession(string strSessionToken)
        {
            SessionToken = strSessionToken;
        }
    }
    public class HttpSessionClosed
    {
        public string SessionToken { get; private set; }

        public HttpSessionClosed(string strSessionToken)
        {
            SessionToken = strSessionToken;
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
        public int                      id              { get; set; }
        public string                   name            { get; set; }
        public int                      startDate       { get; set; }
        public int                      endDate         { get; set; }
        public string                   status          { get; set; }
        public bool                     showWinnersList { get; set; }
        public string                   clientStyle     { get; set; }
        public bool                     optin           { get; set; }
        public string                   clientMode      { get; set; }
        public string                   htmlRules       { get; set; }
        public string                   shortHtmlRules  { get; set; }
        public PPRacePrizePool          prizePool       { get; set; }
        public List<PPRaceWinner>       winners         { get; set; }
        public Dictionary<long, bool>   dicWinners      { get; set; }
        public List<PPRaceWinner>       pendingPrizes   { get; set; }
        public string                   lbGuid          { get; set; }
    }
    public class PPRacePrizePool
    {
        public string               currency                { get; set; }
        public string               currencyOriginal        { get; set; }
        public double               maxBetLimitByMultiplier { get; set; }
        public double               minBetLimit             { get; set; }
        public List<PPRacePrize>    prizesList              { get; set; }
        public double               totalPrizeAmount        { get; set; }
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
        public int                          id              { get; set; }
        public int                          startDate       { get; set; }
        public int                          endDate         { get; set; }
        public string                       name            { get; set; }
        public List<string>                 optJurisdiction { get; set; }
        public string                       status          { get; set; }
        public string                       clientMode      { get; set; }
        public string                       htmlRules       { get; set; }
        public string                       shortHtmlRules  { get; set; }
        public List<PPTournamentLeaderItem> leaderBoard     { get; set; }
        public PPTournamentPrizePool        prizePool       { get; set; }
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
        public string                   currency            { get; set; }
        public string                   currencyOriginal    { get; set; }
        public double                   minBetLimit         { get; set; }
        public double                   totalPrizeAmount    { get; set; }
        public List<PPTournamentPrize>  prizesList          { get; set; }
    }
    public class RequirePromoSnapshot
    {
    }
    public class PPTournamentPrize
    {
        public int      placeFrom   { get; set; }
        public int      placeTo     { get; set; }
        public long     amount      { get; set; }
        public string   type        { get; set; }
    }
    public class PPGameRecentHistoryDetailItem
    {
        public long                         roundId         { get; set; }
        public string                       configHash      { get; set; }
        public string                       currency        { get; set; }
        public string                       currencySymbol  { get; set; }
        public Dictionary<string, string>   request         { get; set; }
        public Dictionary<string, string>   response        { get; set; }
    }
    public class PPGameRecentHistoryItem
    {
        public string   balance         { get; set; }
        public string   bet             { get; set; }
        public string   win             { get; set; }
        public long     dateTime        { get; set; }
        public string   roundDetails    { get; set; }
        public long     roundId         { get; set; }
        public string   hash            { get; set; }
        public string   currency        { get; set; }
        public string   currencySymbol  { get; set; }
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

        public PPGameHistoryTopListItem(long roundID,double bet,double baseBet,double win,double rtp,string sharedLink,long playedDate)
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
    public class ForceLogoutMesssage
    {
        public int      AgentID { get; private set; }
        public string   UserID  { get; private set; }
        public string   GlobalUserID => string.Format("{0}_{1}", AgentID, UserID);

        public ForceLogoutMesssage(string strUserID)
        {
            UserID = strUserID;
        }
    }
    public class QuitAndNotifyMessage
    {
    }
    public class ApiUserBeginWithdraw : IConsistentHashable
    {
        public int      AgentID     { get; private set; }
        public string   UserID      { get; private set; }
        public string   GlobalUserID => string.Format("{0}_{1}", AgentID, UserID);
        public object   ConsistentHashKey => GlobalUserID;

        public ApiUserBeginWithdraw(int agentID, string strUserID)
        {
            AgentID = agentID;
            UserID  = strUserID;
        }
    }
    public class ApiUserEndWithdraw : IConsistentHashable
    {
        public int      AgentID { get; private set; }
        public string   UserID  { get; private set; }
        public string   GlobalUserID => string.Format("{0}_{1}", AgentID, UserID);
        public object   ConsistentHashKey => GlobalUserID;

        public ApiUserEndWithdraw(int agentID, string strUserID)
        {
            AgentID = agentID;
            UserID  = strUserID;
        }
    }
    public class ApiUserDepositMessage
    {
        public int      AgentID             { get; private set; }
        public string   UserID              { get; private set; }
        public double   Amount              { get; private set; }
        public long     LastScoreCounter    { get; private set; }
        public string   GlobalUserID => string.Format("{0}_{1}", AgentID, UserID);

        public ApiUserDepositMessage(int agentID,string strUserID,double amount,long lastScoreCounter)
        {
            AgentID             = agentID;
            UserID              = strUserID;
            Amount              = amount;
            LastScoreCounter    = lastScoreCounter;
        }
    }
    public class UserSpinItem : IConsistentHashable
    {
        public int      AgentID     { get; private set; }
        public string   UserID      { get; private set; }
        public string   Symbol      { get; private set; }
        public double   BetPerLine  { get; private set; }
        public int      LineCount   { get; private set; }
        public int      MoreBetID   { get; private set; }
        public int      PurchaseID  { get; private set; }
        public double   BetMoney    { get; private set; }

        public UserSpinItem(int agentID,string strUserID,string symbol,double betPerLine,int lineCount,int moreBetID,int purchaseID,double betMoney)
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

        public object ConsistentHashKey => string.Format("{0}_{1}", AgentID, UserID);
    }
    public enum Currencies
    {
        USD     = 0,    //1
        EUR     = 1,    //1
        TND     = 2,    //1
        KRW     = 3,    //1000
        TRY     = 4,    //10
        INR     = 5,    //50
        MAD     = 6,    //10
        BRL     = 7,    //1
        IDR     = 8,    //1000
        AUD     = 9,    //1
        CHF     = 10,   //1
        NOK     = 11,   //10
        THB     = 12,   //30
        MYR     = 13,   //1
        XOF     = 14,   //500
        NGN     = 15,   //500
        TVD     = 16,   //1
        RUB     = 17,   //50
        ILS     = 18,   //5
        ARS     = 19,   //100
        COP     = 20,   //500
        RT      = 21,   //30
        BWP     = 22,   // 10
        AED     = 23,   // 1
        UAH     = 24,   // 10
        UYU     = 25,   // 10
        LBP     = 26,   // 1000
        IQD     = 27,   // 1000
        PHP     = 28,   // 50
        EGP     = 29,   // 50
        AZN     = 30,   // 1
        PKR     = 31,   // 100
        KZT     = 32,   // 100
        UZS     = 33,   // 10000
        CAD     = 34,   // 1
        CNY     = 35,   // 5
        PLN     = 36,   // 1
        HUF     = 37,   // 100
        NZD     = 38,   // 1
        CLP     = 39,   // 500
        CZK     = 40,   // 10
        GEL     = 41,   // 1
        RON     = 42,   // 1
        BAM     = 43,   // 2
        TOPIA   = 44,   // 1
        SYP     = 45,   //1000
        SEK     = 46,   //10
        ZAR     = 47,   //10
        GBP     = 48,   //1
        GOF     = 49,   //1000
        KES     = 50,   //100
        ZWL     = 51,   //5000
        ZMW     = 52,   //5
        AOA     = 53,   //50
        MZN     = 54,   //5
        NAD     = 55,   //1
        PYG     = 56,   //500
        BOB     = 57,   //1
        DZD     = 58,   //10
        ISK     = 59,   //50
        PEN     = 60,   //1
        BDT     = 61,   //1
        DKK     = 62,   //5
        MNT     = 63,   //500
        TMT     = 64,   //1
        MXN     = 65,   //PHP
        IRR     = 66,   //500000
        IRT     = 67,   //50000
        HKD     = 68,   //10
        JPY     = 69,   //100
        SGD     = 70,   //1
        VND     = 71,   //10000
        MMK     = 72,   //1000
        UGX     = 73,   //1000
        TZS     = 74,   //1000
        GHS     = 75,   //1
        RSD     = 76,   //10
        HRK     = 77,   //1
        XAF     = 78,   //500
        GNF     = 79,   //500
        VES     = 80,   //1
        BYN     = 81,   //1
        LKR     = 82,   //5
        JC      = 83,   //1
        GC      = 84,   //1
        SC      = 85,   //1
        ENT     = 86,   //10


        COUNT   = 87,
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
}
