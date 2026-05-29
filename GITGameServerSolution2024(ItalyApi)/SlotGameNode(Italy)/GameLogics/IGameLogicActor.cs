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
        protected static readonly string _logSplitString = "##";
        protected static int PoolCount = 2;
        protected GameConfig _config = null;
        protected GAMEID _gameID;

        protected readonly ILoggingAdapter _logger = Logging.GetLogger(Context);

        #region 投注池信息（每个游戏最多有2个投注池：基本池、辅助池）
        protected double[] _totalBets = new double[PoolCount];
        protected double[] _totalWins = new double[PoolCount];
        #endregion

        protected string GameName { get; set; }

        protected Dictionary<string, IActorRef> _dicEnteredUsers = new Dictionary<string, IActorRef>();
        protected IActorRef _dbReader = null;
        protected IActorRef _dbWriter = null;
        protected IActorRef _redisWriter = null;

        protected static RealExtensions.Epsilon _epsilion = new RealExtensions.Epsilon(0.001);

        #region 奖励信息
        protected GITMessage _bonusSendMessage;
        protected double _rewardedBonusMoney;
        protected bool _isRewardedBonus;
        #endregion

        protected Dictionary<int, double> _agentPayoutRates = new Dictionary<int, double>();
        protected Dictionary<int, double[]> _agentTotalBets = new Dictionary<int, double[]>();
        protected Dictionary<int, double[]> _agentTotalWins = new Dictionary<int, double[]>();

        public IGameLogicActor()
        {
            Receive<DBProxyInform>(inform =>
            {
                _dbReader = inform.DBReader;
                _dbWriter = inform.DBWriter;
                _redisWriter = inform.RedisWriter;
            });

            //用户游戏进入消息
            ReceiveAsync<EnterGameRequest>(onEnterUserMessage);
            //用户游戏退出消息
            ReceiveAsync<ExitGameRequest>(onExitUserMessage);
            //用户消息处理
            ReceiveAsync<FromUserMessage>(onProcMessage);
            //支付设置更新消息
            Receive<PayoutConfigUpdated>(_ => onPayoutConfigUpdated());
            Receive<AgentPayoutConfigUpdated>(onAgentPayoutConfigUpdated);

            Receive<string>(onProcCommand);
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
                _config = new GameConfig(97.0f, 0.0f, 10.0f);

            if (PayoutConfigSnapshot.Instance.AgentPayoutConfigs.ContainsKey(_gameID))
                _agentPayoutRates = PayoutConfigSnapshot.Instance.AgentPayoutConfigs[_gameID];
            else
                _agentPayoutRates = new Dictionary<int, double>();
        }

        #region 消息处理函数
        private async Task onEnterUserMessage(EnterGameRequest message)
        {
            string strGlobalUserID = string.Format("{0}_{1}", message.AgentID, message.UserID);
            _dicEnteredUsers[strGlobalUserID] = message.UserActor;

            //加载该用户的游戏历史信息。(在桌面游戏中使用)
            bool isLoadSuccess = await loadUserHistoricalData(message.AgentID, message.UserID, message.NewEnter);

            //读取历史信息时发生Redis错误
            if (!isLoadSuccess)
            {
                //发送进入失败消息
                Sender.Tell(new EnterGameResponse((int)_gameID, Self, 1));
                return;
            }

            EnterGameResponse response = new EnterGameResponse((int)_gameID, Self, 0);
            //如果新进入游戏时
            if (message.NewEnter)
                await onUserEnterGame(message.UserID, message.Currency);

            Sender.Tell(response);  //发送游戏入场成功消息。
        }

        protected virtual async Task onExitUserMessage(ExitGameRequest message)
        {
            string strGlobalUserID = string.Format("{0}_{1}", message.AgentID, message.UserID);
            await onUserExitGame(message.AgentID, message.UserID, message.UserRequested);
            _dicEnteredUsers.Remove(strGlobalUserID);
            Sender.Tell(new ExitGameResponse());
        }

        private async Task onProcMessage(FromUserMessage message)
        {
            //初始化奖励信息。
            _bonusSendMessage = null;
            _isRewardedBonus = false;
            _rewardedBonusMoney = 0.0;

            await onProcMessage(message.UserID, message.AgentID, (CurrencyEnum)message.Currency, message.Message, message.Bonus, message.UserBalance, message.IsMustLose);
        }
        #endregion

        #region 虚函数
        protected virtual async Task onUserEnterGame(string strUserID, int currency)
        {

        }
        protected virtual async Task onUserExitGame(int agentId, string strUserID, bool userRequested)
        {

        }
        protected virtual async Task onProcMessage(string strUserID, int companyID, CurrencyEnum currency, GITMessage message, UserBonus userBonus, double userBalance, bool isMustLose)
        {

        }
        protected virtual async Task<bool> loadUserHistoricalData(int agentID, string strUserID, bool isNewEnter)
        {
            return true;
        }
        #endregion

        #region PayoutPool 相关函数
        protected double getPayoutRate(int agentID)
        {
            double payoutRate = _config.PayoutRate;
            if (_agentPayoutRates.ContainsKey(agentID))
                payoutRate = _agentPayoutRates[agentID];
            return payoutRate;
        }

        protected virtual void onPayoutConfigUpdated()
        {
            foreach (KeyValuePair<int, double[]> websiteTotalBet in _agentTotalBets)
                resetAgentPayoutPool(websiteTotalBet.Key);
        }

        protected void onAgentPayoutConfigUpdated(AgentPayoutConfigUpdated updated)
        {
            if (updated.ChangedPayout)
                resetAgentPayoutPool(updated.AgentID);
        }

        protected void resetAgentPayoutPool(int websiteID)
        {
            if (!_agentTotalBets.ContainsKey(websiteID) || !_agentTotalWins.ContainsKey(websiteID))
                return;

            for (int i = 0; i < PoolCount; ++i)
            {
                _agentTotalBets[websiteID][i] = 0.0;
                _agentTotalWins[websiteID][i] = 0.0;
            }
        }

        protected void resetPayoutPool()
        {
            for (int i = 0; i < PoolCount; i++)
            {
                _totalBets[i] = 0.0;
                _totalWins[i] = 0.0;
            }
        }

        protected bool checkPayoutRate(double betMoney, double winMoney, int poolIndex = 0)
        {
            if (betMoney == 0.0 && winMoney == 0.0)
                return true;

            double totalBet = _totalBets[poolIndex] + betMoney;
            double totalWin = _totalWins[poolIndex] + winMoney;

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
        #endregion

        protected virtual void checkBonus(UserBonus userBonus, string strUserID)
        {

        }

        protected virtual async Task<bool> subtractEventMoney(int agentID, string strUserID, double totalWin)
        {
            try
            {
                bool result = await Context.System.ActorSelection("/user/apiWorker").Ask<bool>(new SubtractEventMoneyRequest(agentID, strUserID, Math.Round(totalWin)), TimeSpan.FromSeconds(5.0));
                return result;
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occurred in IGameLogicActor::subtractEventMoney {0}", ex);
                return false;
            }
        }

        protected virtual void addEventLeftMoney(int agentID, string strUserID, double leftMoney)
        {
            try
            {
                Context.System.ActorSelection("/user/apiWorker").Tell(new AddEventLeftMoneyRequest(agentID, strUserID, Math.Round(leftMoney)));
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occurred in IGameLogicActor::addEventLeftMoney {0}", ex);
            }
        }
    }

    public class CurrencyObj
    {
        public string   CurrencyText    { get; set; }
        public string   CurrencySymbol  { get; set; }
        public int      Rate            { get; set; }
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
