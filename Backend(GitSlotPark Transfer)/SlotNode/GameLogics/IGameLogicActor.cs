using Akka.Actor;
using Akka.Event;
using GITProtocol;
using GITProtocol.Utils;
using MongoDB.Bson;
using SlotGamesNode.Database;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SlotGamesNode.GameLogics
{
    public abstract class IGameLogicActor : ReceiveActor
    {
        protected static readonly string    _logSplitString = "##";
        protected GameConfig                _config         = null;
        protected GAMEID                    _gameID;
        protected readonly ILoggingAdapter  _logger         = Context.GetLogger();

        protected Dictionary<string, IActorRef> _dicEnteredUsers = new Dictionary<string, IActorRef>();
        protected IActorRef _dbReader       = null;
        protected IActorRef _dbWriter       = null;
        protected IActorRef _redisWriter    = null;
        protected float     _randomJackpot;
        protected static RealExtensions.Epsilon _epsilion = new RealExtensions.Epsilon(0.001);
        protected GITMessage                _bonusSendMessage;
        protected double                    _rewardedBonusMoney;
        protected bool                      _isRewardedBonus;
        protected string                    _roundID;
        
        protected Dictionary<int, double> _websitePayoutRates = new Dictionary<int, double>();

        protected string GameName { get; set; }

        public IGameLogicActor()
        {
            Receive<DBProxyInform>(inform =>
            {
                _dbReader       = inform.DBReader;
                _dbWriter       = inform.DBWriter;
                _redisWriter    = inform.RedisWriter;
            });
            ReceiveAsync<EnterGameRequest>      (onEnterUserMessage);
            ReceiveAsync<ExitGameRequest>       (onExitUserMessage);
            ReceiveAsync<FromUserMessage>       (onProcMessage);
            Receive<string>                     (onProcCommand);
            ReceiveAsync<BsonDocument>          (onLoadSpinData);
            ReceiveAsync<PerformanceTestRequest>(onPerformanceTest);
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
                _config = new GameConfig(97.0);

            if (PayoutConfigSnapshot.Instance.WebsitePayoutConfigs.ContainsKey(_gameID))
                _websitePayoutRates = PayoutConfigSnapshot.Instance.WebsitePayoutConfigs[_gameID];
            else
                _websitePayoutRates = new Dictionary<int, double>();
        }

        protected void sumUpWebsiteBetWin(int websiteID,double betMoney,double winMoney,int poolIndex = 0)
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

        protected async Task<bool> checkWebsitePayoutRate(int websiteID,double betMoney,double winMoney,int poolIndex = 0)
        {
            if (betMoney == 0.0 && winMoney == 0.0)
                return true;

            bool result = await PayoutPoolConfig.Instance.PoolActor.Ask<bool>(new CheckWebsitePayoutRequest(websiteID, betMoney, winMoney, poolIndex));
            return result;
        }

        private async Task onEnterUserMessage(EnterGameRequest message)
        {
            _dicEnteredUsers[message.UserID] = message.UserActor;

            //해당유저의 게임리력정보를 로드한다.(테이블게임들에서 사용함)
            bool isLoadSuccess = await loadUserHistoricalData(message.AgentID, message.UserID, message.NewEnter);

            //리력정보를 읽는 과정에 Redis오유가 발생했다면 
            if (!isLoadSuccess)
            {
                //입장실패메세지를 보낸다.
                Sender.Tell(new EnterGameResponse((int)_gameID, Self, 1));
            }
            else
            {
                EnterGameResponse response = new EnterGameResponse((int)_gameID, Self, 0);
                //게임에 새로 진입할 경우
                if (message.NewEnter)
                    await onUserEnterGame(message.AgentID, message.UserID);

                Sender.Tell(response); //게임입장성공메세지를 보낸다.
            }
        }

        protected virtual async Task onExitUserMessage(ExitGameRequest message)
        {
            await onUserExitGame(message.WebsiteID, message.UserID, message.UserRequested);
            _dicEnteredUsers.Remove(message.UserID);
            Sender.Tell(new ExitGameResponse());
        }

        private async Task onProcMessage(FromUserMessage message)
        {
            _bonusSendMessage   = null;
            _isRewardedBonus    = false;
            _rewardedBonusMoney = 0.0;
            
            await onProcMessage(message.UserID, message.WebsiteID, message.Message, message.Bonus, message.UserBalance, message.Currency);
        }

        protected virtual async Task onUserEnterGame(int agentID, string strUserID)
        {
        }

        protected virtual async Task onUserExitGame(int agentID, string strUserID, bool userRequested)
        {
        }

        protected virtual async Task onProcMessage(string strUserID,int websiteID,GITMessage message,UserBonus userBonus,double userBalance,Currencies currency)
        {
        }

        protected virtual async Task<bool> loadUserHistoricalData(int agentID,string strUserID,bool isNewEnter)
        {
            return true;
        }

        protected virtual GITMessage createJackpotBonusMessage(byte jacpotType, double bonus)
        {
            return null;
        }

        protected virtual void checkBonus(UserBonus userBonus, string strUserID)
        {
        }
    }
}
