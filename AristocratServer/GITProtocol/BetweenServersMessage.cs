using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Akka.Actor;
using Akka.Routing;
using Newtonsoft.Json;


namespace GITProtocol
{
    public class EnterGameRequest
    {
        public string UserID { get; private set; }
        public int GameID { get; private set; }
        public bool NewEnter { get; private set; }
        public IActorRef UserActor { get; private set; }
        public EnterGameRequest(int gameID, string userID, IActorRef userActor, bool newEnter = true)
        {
            this.GameID = gameID;
            this.UserID = userID;
            this.NewEnter = newEnter;
            this.UserActor = userActor;
        }
    }

    public class EnterGameResponse
    {
        public IActorRef GameActor { get; private set; }
        public int GameID { get; private set; }
        public int Ack { get; private set; }
        public List<GITMessage> SubMessages { get; private set; }

        public EnterGameResponse(int gameID, IActorRef gameActor, int ack)
        {
            this.GameID = gameID;
            this.Ack = ack;
            this.GameActor = gameActor;
            this.SubMessages = new List<GITMessage>();
        }
    }

    public class ExitGameRequest
    {
        public string UserID { get; private set; }
        public int CompanyID { get; private set; }
        public double Balance { get; private set; }
        public bool UserRequested { get; private set; }
        public bool IsNewServerReady { get; private set; }

        public ExitGameRequest(string userID, int companyID, double balance, bool userRequested, bool isNewServerReady)
        {
            this.UserID = userID;
            this.CompanyID = companyID;
            this.Balance = balance;
            this.UserRequested = userRequested;
            this.IsNewServerReady = isNewServerReady;
        }
    }
    public class ExitGameResponse
    {
    }
    public class FromUserMessage
    {
        public string UserID { get; private set; }
        public int CompanyID { get; private set; }
        public GITMessage Message { get; private set; }
        public UserBonus Bonus { get; private set; }
        public double UserBalance { get; private set; }
        public IActorRef UserActor { get; private set; }
        public Currencies Currency { get; private set; }
        public int MoneyMode { get; private set; }
        public FromUserMessage(string strUserID, int companyID, double userBalance, IActorRef userActor, GITMessage message, UserBonus bonus, Currencies currency, int moneyMode)
        {
            this.UserID = strUserID;
            this.CompanyID = companyID;
            this.UserBalance = userBalance;
            this.UserActor = userActor;
            this.Message = message;
            this.Bonus = bonus;
            this.Currency = currency;
            this.MoneyMode = moneyMode;
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
        public int GameID { get; private set; }
        public List<GITMessage> Messages { get; private set; }

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
        }
        public ToUserMessage(int gameID, GITMessage message)
        {
            this.GameID = gameID;
            this.Messages = new List<GITMessage>();
            if (message != null)
                this.Messages.Add(message);
        }
    }
    public class ToUserResultMessage : ToUserMessage
    {
        public double BetMoney { get; private set; }
        public double WinMoney { get; private set; }
        public double RealBet { get; private set; }
        public double TurnOver { get; protected set; }
        public GameLogInfo GameLog { get; private set; }
        public string BetTransactionID { get; set; }
        public string TransactionID { get; set; }
        public string RoundID { get; set; }

        public long FreeSpinID { get; set; }

        public UserBonus Bonus { get; private set; }

        public ToUserResultMessage()
        {
        }

        public ToUserResultMessage(int gameID, GITMessage message, double betMoney, double winMoney, double realBet, GameLogInfo gameLog, long freeSpinID, UserBonus bonus, double turnOver = -1.0) : base(gameID, message)
        {
            this.BetMoney = betMoney;
            this.WinMoney = winMoney;
            this.GameLog = gameLog;
            this.RealBet = realBet;
            this.Bonus = bonus;
            if (turnOver == -1.0)
                this.TurnOver = realBet;
            else
                this.TurnOver = realBet;
        }
    }
    public enum UserBonusType
    {
        GAMEJACKPOT = 0,
        USEREVENT = 1,
        REPACKET = 2,
        RACEPRIZE = 3,
        FREESPIN = 4,
        CASHBACK = 5,
    }
    public class UserBonus
    {
        public UserBonusType BonusType { get; protected set; }
        public long BonusID { get; protected set; }
        public bool IsRewarded { get; set; }
    }
    public class UserFreeSpinBonus : UserBonus
    {
        public string AgentID { get; private set; }
        public int GameID { get; private set; }
        public string Currency { get; private set; }
        public int BetLevel { get; private set; }
        public int FreeSpinCount { get; private set; }
        public int ExpireTime { get; private set; }  

