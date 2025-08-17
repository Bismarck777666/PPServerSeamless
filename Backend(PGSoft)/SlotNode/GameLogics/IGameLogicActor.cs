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
using SlotGamesNode.GameLogics;
using SlotGamesNode;
using MongoDB.Bson;

namespace SlotGamesNode.GameLogics
{
    public abstract class IGameLogicActor : ReceiveActor
    {
        protected GameConfig    _config = null;
        protected GAMEID        _gameID;

        protected readonly ILoggingAdapter _logger = Logging.GetLogger(Context);

        protected string GameName { get; set; }

        protected Dictionary<string, IActorRef> _dicEnteredUsers = new Dictionary<string, IActorRef>();
        protected IActorRef _dbReader       = null;
        protected IActorRef _dbWriter       = null;
        protected IActorRef _redisWriter    = null;

        protected static RealExtensions.Epsilon _epsilion           = new RealExtensions.Epsilon(0.001);
        
        #region 보너스정보
        protected GITMessage    _bonusSendMessage;
        protected double        _rewardedBonusMoney;
        protected bool          _isRewardedBonus;
        #endregion

        protected Dictionary<int, double>       _websitePayoutRates = new Dictionary<int, double>();
        public IGameLogicActor()
        {
            Receive<DBProxyInform>(inform =>
            {
                _dbReader       = inform.DBReader;
                _dbWriter       = inform.DBWriter;
                _redisWriter    = inform.RedisWriter;
            });

            ReceiveAsync<EnterGameRequest>  (onEnterUserMessage);
            ReceiveAsync<ExitGameRequest>   (onExitUserMessage);
            ReceiveAsync<FromUserMessage>   (onProcMessage);
            ReceiveAsync<BsonDocument>      (onLoadSpinData);
            Receive<string>(onProcCommand);
        }

        protected virtual async Task onLoadSpinData(BsonDocument infoDocument)
        {

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
                _config = new GameConfig(97.0f);

            if (PayoutConfigSnapshot.Instance.WebsitePayoutConfigs.ContainsKey(_gameID))
                _websitePayoutRates = PayoutConfigSnapshot.Instance.WebsitePayoutConfigs[_gameID];
            else
                _websitePayoutRates = new Dictionary<int, double>();
        }

        protected virtual async Task onEnterUserMessage(EnterGameRequest message)
        {
            string strGlobalUserID = string.Format("{0}_{1}", message.AgentID, message.UserID);
            _dicEnteredUsers[strGlobalUserID] = message.UserActor;
            bool isLoadSuccess = await loadUserHistoricalData(strGlobalUserID, message.NewEnter);

            if (!isLoadSuccess)
            {
                Sender.Tell(new EnterGameResponse(_gameID, Self, 1, null, null));
                return;
            }
            onUserEnterGame(strGlobalUserID, message.UserBalance, message.Currency);
        }
        protected virtual async Task onExitUserMessage(ExitGameRequest message)
        {
            string strGlobalUserID = string.Format("{0}_{1}", message.WebsiteID, message.UserID);
            await onUserExitGame(strGlobalUserID, message.UserRequested);
            _dicEnteredUsers.Remove(strGlobalUserID);
            Sender.Tell(new ExitGameResponse());
        }
        private async Task onProcMessage(FromUserMessage message)
        {
            //보너스정보들을 초기화한다.
            _bonusSendMessage   = null;
            _isRewardedBonus    = false;
            _rewardedBonusMoney = 0.0;

            await onProcMessage(message.UserID, message.WebsiteID, message.Message, message.Bonus, message.UserBalance, message.Currency, message.IsAffiliate);
        }

        protected virtual void onUserEnterGame(string strGlobalUserID, double userBalance, Currencies currency)
        {
            EnterGameResponse response = new EnterGameResponse(_gameID, Self, 0, null, null);
            Sender.Tell(response); 
        }
        protected virtual async Task onUserExitGame(string strGlobalUserID, bool userRequested)
        {

        }
        protected virtual async Task onProcMessage(string strUserID, int websiteID, GITMessage message, UserBonus userBonus, double userBalance, Currencies currency, bool isAffiliate)
        {

        }
        protected virtual async Task<bool> loadUserHistoricalData(string strUserID, bool isNewEnter)
        {
            return true;
        }

        protected virtual void onPayoutConfigUpdated()
        {

        }

        protected void sumUpWebsiteBetWin(int websiteID, double betMoney, double winMoney, int poolIndex = 0)
        {
            PayoutPoolConfig.Instance.PoolActor.Tell(new SumUpWebsiteBetWinRequest(websiteID, betMoney, winMoney, poolIndex));
        }
        protected double getPayoutRate(int websiteID)
        {
            double payoutRate = _config.PayoutRate;
            if (_websitePayoutRates.ContainsKey(websiteID))
                payoutRate = _websitePayoutRates[websiteID];
            return payoutRate;
        }
        protected async Task<bool> checkWebsitePayoutRate(int websiteID, double betMoney, double winMoney, int poolIndex = 0)
        {
            if (betMoney == 0.0 && winMoney == 0.0)
                return true;

            bool result = await PayoutPoolConfig.Instance.PoolActor.Ask<bool>(new CheckWebsitePayoutRequest(websiteID, betMoney, winMoney, poolIndex));
            return result;
        }

    }

    public class DBProxyInform
    {
        public IActorRef DBReader { get; private set; }
        public IActorRef DBWriter { get; private set; }
        public IActorRef RedisWriter { get; private set; }
        public DBProxyInform(IActorRef dbReader, IActorRef dbWriter, IActorRef redisWriter)
        {
            this.DBReader = dbReader;
            this.DBWriter = dbWriter;
            this.RedisWriter = redisWriter;
        }
    }
}
