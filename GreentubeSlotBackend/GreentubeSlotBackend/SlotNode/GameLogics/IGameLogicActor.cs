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

        protected string                        GameName        { get; set; }
        
        protected Dictionary<string, IActorRef> _dicEnteredUsers    = new Dictionary<string, IActorRef>();
        protected IActorRef                     _dbReader           = null;
        protected IActorRef                     _dbWriter           = null;
        protected IActorRef                     _redisWriter        = null;

        protected static RealExtensions.Epsilon _epsilion       = new RealExtensions.Epsilon(0.001);

        #region 
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

            ReceiveAsync<EnterGameRequest>      (onEnterUserMessage);

            ReceiveAsync<ExitGameRequest>       (onExitUserMessage);

            ReceiveAsync<FromUserMessage>       (onProcMessage);
            Receive<string>                     (onProcCommand);
            ReceiveAsync<BsonDocument>          (onLoadSpinData);
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
            PayoutPoolConfig.Instance.PoolActor.Tell(new SumUpWebsiteBetWinRequest(websiteID, _gameID, betMoney, winMoney, poolIndex));
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

            double payoutRate = getPayoutRate(websiteID);
            bool result = await PayoutPoolConfig.Instance.PoolActor.Ask<bool>(new CheckWebsitePayoutRequest(websiteID, _gameID, betMoney, winMoney, poolIndex, payoutRate));
            return result;
        }

        #region
        private async Task onEnterUserMessage(EnterGameRequest message)
        {
            string strGlobalUserID = string.Format("{0}_{1}", message.AgentID, message.UserID);
            _dicEnteredUsers[strGlobalUserID] = message.UserActor;

            bool isLoadSuccess = await loadUserHistoricalData(strGlobalUserID, message.NewEnter);

            if(!isLoadSuccess)
            {
                Sender.Tell(new EnterGameResponse((int) _gameID, Self, 1));
                return;
            }

            EnterGameResponse response = new EnterGameResponse((int) _gameID, Self, 0);
            if (message.NewEnter)
                await onUserEnterGame(message.AgentID, message.UserID);

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
            await onProcMessage(message.UserID, message.WebsiteID, message.Message, message.UserBalance, message.Currency);
        }
        #endregion

        #region
        protected virtual async Task onUserEnterGame(int websiteID, string strGlobalUserID)
        {

        }
        protected virtual async Task onUserExitGame(string strUserID, bool userRequested)
        {

        }
        protected virtual async Task onProcMessage(string strUserID, int websiteID, GITMessage message, double userBalance, Currencies currency)
        {

        }
        protected virtual async Task<bool> loadUserHistoricalData(string strGlobalUserID, bool isNewEnter)
        {
            return true;
        }
        #endregion
    }
}
