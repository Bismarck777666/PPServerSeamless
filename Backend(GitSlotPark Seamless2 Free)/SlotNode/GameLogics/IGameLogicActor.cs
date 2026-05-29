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
using MongoDB.Bson;

namespace SlotGamesNode.GameLogics
{
    public abstract class IGameLogicActor : ReceiveActor
    {
        protected static readonly string        _logSplitString = "##";
        protected GameConfig                    _config         = null;
        protected GAMEID                        _gameID;

        protected readonly ILoggingAdapter      _logger         = Logging.GetLogger(Context);

        protected string                        GameName { get; set; }

        protected Dictionary<string, IActorRef> _dicEnteredUsers = new Dictionary<string, IActorRef>();
        protected IActorRef                     _dbReader       = null;
        protected IActorRef                     _dbWriter       = null;
        protected IActorRef                     _redisWriter    = null;

        protected float _randomJackpot;

        protected static RealExtensions.Epsilon _epsilion       = new RealExtensions.Epsilon(0.001);

        #region 奖励信息
        protected GITMessage    _bonusSendMessage;
        protected double        _rewardedBonusMoney;
        protected bool          _isRewardedBonus;
        #endregion

        #region 交易信息
        protected string _roundID;
        #endregion

        protected Dictionary<int, double> _websitePayoutRates = new Dictionary<int, double>();

        public IGameLogicActor()
        {
            Receive<DBProxyInform>(inform =>
            {
                _dbReader    = inform.DBReader;
                _dbWriter    = inform.DBWriter;
                _redisWriter = inform.RedisWriter;
            });

            //用户游戏进入消息
            ReceiveAsync<EnterGameRequest>          (onEnterUserMessage);

            //用户游戏退出消息
            ReceiveAsync<ExitGameRequest>           (onExitUserMessage);

            //用户消息处理
            ReceiveAsync<FromUserMessage>           (onProcMessage);
            //免费旋转取消消息
            Receive<FreeSpinCancelRequest>          (onUserFreeSpinCancel);


            Receive<string>                         (onProcCommand);
            ReceiveAsync<BsonDocument>              (onLoadSpinData);
            ReceiveAsync<PerformanceTestRequest>    (onPerformanceTest);
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
        protected virtual async Task onLoadSpinData(BsonDocument infoDocument)
        {

        }
        protected virtual async Task onPerformanceTest(PerformanceTestRequest _)
        {

        }
        protected virtual void LoadSetting()
        {            
            if (PayoutConfigSnapshot.Instance.PayoutConfigs.ContainsKey(_gameID))
                _config = PayoutConfigSnapshot.Instance.PayoutConfigs[_gameID];
            else
                _config = new GameConfig(97.0f);

            if (PayoutConfigSnapshot.Instance.WebsitePayoutConfigs.ContainsKey(_gameID))
                _websitePayoutRates = PayoutConfigSnapshot.Instance.WebsitePayoutConfigs[_gameID];
            else
                _websitePayoutRates = new Dictionary<int, double>();

        }
        protected void sumUpWebsiteBetWin(int websiteID, double betMoney, double winMoney, int poolIndex = 0)
        {
            double payoutRate = getPayoutRate(websiteID, false);
            PayoutPoolConfig.Instance.PoolActor.Tell(new SumUpWebsiteBetWinRequest(websiteID, _gameID, betMoney, winMoney, poolIndex, payoutRate));
        }
        protected double getPayoutRate(int websiteID, bool isAffiliate)
        {
            if (isAffiliate)
                return 100.0;

            double payoutRate = _config.PayoutRate;
            if (_websitePayoutRates.ContainsKey(websiteID))
                payoutRate = _websitePayoutRates[websiteID];
            return payoutRate;
        }
        protected async Task<bool> checkWebsitePayoutRate(int websiteID, double betMoney, double winMoney, int poolIndex = 0)
        {
            if (betMoney == 0.0 && winMoney == 0.0)
                return true;

            double payoutRate = getPayoutRate(websiteID, false);
            bool result = await PayoutPoolConfig.Instance.PoolActor.Ask<bool>(new CheckWebsitePayoutRequest(websiteID, _gameID, betMoney, winMoney, poolIndex, payoutRate));
            return result;
        }
        
        #region 消息处理函数
        private async Task onEnterUserMessage(EnterGameRequest message)
        {
            _dicEnteredUsers[message.UserID] = message.UserActor;

            //加载该用户的游戏历史信息。(在桌面游戏中使用)
            bool isLoadSuccess = await loadUserHistoricalData(message.AgentID, message.UserID, message.NewEnter);

            //读取历史信息时发生Redis错误
            if(!isLoadSuccess)
            {
                //发送进入失败消息
                Sender.Tell(new EnterGameResponse((int) _gameID, Self, 1));
                return;
            }

            EnterGameResponse response = new EnterGameResponse((int) _gameID, Self, 0);
            //如果新进入游戏时
            if (message.NewEnter)
                await onUserEnterGame(message.AgentID, message.UserID);

            Sender.Tell(response);  //发送游戏入场成功消息。
        }
        protected virtual async Task onExitUserMessage(ExitGameRequest message)
        {
            await onUserExitGame(message.WebsiteID, message.UserID, message.UserRequested);
            _dicEnteredUsers.Remove(message.UserID);
            Sender.Tell(new ExitGameResponse());
        }
        protected virtual void onUserFreeSpinCancel(FreeSpinCancelRequest request)
        {

        }
        private async Task onProcMessage(FromUserMessage message)
        {
            //初始化奖励信息。
            _bonusSendMessage   = null;
            _isRewardedBonus    = false;
            _rewardedBonusMoney = 0.0;

            await onProcMessage(message.UserID, message.WebsiteID, message.Message, message.Bonus, message.UserBalance, message.Currency, message.IsAffiliate);
        }
        #endregion

        #region 虚函数
        protected virtual async Task onUserEnterGame(int agentID, string strUserID)
        {

        }
        protected virtual async Task onUserExitGame(int agentID, string strUserID, bool userRequested)
        {

        }

        protected virtual async Task onProcMessage(string strUserID, int websiteID,  GITMessage message, UserBonus userBonus, double userBalance, Currencies currency, bool isAffiliate)
        {

        }
        protected virtual async Task<bool> loadUserHistoricalData(int agentID, string strUserID, bool isNewEnter)
        {
            return true;
        }        
        #endregion

        protected virtual GITMessage createJackpotBonusMessage(byte jacpotType, double bonus)
        {
            return null;
        }
        protected virtual void checkBonus(UserBonus userBonus, string strUserID)
        {

        }
        protected virtual async Task<bool> subtractEventMoney(int websiteID, string strUserID, double totalWin)
        {
            try
            {
                bool result = await Context.System.ActorSelection("/user/apiWorker").Ask<bool>(new SubtractEventMoneyRequest(websiteID, strUserID, Math.Round(totalWin, 2)), TimeSpan.FromSeconds(5.0));
                return result;
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occurred in IGameLogicActor::subtractEventMoney {0}", ex);
                return false;
            }
        }
        protected virtual void addEventLeftMoney(int websiteID, string strUserID, double leftMoney)
        {
            try
            {
                //Context.System.ActorSelection("/user/apiWorker").Tell(new AddEventLeftMoneyRequest(websiteID, strUserID, Math.Round(leftMoney, 2)));
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occurred in IGameLogicActor::addEventLeftMoney {0}", ex);
            }
        }

    }

    
}