        public UserFreeSpinBonus(int bonusID, string agentID, string currency, int gameid, int betLevel, int freeSpinCount, int expireTime)
        {
            this.BonusID = bonusID;
            this.AgentID = agentID;
            this.Currency = currency;
            this.GameID = gameid;
            this.BetLevel = betLevel;
            this.FreeSpinCount = freeSpinCount;
            this.ExpireTime = expireTime;
            this.BonusType = UserBonusType.FREESPIN;
        }
    }

    public class UserPPRacePrizeBonus : UserBonus
    {
        public string CuntryCode { get; private set; }
        public string Currency { get; private set; }
        public int RaceID { get; private set; }
        public int PrizeID { get; private set; }
        public string AgentID { get; private set; }
        public string PrizeType { get; private set; }
        public int IsAgent { get; private set; }
        public double MinBetLimit { get; private set; }
        public double BetMultiplier { get; private set; }
        public double Amount { get; private set; }
        public double MaxBetLimitByMultiplier { get; private set; }

        public UserPPRacePrizeBonus(long bonusID, string countryCode, string currency, int raceID, int prizeID, string agentID, string prizeType, int isAgent, double amount, double betMultiplier, double minBetLimit, double maxBetLimitByMultiplier)
        {
            this.BonusType = UserBonusType.RACEPRIZE;
            this.BonusID = bonusID;
            this.CuntryCode = countryCode;
            this.Currency = currency;
            this.RaceID = raceID;
            this.PrizeID = prizeID;
            this.AgentID = agentID;
            this.PrizeType = prizeType;
            this.IsAgent = isAgent;
            this.Amount = amount;
            this.BetMultiplier = betMultiplier;
            this.MinBetLimit = minBetLimit;
            this.MaxBetLimitByMultiplier = maxBetLimitByMultiplier;
        }
    }
    public class UserPPCashback : UserBonus
    {
        public int CashbackID { get; private set; }
        public double Cashback { get; private set; }
        public string AgentID { get; private set; }
        public string CuntryCode { get; private set; }
        public string Currency { get; private set; }
        public int IsAgent { get; private set; }
        public int Period { get; private set; }
        public string PeriodKey { get; private set; }
        public UserPPCashback(long bonusID, int cashbackID, double cashback, string agentID, string countryCode, string currency, int isAgent, int period, string periodkey)
        {
            this.BonusType = UserBonusType.CASHBACK;
            this.BonusID = bonusID;
            this.CashbackID = cashbackID;
            this.Cashback = cashback;
            this.CuntryCode = countryCode;
            this.Currency = currency;
            this.AgentID = agentID;
            this.IsAgent = isAgent;
            this.Period = period;
            this.PeriodKey = periodkey;
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
    public class UserManagementShuttingDownMsg
    {

    }
    public class PPRace
    {
        public int id { get; set; }
        public string name { get; set; }
        public int startDate { get; set; }
        public int endDate { get; set; }
        public string status { get; set; }
        public bool showWinnersList { get; set; }
        public string clientStyle { get; set; }
        public bool optin { get; set; }
        public string clientMode { get; set; }
        public string htmlRules { get; set; }
        public string shortHtmlRules { get; set; }
        public PPRacePrizePool prizePool { get; set; }
        public string agentid { get; set; }   //프리이즈를 창조한 에이전트(null이면 슈퍼)
        public int prizesLimit { get; set; }   //한번에 받을수있는 프리이즈개수 (0이면 무한대)
        public string games { get; set; }   //프라이즈에 참가하는 게임들(빈문자이면 모든게임 콤마구분)
        public string playersinc { get; set; }   //프라이즈에 참가하는 유저들(빈문자이면 모든유저 콤마구분)
        public string playersexc { get; set; }   //프라이즈에 참가못하는 유저들(빈문자이면 없음 콤마구분)
        public List<PPRaceWinner> winners { get; set; }
        public Dictionary<long, bool> dicWinners { get; set; }
        public List<PPRaceWinner> pendingPrizes { get; set; }
        public string lbGuid { get; set; }
        public int type { get; set; }           // 0: PrizeDrop, 1: Cashback      
    }
    public class PPCashback : PPRace
    {
        public int cType { get; set; }          // 0: TotalBet - TotalWin, 1: TotalWin - TotalBet
        public int period { get; set; }         // 0: Daily, 1: Weekly, 2: Monthly  
        public double minNet { get; set; }

        public double cashback { get; set; }
        public int rounds { get; set; }


    }
    public class PPCashbackInfo
    {
        public int BonusID { get; set; }
        public string AgentID { get; set; }
        public int Rounds { get; set; }
        public double Bet { get; set; }
        public double Win { get; set; }

        public PPCashbackInfo()
        {

        }
        public PPCashbackInfo(int bonusID, string agentID, int rounds, double bet, double win)
        {
            BonusID = bonusID;
            AgentID = agentID;
            Rounds = rounds;
            Bet = bet;
            Win = win;
        }
        public void SerializeFrom(BinaryReader reader)
        {
            this.BonusID = reader.ReadInt32();
            this.AgentID = reader.ReadString();
            this.Rounds = reader.ReadInt32();
            this.Bet = reader.ReadDouble();
            this.Win = reader.ReadDouble();
        }
        public void SerializeTo(BinaryWriter writer)
        {
            writer.Write(BonusID);
            writer.Write(AgentID);
            writer.Write(Rounds);
            writer.Write(Bet);
            writer.Write(Win);
        }
        public byte[] convertToByte()
        {
            using (MemoryStream ms = new MemoryStream())
            {
                using (BinaryWriter bw = new BinaryWriter(ms))
                {
                    this.SerializeTo(bw);
                }
                return ms.ToArray();
            }
        }

    }
    public class PPRacePrizePool
    {
        public string currency { get; set; }
        public string currencyOriginal { get; set; }
        public double maxBetLimitByMultiplier { get; set; }
        public double minBetLimit { get; set; }
        public List<PPRacePrize> prizesList { get; set; }
        public double totalPrizeAmount { get; set; }
    }
    public class PPRacePrize
    {
        public int prizeID { get; set; }
        public int count { get; set; }
        public string type { get; set; }
        public double betMultiplier { get; set; }
        public double amount { get; set; }
        public string gift { get; set; }

    }
    public class PPRaceWinner
    {
        public long id { get; set; }
        public int prizeID { get; set; }
        public string playerID { get; set; }
        public string countryID { get; set; }
        public string memberCurrency { get; set; }
        public double bet { get; set; }
        public double effectiveBetForBetMultiplier { get; set; }
        public double effectiveBetForFreeRounds { get; set; }

    }
    public class PPTournament
    {
        public int id { get; set; }
        public int startDate { get; set; }
        public int endDate { get; set; }
        public string name { get; set; }
        public List<string> optJurisdiction { get; set; }
        public string status { get; set; }
        public string clientMode { get; set; }
        public string htmlRules { get; set; }
        public string shortHtmlRules { get; set; }
        public List<PPTournamentLeaderItem> leaderBoard { get; set; }
        public PPTournamentPrizePool prizePool { get; set; }
        public int type { get; set; }
        public string agentid { get; set; }   //프리이즈를 창조한 에이전트(null이면 슈퍼)
        public int currency { get; set; }   //토너먼트 화페
        public string games { get; set; }   //프라이즈에 참가하는 게임들(빈문자이면 모든게임 콤마구분)
        public string playersinc { get; set; }   //프라이즈에 참가하는 유저들(빈문자이면 모든유저 콤마구분)
        public string playersexc { get; set; }   //프라이즈에 참가못하는 유저들(빈문자이면 없음 콤마구분)
    }

    public class PPTournamentLeaderItem
    {
        public int position { get; set; }
        public string playerID { get; set; }
        public double score { get; set; }
        public double scoreBet { get; set; }
        public string memberCurrency { get; set; }
        public double effectiveBetForFreeRounds { get; set; }
        public double effectiveBetForBetMultiplier { get; set; }
        public string countryID { get; set; }
        public double totalbet { get; set; }           //전체베팅
        public double totalwin { get; set; }           //전체당첨
        public double win { get; set; }           //토너먼트당첨금
    }

    public class PPTournamentPrizePool
    {
        public string currency { get; set; }
        public string currencyOriginal { get; set; }
        public double minBetLimit { get; set; }
        public double maxBetLimitByMultiplier { get; set; }
        public double totalPrizeAmount { get; set; }
        public List<PPTournamentPrize> prizesList { get; set; }

    }
    public class PPTournamentPrize
    {
        public int placeFrom { get; set; }
        public int placeTo { get; set; }
        public string type { get; set; }
        public long amount { get; set; }
        public double betMultiplier { get; set; }
    }
    public class PPFreespin
    {
        public int id { get; set; }
        public string name { get; set; }
        public string agentid { get; set; }
        public int startDate { get; set; }
        public int endDate { get; set; }
        public string status { get; set; }
        public int currency { get; set; }
        public int level { get; set; }
        public int fscount { get; set; }
        public string games { get; set; }
        public string playersinc { get; set; }
        public string playersexc { get; set; }
    }


    public class PPFreeSpinInfo
    {
        public int BonusID { get; set; }
        public string AgentID { get; set; }
        public string Currency { get; set; }
        public int AwardedCount { get; set; }
        public int RemainCount { get; set; }
        public double BetPerLine { get; set; }
        public double TotalWin { get; set; }
        public bool Pending { get; set; }
        public DateTime ExpireTime { get; set; }

        public PPFreeSpinInfo()
        {

        }
        public PPFreeSpinInfo(int bonusID, string agentID, string currency, int awardedCount, int remainCount, double betPerLine, double totalWin, bool pending, int expireTime)
        {
            BonusID = bonusID;
            AgentID = agentID;
            Currency = currency;
            AwardedCount = awardedCount;
            RemainCount = remainCount;
            BetPerLine = betPerLine;
            TotalWin = totalWin;
            Pending = pending;
            ExpireTime = DateTimeOffset.FromUnixTimeSeconds(expireTime).DateTime;
        }
        public void SerializeFrom(BinaryReader reader)
        {
            this.BonusID = reader.ReadInt32();
            this.AgentID = reader.ReadString();
            this.Currency = reader.ReadString();
            this.AwardedCount = reader.ReadInt32();
            this.RemainCount = reader.ReadInt32();
            this.BetPerLine = reader.ReadDouble();
            this.TotalWin = reader.ReadDouble();
            this.Pending = reader.ReadBoolean();
            this.ExpireTime = DateTime.Parse(reader.ReadString());
        }
        public void SerializeTo(BinaryWriter writer)
        {
            writer.Write(BonusID);
            writer.Write(AgentID);
            writer.Write(Currency);
            writer.Write(AwardedCount);
            writer.Write(RemainCount);
            writer.Write(BetPerLine);
            writer.Write(TotalWin);
            writer.Write(Pending);
            writer.Write(ExpireTime.ToString("yyyy-MM-dd HH:mm:ss"));
        }
        public byte[] convertToByte()
        {
            using (MemoryStream ms = new MemoryStream())
            {
                using (BinaryWriter bw = new BinaryWriter(ms))
                {
                    this.SerializeTo(bw);
                }
                return ms.ToArray();
            }
        }
    }

    public class RequirePromoSnapshot
    {

    }
    public class PPGameRecentHistoryDetailItem
    {
        public long roundId { get; set; }
        public string configHash { get; set; }
        public string currency { get; set; }
        public string currencySymbol { get; set; }
        public Dictionary<string, string> request { get; set; }
        public Dictionary<string, string> response { get; set; }

    }
    public class PPGameRecentHistoryItem
    {

        public string balance { get; set; }
        public string bet { get; set; }
        public string win { get; set; }
        public long dateTime { get; set; }
        public string roundDetails { get; set; }
        public long roundId { get; set; }
        public string hash { get; set; }
    }
    public class PPGameLogHistoryItem
    {

        public string balance { get; set; }
        public string bet { get; set; }
        public string win { get; set; }
        public long dateTime { get; set; }
        public string roundDetails { get; set; }
        public long roundId { get; set; }
        public string hash { get; set; }
        public string currency { get; set; }
        public string currencySymbol { get; set; }
    }
    public class PPGameHistoryTopListItem
    {
        public long roundID { get; private set; }
        public double bet { get; private set; }
        public double base_bet { get; private set; }
        public double win { get; private set; }
        public double rtp { get; private set; }
        public string sharedLink { get; private set; }
        public long playedDate { get; private set; }

        public PPGameHistoryTopListItem(long roundID, double bet, double baseBet, double win, double rtp, string sharedLink, long playedDate)
        {
            this.roundID = roundID;
            this.bet = bet;
            this.base_bet = baseBet;
            this.win = win;
            this.rtp = rtp;
            this.sharedLink = sharedLink;
            this.playedDate = playedDate;
        }
    }
    public class ApiDepositMessage
    {
        public int AgentID { get; private set; }
        public string UserID { get; private set; }
        public double Amount { get; private set; }
        public long LastScoreCounter { get; private set; }

        public ApiDepositMessage(int agentID, string strUserID, double amount, long lastScoreCounter)
        {
            this.AgentID = agentID;
            this.UserID = strUserID;
            this.Amount = amount;
            this.LastScoreCounter = lastScoreCounter;
        }
    }
    public class ApiWithdrawRequest : IConsistentHashable
    {
        public int AgentID { get; private set; }
        public string UserID { get; private set; }
        public double Amount { get; private set; }
        public object ConsistentHashKey
        {
            get { return this.UserID; }
        }
        public ApiWithdrawRequest(int agentID, string strUserID, double amount)
        {
            this.AgentID = agentID;
            this.UserID = strUserID;
            this.Amount = amount;
        }
    }
    public class ApiWithdrawCompleted : IConsistentHashable
    {
        public int AgentID { get; private set; }
        public string UserID { get; private set; }
        public object ConsistentHashKey
        {
            get { return this.UserID; }
        }
        public ApiWithdrawCompleted(int agentID, string strUserID)
        {
            this.AgentID = agentID;
            this.UserID = strUserID;
        }
    }
    public class ApiWithdrawResponse
    {
        public int Result { get; private set; }
        public double BeforeScore { get; private set; }
        public double AfterScore { get; private set; }

        public ApiWithdrawResponse(int result, double beforeScore, double afterScore)
        {
            this.Result = result;
            this.BeforeScore = beforeScore;
            this.AfterScore = afterScore;
        }
    }

    public enum Currencies
    {
        MYR = 0,
        SGD = 1,
        AUD = 2,
        THB = 3,
        USD = 4,
        MMK = 5,
        HKD = 6,
        IDR = 7,
        BDT = 8,
        INR = 9,
        CNY = 10,
        COUNT = 11,
    }

    public enum Country
    {
        MY = 0,
        SG = 1,
        AU = 2,
        TH = 3,
        US = 4,
        MM = 5,
        HK = 6,
        ID = 7,
        BD = 8,
        IN = 9,
        CN = 10,
    }
    public enum PPTourType
    {
        HighestSingleSpinWinAmount = 0,
        HighestSingleSpinWinRate = 1,
        HighestTotalofAllBetAmounts = 2,
        HighestTotalofAllWinAmounts = 3,
        HighestTotalofAllWinRates = 4,
    }
}
