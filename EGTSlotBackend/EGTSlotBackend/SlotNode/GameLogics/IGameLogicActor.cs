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

        #region Transaction Info 
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

            ReceiveAsync<EnterGameRequest>      (OnEnterUserMessage);

            ReceiveAsync<ExitGameRequest>       (OnExitUserMessage);

            ReceiveAsync<FromUserMessage>       (OnProcMessage);
            Receive<string>                     (OnProcCommand);
            ReceiveAsync<BsonDocument>          (OnLoadSpinData);
        }

        protected override void PostStop()
        {
            Context.System.EventStream.Unsubscribe(Self);
            base.PostStop();
        }
        protected virtual void OnProcCommand(string strCommand)
        {
            if (strCommand == "loadSetting")
                LoadSetting();
        }
        protected virtual async Task OnLoadSpinData(BsonDocument infoDocument)
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
        protected void SumUpWebsiteBetWin(int websiteID, double betMoney, double winMoney, int poolIndex = 0)
        {
            PayoutPoolConfig.Instance.PoolActor.Tell(new SumUpWebsiteBetWinRequest(websiteID, _gameID, betMoney, winMoney, poolIndex));
        }
        protected double GetPayoutRate(int websiteID)
        {
            double payoutRate = _config.PayoutRate;
            if (_websitePayoutRates.ContainsKey(websiteID))
                payoutRate = _websitePayoutRates[websiteID];
            return payoutRate;
        }
        protected async Task<bool> CheckWebsitePayoutRate(int websiteID, double betMoney, double winMoney, int poolIndex = 0)
        {
            if (betMoney == 0.0 && winMoney == 0.0)
                return true;

            double payoutRate = GetPayoutRate(websiteID);
            bool result = await PayoutPoolConfig.Instance.PoolActor.Ask<bool>(new CheckWebsitePayoutRequest(websiteID, _gameID, betMoney, winMoney, poolIndex, payoutRate));
            return result;
        }

        #region Proc Messages
        private async Task OnEnterUserMessage(EnterGameRequest message)
        {
            string strGlobalUserID = string.Format("{0}_{1}", message.AgentID, message.UserID);
            _dicEnteredUsers[strGlobalUserID] = message.UserActor;

            bool isLoadSuccess = await LoadUserHistoricalData(strGlobalUserID, message.NewEnter);

            if(!isLoadSuccess)
            {
                Sender.Tell(new EnterGameResponse((int) _gameID, Self, 1));
                return;
            }

            EnterGameResponse response = new EnterGameResponse((int) _gameID, Self, 0);
            if (message.NewEnter)
                await OnUserEnterGame(message.AgentID, message.UserID);

            Sender.Tell(response);
        }
        protected virtual async Task OnExitUserMessage(ExitGameRequest message)
        {
            await OnUserExitGame(message.UserID, message.UserRequested);
            _dicEnteredUsers.Remove(message.UserID);
            Sender.Tell(new ExitGameResponse());
        }
        private async Task OnProcMessage(FromUserMessage message)
        {
            await OnProcMessage(message.UserID, message.WebsiteID, message.Message, message.UserBalance, message.Currency);
        }
        #endregion

        #region Virtual Funcs
        protected virtual async Task OnUserEnterGame(int websiteID, string strGlobalUserID)
        {

        }
        protected virtual async Task OnUserExitGame(string strUserID, bool userRequested)
        {

        }
        protected virtual async Task OnProcMessage(string strUserID, int websiteID, GITMessage message, double userBalance, Currencies currency)
        {

        }
        protected virtual async Task<bool> LoadUserHistoricalData(string strGlobalUserID, bool isNewEnter)
        {
            return true;
        }
        #endregion
    }
}
