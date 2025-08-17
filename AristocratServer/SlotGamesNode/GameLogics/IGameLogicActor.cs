using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Akka.Actor;
using Akka.Event;
using GITProtocol;
using GITProtocol.Utils;
using SlotGamesNode.Database;
using PCGSharp;

namespace SlotGamesNode.GameLogics
{
    public abstract class IGameLogicActor : ReceiveActor
    {
        protected static int                    PoolCount       = 2;
        protected GameConfig                    _config         = null;
        protected GAMEID                        _gameID;

        protected readonly ILoggingAdapter      _logger         = Logging.GetLogger(Context);

        protected double[] _totalBets = new double[PoolCount];
        protected double[] _totalWins = new double[PoolCount];

        protected string                        GameName { get; set; }

        protected Dictionary<string, IActorRef> _dicEnteredUsers = new Dictionary<string, IActorRef>();
        protected IActorRef                     _dbReader       = null;
        protected IActorRef                     _dbWriter       = null;
        protected IActorRef                     _redisWriter    = null;


        protected static RealExtensions.Epsilon _epsilion       = new RealExtensions.Epsilon(0.001);


        #region 보너스정보
        protected GITMessage    _bonusSendMessage;
        protected double        _rewardedBonusMoney;
        protected bool          _isRewardedBonus;
        protected bool          _isRewardedCashback;
        #endregion

        public IGameLogicActor()
        {
            Receive<DBProxyInform>(inform =>
            {
                _dbReader    = inform.DBReader;
                _dbWriter    = inform.DBWriter;
                _redisWriter = inform.RedisWriter;
            });

            ReceiveAsync<EnterGameRequest>  (onEnterUserMessage);

            ReceiveAsync<ExitGameRequest>   (onExitUserMessage);

            ReceiveAsync<FromUserMessage>   (onProcMessage);

            Receive<PayoutConfigUpdated>    (_ => onPayoutConfigUpdated());

            Receive<string>                 (onProcCommand);
        }

        protected override void PostStop()
        {
            Context.System.EventStream.Unsubscribe(Self);
            base.PostStop();
        }
        protected virtual void onProcCommand(string strCommand)
        {
            if (strCommand == "loadSetting")
                LoadSetting();
        }
        protected virtual void LoadSetting()
        {            
            if (PayoutConfigSnapshot.Instance.PayoutConfigs.ContainsKey(_gameID))
                _config = PayoutConfigSnapshot.Instance.PayoutConfigs[_gameID];
            else
                _config = new GameConfig(97.0f, 0.0f, 1000.0f, false);
        }
        
        #region 메세지처리 함수들
        private async Task onEnterUserMessage(EnterGameRequest message)
        {
            _dicEnteredUsers[message.UserID] = message.UserActor;

            bool isLoadSuccess = await loadUserHistoricalData(message.UserID, message.NewEnter);

            if(!isLoadSuccess)
            {
                Sender.Tell(new EnterGameResponse((int) _gameID, Self, 1));
                return;
            }

            EnterGameResponse response = new EnterGameResponse((int) _gameID, Self, 0);
            if (message.NewEnter)
                await onUserEnterGame(message.UserID);

            Sender.Tell(response);  
        }
        protected virtual async Task onExitUserMessage(ExitGameRequest message)
        {
            await onUserExitGame(message.UserID, message.UserRequested);
            _dicEnteredUsers.Remove(message.UserID);
            Sender.Tell(new ExitGameResponse());
        }
        private async Task onProcMessage(FromUserMessage message)
        {
            _bonusSendMessage   = null;
            _isRewardedBonus    = false;
            _rewardedBonusMoney = 0.0;

            if (message.MoneyMode == 1)
                await onProcMessage(message.UserID, message.CompanyID, message.Message, message.Bonus, Math.Round(message.UserBalance * 100.0, 2), message.Currency, message.MoneyMode);
            else
                await onProcMessage(message.UserID, message.CompanyID, message.Message, message.Bonus, message.UserBalance, message.Currency, message.MoneyMode);
        }
        #endregion

        #region 가상함수들
        protected virtual async Task onUserEnterGame(string strUserID)
        {

        }
        protected virtual async Task onUserExitGame(string strUserID, bool userRequested)
        {

        }
        protected virtual async Task onProcMessage(string strUserID, int companyID, GITMessage message,UserBonus bonus, double userBalance, Currencies currency, int agentMoneyMode)
        {

        }
        protected virtual async Task<bool> loadUserHistoricalData(string strUserID, bool isNewEnter)
        {
            return true;
        }        

        protected virtual void onPayoutConfigUpdated()
        {

        }
        #endregion

        protected void resetPayoutPool()
        {
            for(int i = 0; i < PoolCount; i++)
            {
                _totalBets[i] = 0.0;
                _totalWins[i] = 0.0;
            }
        }
        protected bool checkPayoutRate(double betMoney, double winMoney, int poolIndex = 0)
        {
            if (betMoney == 0.0 && winMoney == 0.0)
                return true;

            double totalBet     = _totalBets[poolIndex] + betMoney;
            double totalWin     = _totalWins[poolIndex] + winMoney;

            double maxTotalWin = totalBet * _config.PayoutRate / 100.0 + _config.PoolRedundency;
            if (totalWin > maxTotalWin)
                return false;

            _totalBets[poolIndex] = totalBet;
            _totalWins[poolIndex] = totalWin;
            return true;
        }

        protected void sumUpBetWin(double betMoney, double winMoney, int poolIndex = 0)
        {
            _totalBets[poolIndex] += betMoney;
            _totalWins[poolIndex] += winMoney;
        }

        protected bool checkEventRate()
        {
            double random = Pcg.Default.NextDouble() * 100.0;
            if (random < _config.EventRate)
                return true;

            return false;
        }
    }

    public class DBProxyInform
    {
        public IActorRef DBReader   { get; private set; }
        public IActorRef DBWriter   { get; private set; }
        public IActorRef RedisWriter { get; private set; }
        public DBProxyInform(IActorRef dbReader, IActorRef dbWriter, IActorRef redisWriter)
        {
            this.DBReader       = dbReader;
            this.DBWriter       = dbWriter;
            this.RedisWriter    = redisWriter;
        }
    }
}
