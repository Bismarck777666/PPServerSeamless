using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Akka.Actor;
using SlotGamesNode.Database;
using GITProtocol;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.IO;
using GITProtocol.Utils;
using PCGSharp;

namespace SlotGamesNode.GameLogics
{
    class BaseBNGSlotGame : IGameLogicActor
    {
        protected string                            _providerName           = "booongo";
        //스핀디비관리액터
        protected IActorRef                         _spinDatabase           = null;
        protected double                            _spinDataDefaultBet     = 0.0f;

        protected int                               _normalMaxID            = 0;
        protected int                               _naturalSpinCount       = 0;
        protected SortedDictionary<double, int>     _naturalSpinOddProbs    = new SortedDictionary<double, int>();
        protected SortedDictionary<double, int[]>   _totalSpinOddIds        = new SortedDictionary<double, int[]>();
        protected List<int>                         _emptySpinIDs           = new List<int>();

        //프리스핀구매기능이 있을떄만 필요하다. 디비안의 모든 프리스핀들의 오드별 아이디어레이
        protected SortedDictionary<double, int[]>   _totalFreeSpinOddIds    = new SortedDictionary<double, int[]>();
        protected int                               _freeSpinTotalCount     = 0;
        protected int                               _minFreeSpinTotalCount  = 0;
        protected double                            _totalFreeSpinWinRate   = 0.0; //스핀디비안의 모든 프리스핀들의 배당평균값
        protected double                            _minFreeSpinWinRate     = 0.0; //구매금액의 20% - 50%사이에 들어가는 모든 프리스핀들의 평균배당값

        //앤티베팅기능이 있을때만 필요하다.(앤티베팅시 감소시켜야할 빈스핀의 갯수)
        protected int                               _anteBetMinusZeroCount  = 0;

        //매유저의 베팅정보 
        protected Dictionary<string, BaseBNGSlotBetInfo>    _dicUserBetInfos                = new Dictionary<string, BaseBNGSlotBetInfo>();

        //유정의 마지막결과정보
        protected Dictionary<string, BaseBNGSlotSpinResult> _dicUserResultInfos             = new Dictionary<string, BaseBNGSlotSpinResult>();

        //백업정보
        protected Dictionary<string, BaseBNGSlotSpinResult> _dicUserLastBackupResultInfos   = new Dictionary<string, BaseBNGSlotSpinResult>();
        protected Dictionary<string, byte[]>                _dicUserLastBackupBetInfos      = new Dictionary<string, byte[]>();
        protected Dictionary<string, byte[]>                _dicUserLastBackupHistory       = new Dictionary<string, byte[]>();

        protected virtual int FreeSpinTypeCount
        {
            get
            {
                return 0; //유저가 선택가능한 프리스핀종류수
            }
        }
        protected virtual bool HasPurEnableOption
        {
            get { return false; }
        }
        protected virtual bool HasSelectableFreeSpin
        {
            get
            {
                return false;
            }
        }
        protected virtual string SymbolName
        {
            get
            {
                return "";
            }
        }
        protected virtual int ClientReqLineCount
        {
            get
            {
                return 20;
            }
        }
        protected virtual string InitResultString
        {
            get
            {
                return "";
            }
        }
        protected virtual string SettingString
        {
            get
            {
                return "";
            }
        }
        protected virtual bool SupportPurchaseFree
        {
            get { return false; }
        }
        protected virtual double PurchaseFreeMultiple
        {
            get { return 0.0; }
        }
        protected virtual bool SupportMoreBet
        {
            get { return false; }
        }
        protected virtual double MoreBetMultiple
        {
            get { return 0.0; }
        }
        
        public BaseBNGSlotGame()
        {
            ReceiveAsync<LoadSpinDataRequest>(onLoadSpinData);
        }
        
        protected override void LoadSetting()
        {
            base.LoadSetting();
            initGameData();

            if (PayoutConfigSnapshot.Instance.AgentPayoutConfigs.ContainsKey(_gameID))
                _agentPayoutRates = PayoutConfigSnapshot.Instance.AgentPayoutConfigs[_gameID];
            else
                _agentPayoutRates = new Dictionary<int, double>();
        }
        
        protected void sumUpCompanyBetWin(int agentID, double betMoney, double winMoney, int poolIndex = 0)
        {
            if (agentID == 0)
            {
                _totalBets[poolIndex] += betMoney;
                _totalWins[poolIndex] += winMoney;
            }
            else
            {
                if (!_agentTotalBets.ContainsKey(agentID))
                    _agentTotalBets[agentID] = new double[PoolCount];

                if (!_agentTotalWins.ContainsKey(agentID))
                    _agentTotalWins[agentID] = new double[PoolCount];

                _agentTotalBets[agentID][poolIndex] += betMoney;
                _agentTotalWins[agentID][poolIndex] += winMoney;
            }
        }
        
        protected bool checkCompanyPayoutRate(int companyID, double betMoney, double winMoney, int poolIndex = 0)
        {
            if (betMoney == 0.0 && winMoney == 0.0)
                return true;

            if (companyID == 0)
            {
                double totalBet = _totalBets[poolIndex] + betMoney;
                double totalWin = _totalWins[poolIndex] + winMoney;

                double maxTotalWin = totalBet * _config.PayoutRate / 100.0 + _config.PoolRedundency;
                if (totalWin > maxTotalWin)
                    return false;

                _totalBets[poolIndex] = totalBet;
                _totalWins[poolIndex] = totalWin;
                return true;
            }
            else
            {
                if (!_agentTotalBets.ContainsKey(companyID))
                    _agentTotalBets[companyID] = new double[PoolCount];

                if (!_agentTotalWins.ContainsKey(companyID))
                    _agentTotalWins[companyID] = new double[PoolCount];

                double companyPayoutRate = _config.PayoutRate;
                if (_agentPayoutRates.ContainsKey(companyID))
                    companyPayoutRate = _agentPayoutRates[companyID];

                double totalBet = _agentTotalBets[companyID][poolIndex] + betMoney;
                double totalWin = _agentTotalWins[companyID][poolIndex] + winMoney;

                double maxTotalWin = totalBet * companyPayoutRate / 100.0 + _config.PoolRedundency;
                if (totalWin > maxTotalWin)
                    return false;

                _agentTotalBets[companyID][poolIndex] = totalBet;
                _agentTotalWins[companyID][poolIndex] = totalWin;
                return true;
            }
        }
        
        protected void resetCompanyPayoutPool(int agentID)
        {
            if (!_agentTotalBets.ContainsKey(agentID) || !_agentTotalWins.ContainsKey(agentID))
                return;

            for (int i = 0; i < PoolCount; i++)
            {
                _agentTotalBets[agentID][i] = 0.0;
                _agentTotalWins[agentID][i] = 0.0;
            }
        }

        protected virtual void initGameData()
        {
        }

        protected async Task onLoadSpinData(LoadSpinDataRequest request)
        {
            bool result = await loadSpinData();
            Sender.Tell(result);
        }
        
        protected virtual async Task<bool> loadSpinData()
        {
            try
            {
                _spinDatabase   = Context.ActorOf(Akka.Actor.Props.Create(() => new SpinDatabase(_providerName, this.GameName)), "spinDatabase");
                bool isSuccess  = await _spinDatabase.Ask<bool>("initialize", TimeSpan.FromSeconds(5.0));
                if (!isSuccess)
                {
                    _logger.Error("couldn't load spin data of game {0}", this.GameName);
                    return false;
                }

                ReadSpinInfoResponse response = await _spinDatabase.Ask<ReadSpinInfoResponse>(new ReadSpinInfoRequest(HasSelectableFreeSpin ? SpinDataTypes.SelFreeSpin : SpinDataTypes.Normal), TimeSpan.FromSeconds(30.0));
                if (response == null)
                {
                    _logger.Error("couldn't load spin odds information of game {0}", this.GameName);
                    return false;
                }

                _normalMaxID            = response.NormalMaxID;
                _naturalSpinOddProbs    = new SortedDictionary<double, int>();
                _spinDataDefaultBet     = response.DefaultBet;
                _naturalSpinCount       = 0;
                _emptySpinIDs           = new List<int>();

                Dictionary<double, List<int>> totalSpinOddIds   = new Dictionary<double, List<int>>();
                Dictionary<double, List<int>> freeSpinOddIds    = new Dictionary<double, List<int>>();

                double freeSpinTotalOdd     = 0.0;
                double minFreeSpinTotalOdd  = 0.0;
                int freeSpinTotalCount      = 0;
                int minFreeSpinTotalCount   = 0;
                for (int i = 0; i < response.SpinBaseDatas.Count; i++)
                {
                    SpinBaseData spinBaseData = response.SpinBaseDatas[i];
                    if (spinBaseData.ID <= response.NormalMaxID)
                    {
                        _naturalSpinCount++;
                        if (_naturalSpinOddProbs.ContainsKey(spinBaseData.Odd))
                            _naturalSpinOddProbs[spinBaseData.Odd]++;
                        else
                            _naturalSpinOddProbs[spinBaseData.Odd] = 1;
                    }
                    if (!totalSpinOddIds.ContainsKey(spinBaseData.Odd))
                        totalSpinOddIds.Add(spinBaseData.Odd, new List<int>());

                    if (spinBaseData.SpinType == 0 && spinBaseData.Odd == 0.0)
                        _emptySpinIDs.Add(spinBaseData.ID);

                    totalSpinOddIds[spinBaseData.Odd].Add(spinBaseData.ID);
                    if (SupportPurchaseFree && !HasPurEnableOption && spinBaseData.SpinType == 1)
                    {
                        freeSpinTotalCount++;
                        freeSpinTotalOdd += spinBaseData.Odd;
                        if (!freeSpinOddIds.ContainsKey(spinBaseData.Odd))
                            freeSpinOddIds.Add(spinBaseData.Odd, new List<int>());
                        freeSpinOddIds[spinBaseData.Odd].Add(spinBaseData.ID);

                        if (spinBaseData.Odd >= PurchaseFreeMultiple * 0.2 && spinBaseData.Odd <= PurchaseFreeMultiple * 0.5)
                        {
                            minFreeSpinTotalCount++;
                            minFreeSpinTotalOdd += spinBaseData.Odd;
                        }
                    }
                }
                
                _totalSpinOddIds = new SortedDictionary<double, int[]>();
                foreach (KeyValuePair<double, List<int>> pair in totalSpinOddIds)
                    _totalSpinOddIds.Add(pair.Key, pair.Value.ToArray());

                if (SupportPurchaseFree && HasPurEnableOption)
                {
                    //따로 읽는다.
                    List<SpinBaseData> freeSpinDatas = await _spinDatabase.Ask<List<SpinBaseData>>(new ReadSpinInfoPurEnabledRequest(), TimeSpan.FromSeconds(30.0));
                    for (int i = 0; i < freeSpinDatas.Count; i++)
                    {
                        freeSpinTotalOdd += freeSpinDatas[i].Odd;
                        freeSpinTotalCount++;

                        if (!freeSpinOddIds.ContainsKey(freeSpinDatas[i].Odd))
                            freeSpinOddIds.Add(freeSpinDatas[i].Odd, new List<int>());
                        freeSpinOddIds[freeSpinDatas[i].Odd].Add(freeSpinDatas[i].ID);
                        if (freeSpinDatas[i].Odd >= PurchaseFreeMultiple * 0.2 && freeSpinDatas[i].Odd <= PurchaseFreeMultiple * 0.5)
                        {
                            minFreeSpinTotalCount++;
                            minFreeSpinTotalOdd += freeSpinDatas[i].Odd;
                        }
                    }
                }
                
                if (SupportPurchaseFree)
                {
                    _totalFreeSpinOddIds = new SortedDictionary<double, int[]>();
                    foreach (KeyValuePair<double, List<int>> pair in freeSpinOddIds)
                        _totalFreeSpinOddIds.Add(pair.Key, pair.Value.ToArray());

                    _freeSpinTotalCount     = freeSpinTotalCount;
                    _minFreeSpinTotalCount  = minFreeSpinTotalCount;
                    _totalFreeSpinWinRate   = freeSpinTotalOdd / freeSpinTotalCount;
                    _minFreeSpinWinRate     = minFreeSpinTotalOdd / minFreeSpinTotalCount;

                    if (_totalFreeSpinWinRate <= _minFreeSpinWinRate || _minFreeSpinTotalCount == 0)
                        _logger.Error("min freespin rate doesn't satisfy condition {0}", GameName);
                }

                if (SupportMoreBet)
                {
                    int naturalEmptyCount = _naturalSpinOddProbs[0.0];
                    _anteBetMinusZeroCount = (int)((1.0 - 1.0 / MoreBetMultiple) * (double)_naturalSpinCount);

                    double moreBetWinRate = 0.0;
                    foreach (KeyValuePair<double, int> pair in _naturalSpinOddProbs)
                    {
                        moreBetWinRate += (pair.Key * pair.Value / (_naturalSpinCount - _anteBetMinusZeroCount));
                    }
                    if (_anteBetMinusZeroCount > naturalEmptyCount)
                        _logger.Error("More Bet Probabily calculation doesn't work in {0}", GameName);
                }

                if (SupportPurchaseFree && PurchaseFreeMultiple > _totalFreeSpinWinRate)
                    _logger.Error("freespin win rate doesn't satisfy condition {0}", GameName);

                double winRate = 0.0;
                foreach (KeyValuePair<double, int> pair in _naturalSpinOddProbs)
                    winRate += (pair.Key * pair.Value / _naturalSpinCount);

                return true;
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occured in loading spin data of game {0} {1}", GameName, ex);
                return false;
            }
        }

        #region 메세지처리함수들
        protected override async Task onProcMessage(string strUserID, int agentID, CurrencyEnum currency, GITMessage message, UserBonus userBonus, double userBalance, bool isMustLose)
        {
            string strGlobalUserID = string.Format("{0}_{1}", agentID, strUserID);
            if (message.MsgCode == (ushort)CSMSG_CODE.CS_BNG_DOLOGIN)
            {
                onDoLogin(strGlobalUserID, (int)currency, message, userBonus, userBalance, isMustLose);
            }
            else if (message.MsgCode == (ushort)CSMSG_CODE.CS_BNG_DOSTART)
            {
                onDoStart(agentID, strUserID, (int)currency, message, userBalance);
            }
            else if (message.MsgCode == (ushort)CSMSG_CODE.CS_BNG_DOSYNC)
            {
                onDoSync(strUserID, (int)currency, message, userBalance);
            }
            else if (message.MsgCode == (ushort)CSMSG_CODE.CS_BNG_DOPLAY)
            {
                await onDoPlay(strUserID, agentID, (int)currency, message, userBonus, userBalance, isMustLose);
            }
            else if (message.MsgCode == (ushort)CSMSG_CODE.CS_PP_NOTPROCDRESULT)
            {
                onDoUndoUserSpin(strGlobalUserID);
            }
        }
        protected virtual void onDoLogin(string strGlobalUserID,int currency, GITMessage message, UserBonus userBonus, double userBalance, bool isMustLose)
        {
            try
            {
                string strRequestID     = (string)message.Pop();
                string strToken         = (string)message.Pop();

                BNGLoginResponse response = new BNGLoginResponse();
                response.command                = "login";
                response.modes                  = new List<string>(new string[] { "auto", "play", "freebet" });
                response.request_id             = strRequestID;
                response.session_id             = strToken;
                response.status                 = new BNGStatus();
                response.status.code            = "OK";
                response.user = new BNGUser();
                response.user.huid              = strGlobalUserID;
                response.user.balance           = userBalance * 100.0;
                response.user.balance_version   = 10;
                response.user.currency          = new Currencies()._currencyInfo[currency].CurrencyText;
                response.user.show_balance      = true;
                GITMessage responseMessage      = new GITMessage((ushort)SCMSG_CODE.SC_BNG_DOLOGIN);
                responseMessage.Append(JsonConvert.SerializeObject(response));
                Sender.Tell(new ToUserMessage((int)_gameID, responseMessage), Self);
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occurred in BaseBNGSlotGame::onDoLogin GameID: {0}, {1}", _gameID, ex);
            }
        }
        protected virtual void onDoSync(string strGlobalUserID, int currency, GITMessage message, double userBalance)
        {
            try
            {
                string strRequestID = (string)message.Pop();
                string strToken     = (string)message.Pop();

                long balanceVersion = 10;
                if (_dicUserBetInfos.ContainsKey(strGlobalUserID))
                {
                    balanceVersion = _dicUserBetInfos[strGlobalUserID].BalanceVersion;
                    _dicUserBetInfos[strGlobalUserID].BalanceVersion += 1;
                }

                BNGLoginResponse response = new BNGLoginResponse();
                response.command                = "sync";
                response.modes                  = new List<string>(new string[] { "auto", "play", "freebet" });
                response.request_id             = strRequestID;
                response.session_id             = strToken;
                response.status = new BNGStatus();
                response.status.code            = "OK";
                response.user = new BNGUser();
                response.user.huid              = strGlobalUserID;
                response.user.balance           = userBalance * 100.0;
                response.user.balance_version   = balanceVersion;
                response.user.currency          = new Currencies()._currencyInfo[currency].CurrencyText;
                response.user.show_balance      = true;

                GITMessage reponseMessage = new GITMessage((ushort)SCMSG_CODE.SC_BNG_DOSYNC);
                reponseMessage.Append(JsonConvert.SerializeObject(response));
                Sender.Tell(new ToUserMessage((int)_gameID, reponseMessage), Self);
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occurred in BaseBNGSlotGame::onDoSync {0}", ex);
            }
        }
        protected virtual void addInfoForStart(dynamic context, string strUserID)
        {

        }
        protected virtual void onDoStart(int agentID, string strUserID,int currency,GITMessage message, double userBalance)
        {
            try
            {
                string strRequestID = (string)message.Pop();
                string strToken     = (string)message.Pop();

                long balanceVersion = 10;

                string strGlobalUserID = string.Format("{0}_{1}", agentID, strUserID);
                if (_dicUserBetInfos.ContainsKey(strGlobalUserID))
                {
                    balanceVersion = _dicUserBetInfos[strGlobalUserID].BalanceVersion;
                    _dicUserBetInfos[strGlobalUserID].BalanceVersion += 1;
                }

                BNGStartResponse response = new BNGStartResponse();
                response.command        = "start";
                response.modes          = new List<string>(new string[] { "auto", "play", "freebet" });
                response.request_id     = strRequestID;
                response.session_id     = strToken;
                response.status         = new BNGStatus();
                response.status.code    = "OK";
                response.settings       = JsonConvert.DeserializeObject<dynamic>(convertSettingString(currency));
                response.origin_data                    = new BNGStartOriginData();
                response.origin_data.quick_spin         = false;
                response.origin_data.data               = new BNGStartOriginData.Data();
                response.origin_data.data.quick_spin    = false;

                if (_dicUserResultInfos.ContainsKey(strGlobalUserID))
                    response.context = JsonConvert.DeserializeObject<dynamic>(_dicUserResultInfos[strGlobalUserID].ResultString);
                else
                    response.context = JsonConvert.DeserializeObject<dynamic>(convertInitResultString(currency));

                if (response.context.current == "bonus")
                    addInfoForStart(response.context, strGlobalUserID);

                response.user                   = new BNGUser();
                response.user.huid              = strGlobalUserID;
                response.user.balance           = userBalance * 100.0;
                response.user.balance_version   = balanceVersion;
                response.user.currency          = new Currencies()._currencyInfo[currency].CurrencyText;
                response.user.show_balance      = true;

                GITMessage reponseMessage = new GITMessage((ushort)SCMSG_CODE.SC_BNG_DOSTART);
                reponseMessage.Append(JsonConvert.SerializeObject(response));
                Sender.Tell(new ToUserMessage((int)_gameID, reponseMessage), Self);
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occurred in BaseBNGSlotGame::onDoStart {0}", ex);
            }
        }
        protected virtual string convertInitResultString(int currency)
        {
            dynamic resultContext = JsonConvert.DeserializeObject<dynamic>(InitResultString);
            if(!object.ReferenceEquals(resultContext["spins"], null) && !object.ReferenceEquals(resultContext["spins"]["bet_per_line"], null))
                resultContext["spins"]["bet_per_line"] = Convert.ToInt32(resultContext["spins"]["bet_per_line"]) * new Currencies()._currencyInfo[currency].Rate;
            
            if (!object.ReferenceEquals(resultContext["spins"], null) && !object.ReferenceEquals(resultContext["spins"]["round_bet"], null))
                resultContext["spins"]["round_bet"] = Convert.ToInt32(resultContext["spins"]["round_bet"]) * new Currencies()._currencyInfo[currency].Rate;
            
            if (!object.ReferenceEquals(resultContext["spins"], null) && !object.ReferenceEquals(resultContext["spins"]["bs_v"], null))
            {
                List<List<int>> bsVArray = new List<List<int>>();

                for(int i = 0; i < resultContext["spins"]["bs_v"].Count; i++)
                {
                    List<int> row = new List<int>();
                    for (int j = 0; j < resultContext["spins"]["bs_v"][i].Count; j++)
                    {
                        int value       = 0;
                        bool inNumeric  = int.TryParse(Convert.ToString(resultContext["spins"]["bs_v"][i][j]), out value);
                        //if (inNumeric)
                        //    resultContext["spins"]["bs_v"][i][j] = value * _currencyInfo[currency].Rate;
                        row.Add(value);
                    }
                    bsVArray.Add(row);
                }

                for (int i = 0; i < bsVArray.Count; i++)
                {
                    for (int j = 0; j < bsVArray[i].Count; j++)
                    {
                        if(bsVArray[i][j] != 0)
                        resultContext["spins"]["bs_v"][i][j] = bsVArray[i][j] * new Currencies()._currencyInfo[currency].Rate;
                    }
                }

            }

            return JsonConvert.SerializeObject(resultContext);
        }

        protected virtual string convertSettingString(int currency)
        {
            dynamic settingContext = JsonConvert.DeserializeObject<dynamic>(SettingString);
            if (!object.ReferenceEquals(settingContext["bets"], null))
            {
                List<int> betButons = new List<int>();
                for (int i = 0; i < settingContext["bets"].Count; i++)
                    betButons.Add(Convert.ToInt32(settingContext["bets"][i]));

                for(int i=0; i < betButons.Count; i++)
                    settingContext["bets"][i] = betButons[i] * new Currencies()._currencyInfo[currency].Rate;
            }

            return JsonConvert.SerializeObject(settingContext);
        }

        protected BNGActionTypes convertStringToAction(string strAction)
        {
            switch(strAction)
            {
                case "spin":
                    return BNGActionTypes.SPIN;
                case "freespin":
                    return BNGActionTypes.FREESPIN;
                case "freespin_stop":
                    return BNGActionTypes.FREESPINSTOP;
                case "freespin_init":
                    return BNGActionTypes.FREESPININIT;
                case "bonus_init":
                    return BNGActionTypes.BONUSINIT;
                case "respin":
                    return BNGActionTypes.RESPIN;
                case "bonus_spins_stop":
                    return BNGActionTypes.BONUSSTOP;
                case "bonus_freespins_stop":
                    return BNGActionTypes.BONUSFREESPINSTOP;

            }
            return BNGActionTypes.NONE;
        }
        protected string convertActionToString(BNGActionTypes action)
        {
            switch(action)
            {
                case BNGActionTypes.SPIN:
                    return "spin";
                case BNGActionTypes.FREESPIN:
                    return "freespin";
                case BNGActionTypes.FREESPININIT:
                    return "freespin_init";
                case BNGActionTypes.FREESPINSTOP:
                    return "freespin_stop";
                case BNGActionTypes.BONUSINIT:
                    return "bonus_init";
                case BNGActionTypes.BONUSSTOP:
                    return "bonus_spins_stop";
                case BNGActionTypes.RESPIN:
                    return "respin";
                case BNGActionTypes.BONUSFREESPINSTOP:
                    return "bonus_freespins_stop";
            }
            return "";
        }
        protected virtual async Task onDoPlay(string strUserID, int agentID, int currency, GITMessage message, UserBonus userBonus, double userBalance, bool isMustLose)
        {
            try
            {
                _isRewardedBonus    = false;
                _bonusSendMessage   = null;
                _rewardedBonusMoney = 0.0;

                string strRequestID = (string) message.Pop();
                string strToken     = (string)message.Pop();
                string strAction    = (string)message.Pop();

                BNGActionTypes action = convertStringToAction(strAction);
                if(action == BNGActionTypes.NONE)
                {
                    _logger.Error("Unknown Action Found in BNG Game {0} ActionName: {1}", GameName, strAction);
                    return;
                }

                string strGlobalUserID = string.Format("{0}_{1}", agentID, strUserID);
                readBetInfoFromMessage(message, strGlobalUserID, action);
                BNGPlayOriginData originData    = new BNGPlayOriginData();
                originData.autogame             = (bool)message.Pop();
                originData.mobile               = (bool)message.Pop();
                originData.portrait             = (bool)message.Pop();
                originData.quickspin            = (bool)message.Pop();
                originData.sound                = (bool)message.Pop();
                originData.quick_spin           = false;
                originData.data                 = new BNGPlayOriginData.Data();
                originData.data.quick_spin      = false;
                await spinGame(strUserID, agentID, currency, userBonus, userBalance, strRequestID, strToken, originData, action);
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occurred in BaseBNGSlotGame::onDoPlay GameID: {0}, {1}", _gameID, ex);
            }
        }
        protected void onDoUndoUserSpin(string strGlobalUserID)
        {
            undoUserResultInfo(strGlobalUserID);
            undoUserBetInfo(strGlobalUserID);
            saveBetResultInfo(strGlobalUserID);
        }
        #endregion

        protected virtual void readBetInfoFromMessage(GITMessage message, string strGlobalUserID, BNGActionTypes action)
        {
            try
            {
                float betPerLine = (float)(double)message.Pop();
                int lineCount    = (int)message.Pop();

                BaseBNGSlotBetInfo oldBetInfo = null;
                if (_dicUserBetInfos.TryGetValue(strGlobalUserID, out oldBetInfo))
                {
                    //만일 유저에게 남은 응답이 존재하는 경우
                    if (oldBetInfo.HasRemainResponse || (action != BNGActionTypes.SPIN))
                        return;

                    if (betPerLine <= 0.0f)
                    {
                        _logger.Error("{0} betInfo.BetPerLine <= 0 in BaseBNGSlotGame::readBetInfoFromMessage {1}", strGlobalUserID, betPerLine);
                        return;
                    }

                    if (lineCount != this.ClientReqLineCount)
                    {
                        _logger.Error("{0} betInfo.LineCount is not matched {1}", strGlobalUserID, lineCount);
                        return;
                    }

                    oldBetInfo.BetPerLine = betPerLine;
                    oldBetInfo.LineCount = lineCount;

                }
                else if(action == BNGActionTypes.SPIN)
                {
                    if (betPerLine <= 0.0f)
                    {
                        _logger.Error("{0} betInfo.BetPerLine <= 0 in BaseBNGSlotGame::readBetInfoFromMessage {1}", strGlobalUserID, betPerLine);
                        return;
                    }
                    if (lineCount != this.ClientReqLineCount)
                    {
                        _logger.Error("{0} betInfo.LineCount is not matched {1}", strGlobalUserID, lineCount);
                        return;
                    }
                    BaseBNGSlotBetInfo betInfo  = new BaseBNGSlotBetInfo();
                    betInfo.BetPerLine       = betPerLine;
                    betInfo.LineCount        = lineCount;
                    _dicUserBetInfos.Add(strGlobalUserID, betInfo);
                }
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occurred in BaseBNGSlotGame::readBetInfoFromMessage {0}", ex);
            }
        }
        protected virtual void undoUserResultInfo(string strGlobalUserID)
        {
            try
            {
                if (!_dicUserLastBackupResultInfos.ContainsKey(strGlobalUserID))
                    return;

                BaseBNGSlotSpinResult lastResult = _dicUserLastBackupResultInfos[strGlobalUserID];
                if (lastResult == null)
                    _dicUserResultInfos.Remove(strGlobalUserID);
                else
                    _dicUserResultInfos[strGlobalUserID] = lastResult;
                _dicUserLastBackupResultInfos.Remove(strGlobalUserID);
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occurred in BaseBNGSlotGame::undoUserResultInfo {0}", ex);
            }
        }
        protected virtual void undoUserBetInfo(string strGlobalUserID)
        {
            try
            {
                if (!_dicUserLastBackupBetInfos.ContainsKey(strGlobalUserID))
                    return;

                byte[] userBetInfoBytes = _dicUserLastBackupBetInfos[strGlobalUserID];
                using (MemoryStream ms = new MemoryStream(userBetInfoBytes))
                {
                    using (BinaryReader binaryReader = new BinaryReader(ms))
                    {
                        BaseBNGSlotBetInfo betInfo = restoreBetInfo(strGlobalUserID, binaryReader);
                        _dicUserBetInfos[strGlobalUserID] = betInfo;
                    }
                }
                _dicUserLastBackupBetInfos.Remove(strGlobalUserID);
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occurred in BaseBNGSlotGame::undoUserBetInfo {0}", ex);
            }
        }

        protected virtual string makeBalanceNotEnoughResult(string strGlobalUserID, int currency, double userBalance, string strToken, string strRequestID, BNGPlayOriginData originData, BaseBNGSlotBetInfo betInfo)
        {
            BNGPlayResponse playResponse = new BNGPlayResponse();
            playResponse.command         = "play";

            if (_dicUserResultInfos.ContainsKey(strGlobalUserID))
                playResponse.context = JsonConvert.DeserializeObject<dynamic>(_dicUserResultInfos[strGlobalUserID].ResultString);
            else
                playResponse.context = JsonConvert.DeserializeObject<dynamic>(convertInitResultString(currency));
            playResponse.modes                  = new List<string>(new string[] { "auto", "play", "freebet" });
            playResponse.origin_data            = originData;
            playResponse.request_id             = strRequestID;
            playResponse.session_id             = strToken;
            playResponse.status                 = new BNGStatus();
            playResponse.status.code            = "FUNDS_EXCEED";
            playResponse.status.type            = "exceed";
            playResponse.user                   = new BNGUser();
            playResponse.user.balance           = userBalance * 100;
            playResponse.user.balance_version   = betInfo.BalanceVersion;
            playResponse.user.currency          = new Currencies()._currencyInfo[currency].CurrencyText;
            playResponse.user.huid              = strGlobalUserID;
            playResponse.user.show_balance      = true;

            betInfo.BalanceVersion++;
            return JsonConvert.SerializeObject(playResponse);
        }
        protected virtual string makeSpinResultString(BaseBNGSlotBetInfo betInfo, BaseBNGSlotSpinResult spinResult, double betMoney, double userBalance, string strUserID, int currency,string strRequestID, string strToken, BNGPlayOriginData originData)
        {

            BNGPlayResponse playResponse = new BNGPlayResponse();
            playResponse.command        = "play";
            playResponse.context        = JsonConvert.DeserializeObject<dynamic>(spinResult.ResultString);
            playResponse.modes          = new List<string>(new string[] { "auto", "play", "freebet" });
            playResponse.origin_data    = originData;
            playResponse.request_id     = strRequestID;
            playResponse.session_id     = strToken;
            playResponse.status         = new BNGStatus();
            playResponse.status.code    = "OK";
            playResponse.user           = new BNGUser();
            playResponse.user.balance   = (userBalance - betMoney + spinResult.TotalWin) * 100;
            playResponse.user.balance_version = betInfo.BalanceVersion;
            playResponse.user.currency  = new Currencies()._currencyInfo[currency].CurrencyText;
            playResponse.user.huid      = strUserID;
            playResponse.user.show_balance = true;
            betInfo.BalanceVersion++;
            return JsonConvert.SerializeObject(playResponse);
        }
       
        protected virtual void convertBetsByBet(dynamic resultContext, float betPerLine, float totalBet)
        {
            string strCurrent = resultContext["current"];
            List<string> strContexts = new List<string>();
            strContexts.Add(strCurrent);
            if (strCurrent != "spins")
                strContexts.Add("spins");

            foreach (var strContext in strContexts)
            {
                if (object.ReferenceEquals(resultContext[strContext], null))
                    continue;

                dynamic spinContext = resultContext[strContext];
            if (!object.ReferenceEquals(spinContext["bet_per_line"], null))
                spinContext["bet_per_line"] = convertWinByBet((double)spinContext["bet_per_line"], totalBet);

            if (!object.ReferenceEquals(spinContext["round_bet"], null))
                spinContext["round_bet"] = convertWinByBet((double)spinContext["round_bet"], totalBet);
        }
        }
        protected double convertWinByBet(double win, double currentBet)
        {
            return Math.Round(win / _spinDataDefaultBet * currentBet * 100.0, 2);
        }
        protected virtual void convertWinsByBet(dynamic resultContext, float currentBet)
        {
            string  strCurrent  = resultContext["current"];
            List<string> strContexts = new List<string>();
            strContexts.Add(strCurrent);
            if (strCurrent != "spins")
                strContexts.Add("spins");

            foreach(var strContext in strContexts)
            {
                if (object.ReferenceEquals(resultContext[strContext], null))
                    continue;

                dynamic spinContext = resultContext[strContext];
                if (!object.ReferenceEquals(spinContext["round_win"], null) && spinContext["round_win"] != null)
                spinContext["round_win"] = convertWinByBet((double)spinContext["round_win"], currentBet);

                if (!object.ReferenceEquals(spinContext["total_win"], null) && spinContext["total_win"] != null)
                spinContext["total_win"] = convertWinByBet((double)spinContext["total_win"], currentBet);

            if (!object.ReferenceEquals(spinContext["winlines"], null))
            {
                JArray winLineArray = spinContext["winlines"] as JArray;
                for(int i = 0; i < winLineArray.Count; i++)
                {
                    if (!object.ReferenceEquals(winLineArray[i]["amount"], null))
                        winLineArray[i]["amount"] = convertWinByBet((double)winLineArray[i]["amount"], currentBet);
                }
            }
            if (!object.ReferenceEquals(spinContext["winscatters"], null))
            {
                JArray winScatterArray = spinContext["winscatters"] as JArray;
                for (int i = 0; i < winScatterArray.Count; i++)
                {
                    if (!object.ReferenceEquals(winScatterArray[i]["amount"], null))
                        winScatterArray[i]["amount"] = convertWinByBet((double)winScatterArray[i]["amount"], currentBet);
                }
            }

                if (!object.ReferenceEquals(spinContext["boost_pay"], null))
                    spinContext["boost_pay"] = convertWinByBet((double)spinContext["boost_pay"], currentBet);

                if (!object.ReferenceEquals(spinContext["boost_win"], null))
                    spinContext["boost_win"] = convertWinByBet((double)spinContext["boost_win"], currentBet);

                if (!object.ReferenceEquals(spinContext["grand_jackpot"], null))
                    spinContext["grand_jackpot"] = convertWinByBet((double)spinContext["grand_jackpot"], currentBet);
                
                if (!object.ReferenceEquals(spinContext["bs_v"], null))
                {
                    JArray bsVArray = spinContext["bs_v"] as JArray;
                    for (int i = 0; i < bsVArray.Count; i++)
                    {
                        if (!object.ReferenceEquals(bsVArray[i], null))
                        {
                            JArray bsVInArray = bsVArray[i] as JArray;
                            for (int j = 0; j < bsVInArray.Count; j++)
                            {
                                double dBonusWin = 0.0;
                                if (double.TryParse(bsVInArray[j].ToString(), out dBonusWin))
                                    bsVInArray[j] = convertWinByBet(dBonusWin, currentBet);
                            }
                        }
                    }
                }
            }

        }
        protected virtual BaseBNGSlotSpinResult calculateResult(string strGlobalUserID, int currency, BaseBNGSlotBetInfo betInfo, string strSpinResponse, bool isFirst, BNGActionTypes action)
        {
            try
            {
                BaseBNGSlotSpinResult spinResult     = new BaseBNGSlotSpinResult();
                dynamic               resultContext  = JsonConvert.DeserializeObject<dynamic>(strSpinResponse);

                //모든 당첨값들을 현재의 베팅금액상태로 전환한다.
                convertWinsByBet(resultContext, betInfo.TotalBet);
                convertBetsByBet(resultContext, betInfo.BetPerLine, betInfo.TotalBet);

                string  strCurrent  = resultContext["current"];
                dynamic spinContext = resultContext[strCurrent];

                string strNextAction  = (string) resultContext["actions"][0];
                spinResult.NextAction = convertStringToAction(strNextAction);

                double roundWin = 0.0;
                if (!object.ReferenceEquals(spinContext["round_win"], null) && spinContext["round_win"] != null)
                    roundWin = (double)spinContext["round_win"];

                double totalWin             = (double)spinContext["total_win"];
                spinResult.TotalWin         =  roundWin / 100.0;
                spinResult.LastWin          = 0.0;
                spinResult.TransactionID    = Guid.NewGuid().ToString().Replace("-", "") + "1";

                if (action == BNGActionTypes.SPIN || !_dicUserResultInfos.ContainsKey(strGlobalUserID))
                    spinResult.RoundID = ((long)DateTime.Now.Subtract(new DateTime(1970, 1, 1)).TotalMilliseconds).ToString() + "001";
                else
                    spinResult.RoundID = _dicUserResultInfos[strGlobalUserID].RoundID;

                if (action == BNGActionTypes.FREESPINSTOP && totalWin > 0.0)
                    spinResult.LastWin = totalWin;
                else if (roundWin > 0.0)
                    spinResult.LastWin = roundWin;
                else if (_dicUserResultInfos.ContainsKey(strGlobalUserID))
                    spinResult.LastWin = _dicUserResultInfos[strGlobalUserID].LastWin;

                resultContext["last_win"]       = spinResult.LastWin;
                resultContext["last_action"]    = convertActionToString(action);
                if(action == BNGActionTypes.SPIN)
                {
                    resultContext["last_args"] = new JObject();
                    resultContext["last_args"]["bet_per_line"] = betInfo.BetPerLine;
                    resultContext["last_args"]["lines"]        = betInfo.LineCount;
                }
                else
                {
                    resultContext["last_args"] = new JObject();
                }
                resultContext["math_version"] = "a";
                resultContext["version"] = 1;
                if (spinResult.NextAction == BNGActionTypes.SPIN)
                    resultContext["round_finished"] = true;
                else
                    resultContext["round_finished"] = false;

                spinResult.ResultString = JsonConvert.SerializeObject(resultContext);
                return spinResult;
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occurred in BaseBNGSlotGame::calculateResult {0}", ex);
                return null;
            }
        }
        protected List<BaseBNGActionToResponse> buildResponseList(List<string> responseList)
        {
            List<BaseBNGActionToResponse> actionResponseList = new List<BaseBNGActionToResponse>();
            for (int i = 1; i < responseList.Count; i++)
            {
                dynamic         resultContext  = JsonConvert.DeserializeObject<dynamic>(responseList[i - 1]);
                BNGActionTypes  actionType     = convertStringToAction((string) resultContext["actions"][0]);
                actionResponseList.Add(new BaseBNGActionToResponse(actionType, responseList[i]));
            }
            return actionResponseList;
        }
        protected virtual async Task<BaseBNGSlotSpinResult> generateSpinResult(BaseBNGSlotBetInfo betInfo, string strUserID, int agentID,int currency ,UserBonus userBonus, bool usePayLimit, BNGActionTypes action)
        {
            BasePPSlotSpinData      spinData    = null;
            BaseBNGSlotSpinResult   result      = null;

            string strGlobalUserID = string.Format("{0}_{1}", agentID, strUserID);
            
            if (betInfo.HasRemainResponse)
            {
                BaseBNGActionToResponse nextResponse = betInfo.pullRemainResponse();
                result = calculateResult(strGlobalUserID, currency, betInfo, nextResponse.Response, false, action);

                //프리게임이 끝났는지를 검사한다.
                if (!betInfo.HasRemainResponse)
                    betInfo.RemainReponses = null;
                return result;
            }

            //유저의 총 베팅액을 얻는다.
            float   totalBet     = betInfo.TotalBet;
            double  realBetMoney = totalBet;

            spinData = await selectRandomStop(agentID, userBonus, totalBet, betInfo, action);

            //첫자료를 가지고 결과를 계산한다.
            double totalWin = totalBet * spinData.SpinOdd;

            if (!usePayLimit || spinData.IsEvent || checkCompanyPayoutRate(agentID, realBetMoney, totalWin))
            {
                do
                {
                    if (spinData.IsEvent)
                    {
                        bool checkRet = await subtractEventMoney(agentID, strUserID, totalWin);
                        if (!checkRet)
                            break;

                        _bonusSendMessage   = null;
                        _rewardedBonusMoney = totalWin;
                        _isRewardedBonus    = true;
                    }

                    result = calculateResult(strGlobalUserID,currency, betInfo, spinData.SpinStrings[0], true, action);
                    if (spinData.SpinStrings.Count > 1)
                        betInfo.RemainReponses = buildResponseList(spinData.SpinStrings);
                    return result;
                }
                while (false);
            }

            double emptyWin = 0.0;
            if (SupportPurchaseFree && betInfo.PurchaseFree)
            {
                spinData = await selectMinStartFreeSpinData(betInfo);
                result = calculateResult(strGlobalUserID,currency, betInfo, spinData.SpinStrings[0], true, action);
                emptyWin = totalBet * spinData.SpinOdd;

                //뒤에 응답자료가 또 있다면
                if (spinData.SpinStrings.Count > 1)
                    betInfo.RemainReponses = buildResponseList(spinData.SpinStrings);
            }
            else
            {
                spinData = await selectEmptySpin(agentID, betInfo);
                result = calculateResult(strGlobalUserID,currency, betInfo, spinData.SpinStrings[0], true, action);
            }
            sumUpCompanyBetWin(agentID, realBetMoney, emptyWin);
            return result;
        }
        protected byte[] backupBetInfo(BaseBNGSlotBetInfo betInfo)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                using (BinaryWriter writer = new BinaryWriter(ms))
                {
                    betInfo.SerializeTo(writer);
                }
                return ms.ToArray();
            }
        }
        protected virtual async Task spinGame(string strUserID, int agentID, int currency, UserBonus userBonus, double userBalance, string strRequestID, string strToken, BNGPlayOriginData originData, BNGActionTypes action)
        {
            try
            {
                string strGlobalUserID = string.Format("{0}_{1}", agentID, strUserID);
                //해당 유저의 베팅정보를 얻는다. 만일 베팅정보가 없다면(례외상황) 그대로 리턴한다.
                BaseBNGSlotBetInfo betInfo = null;
                if (!_dicUserBetInfos.TryGetValue(strGlobalUserID, out betInfo))
                    return;

                byte[] betInfoBytes = backupBetInfo(betInfo);

                BaseBNGSlotSpinResult lastResult = null;
                if (_dicUserResultInfos.ContainsKey(strGlobalUserID))
                    lastResult = _dicUserResultInfos[strGlobalUserID];

                double betMoney = betInfo.TotalBet;
                if (betInfo.HasRemainResponse)
                    betMoney = 0.0;

                UserBetTypes betType = UserBetTypes.Normal;
                //만일 베팅머니가 유저의 밸런스보다 크다면 끝낸다.(2020.02.15)
                if (userBalance.LT(betMoney, _epsilion) || betMoney < 0.0)
                {
                    string strBalanceErrorResult = makeBalanceNotEnoughResult(strGlobalUserID,currency, userBalance, strToken, strRequestID, originData, betInfo);
                    GITMessage message = new GITMessage((ushort)SCMSG_CODE.SC_BNG_DOPLAY);
                    message.Append(strBalanceErrorResult);
                    Sender.Tell(new ToUserMessage((int)_gameID, message));
                    saveBetResultInfo(strGlobalUserID);
                    return;
                }

                //결과를 생성한다.
                BaseBNGSlotSpinResult spinResult = await this.generateSpinResult(betInfo, strUserID, agentID,currency ,userBonus, true, action);

                //게임로그
                string strGameLog = spinResult.ResultString;
                _dicUserResultInfos[strGlobalUserID] = spinResult;

                //결과를 보내기전에 베팅정보를 디비에 보관한다.(2018.06.12)
                saveBetResultInfo(strGlobalUserID);

                //생성된 게임결과를 유저에게 보낸다.
                sendGameResult(betInfo, spinResult, strGlobalUserID, currency, betMoney, spinResult.WinMoney, strGameLog, userBalance, strRequestID, strToken, originData, betType);

                _dicUserLastBackupBetInfos[strGlobalUserID]     = betInfoBytes;
                _dicUserLastBackupResultInfos[strGlobalUserID]  = lastResult;

                //게임결과를 디비에 보관한다.
                saveResultToHistory(spinResult,agentID ,strUserID, currency,userBalance, betMoney, spinResult.WinMoney, action);
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occurred in BaseBNGSlotGame::spinGame {0}", ex);
            }
        }
        protected virtual void saveResultToHistory(BaseBNGSlotSpinResult result,int agentID ,string strUserID, int currency, double balance, double betMoney, double winMoney, BNGActionTypes action)
        {
            try
            {
                BNGHistoryItem historyItem = new BNGHistoryItem();
                historyItem.AgentID         = agentID;
                historyItem.UserID          = strUserID;
                historyItem.Bet             = betMoney;
                historyItem.Win             = winMoney;
                historyItem.TransactionID   = result.TransactionID;
                historyItem.RoundID         = result.RoundID;
                historyItem.Time            = DateTime.UtcNow;
                historyItem.GameID          = (int) _gameID;

                BNGHistoryItemOverview overview = new BNGHistoryItemOverview();
                overview.balance_before         = Math.Round(balance, 2).ToString();
                overview.balance_after          = Math.Round(balance - betMoney + winMoney, 2).ToString();
                if(betMoney == 0.0)
                    overview.bet = null;
                else
                    overview.bet = Math.Round(betMoney, 2).ToString();
                overview.win                    = Math.Round(winMoney, 2).ToString();
                overview.brand                  = "*";
                overview.c_at                   = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss");
                overview.round_id               = result.RoundID;
                overview.transaction_id         = result.TransactionID;
                overview.currency               = new Currencies()._currencyInfo[currency].CurrencyText;
                overview.exceed_code            = "";
                overview.exceed_message         = "";
                overview.mode                   = "REAL";
                overview.tag                    = "";
                overview.status                 = "OK";
                overview.type                   = "COMMIT";
                overview.player_id              = strUserID;
                overview.game_id                = ((int) _gameID).ToString();
                overview.game_name              = this.SymbolName;
                overview.is_bonus               = false;
                overview.outcome                = Math.Round(winMoney - betMoney, 2).ToString();
                overview.profit                 = Math.Round(betMoney - winMoney, 2).ToString();
                
                if (action == BNGActionTypes.SPIN)
                    overview.round_started = true;
                else
                    overview.round_started = false;
                if (result.NextAction == BNGActionTypes.SPIN)
                    overview.round_finished = true;
                else
                    overview.round_finished = false;

                historyItem.Overview = JsonConvert.SerializeObject(overview);

                dynamic context     = JsonConvert.DeserializeObject<dynamic>(result.ResultString);
                string  strCurrent  = (string) context["current"];
                dynamic newContext  = context[strCurrent];

                JObject detailObject = new JObject();
                detailObject["big_win"]         = false;
                detailObject["last_action"]     = context["last_action"];
                detailObject["math_version"]    = "a";
                detailObject["round_bet"]       = newContext["round_bet"];
                detailObject["round_win"]       = newContext["round_win"];
                detailObject["round_finished"]  = overview.round_finished;
                detailObject["round_started"]   = overview.round_started;
                detailObject["state"]           = strCurrent;
                detailObject["version"]         = 4;
                detailObject["context"]         = newContext;
                historyItem.Detail              = JsonConvert.SerializeObject(detailObject);

                _dbWriter.Tell(historyItem);
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occurred in BaseBNGSlotGame::saveResultToHistory {0}", ex);
            }
        }
        protected virtual void sendGameResult(BaseBNGSlotBetInfo betInfo, BaseBNGSlotSpinResult spinResult, string strGlobalUserID,int currency, double betMoney, double winMoney, string strGameLog, double userBalance, string strRequestID, string strToken, BNGPlayOriginData originData,UserBetTypes betType)
        {
            string strSpinResult = makeSpinResultString(betInfo, spinResult, betMoney, userBalance, strGlobalUserID, currency, strRequestID, strToken, originData);
            GITMessage message = new GITMessage((ushort)SCMSG_CODE.SC_BNG_DOPLAY);
            message.Append(strSpinResult);

            ToUserResultMessage toUserResult = new ToUserResultMessage((int)_gameID, message, betMoney, winMoney, new GameLogInfo(GameName, "0", strGameLog),betType);
            if (_isRewardedBonus)
            {
                toUserResult.setBonusReward(_rewardedBonusMoney);
                toUserResult.insertFirstMessage(_bonusSendMessage);
            }
            Sender.Tell(toUserResult, Self);
        }

        #region 스핀자료처리부분
        protected OddAndIDData selectOddAndIDFromProbsWithRange(SortedDictionary<double, int[]> oddProbs, int totalCount, double minOdd, double maxOdd)
        {
            int random = Pcg.Default.Next(0, totalCount);
            int sum = 0;
            foreach (KeyValuePair<double, int[]> pair in oddProbs)
            {
                if (pair.Key < minOdd)
                    continue;

                if (pair.Key > maxOdd)
                    break;

                sum += pair.Value.Length;
                if (random < sum)
                {
                    OddAndIDData ret = new OddAndIDData();
                    ret.ID = pair.Value[Pcg.Default.Next(0, pair.Value.Length)];
                    ret.Odd = pair.Key;
                    return ret;
                }
            }
            return null;
        }
        protected OddAndIDData selectOddAndIDFromProbsWithRange(SortedDictionary<double, int[]> oddProbs, double minOdd, double maxOdd)
        {
            int totalCount = 0;
            foreach (KeyValuePair<double, int[]> pair in oddProbs)
            {
                if (pair.Key < minOdd)
                    continue;
                if (pair.Key > maxOdd)
                    break;
                totalCount += pair.Value.Length;
            }

            if (totalCount == 0)
                return null;

            int random = Pcg.Default.Next(0, totalCount);
            int sum = 0;
            foreach (KeyValuePair<double, int[]> pair in oddProbs)
            {
                if (pair.Key < minOdd)
                    continue;

                if (pair.Key > maxOdd)
                    break;

                sum += pair.Value.Length;
                if (random < sum)
                {
                    OddAndIDData ret = new OddAndIDData();
                    ret.ID = pair.Value[Pcg.Default.Next(0, pair.Value.Length)];
                    ret.Odd = pair.Key;
                    return ret;
                }
            }
            return null;
        }
        protected OddAndIDData selectOddAndIDFromProbs(SortedDictionary<double, int[]> oddProbs, int totalCount)
        {
            int random  = Pcg.Default.Next(0, totalCount);
            int sum     = 0;
            foreach (KeyValuePair<double, int[]> pair in oddProbs)
            {
                sum += pair.Value.Length;
                if (random < sum)
                {
                    OddAndIDData ret = new OddAndIDData();
                    ret.ID  = pair.Value[Pcg.Default.Next(0, pair.Value.Length)];
                    ret.Odd = pair.Key;
                    return ret;
                }
            }
            return null;
        }
        protected double selectOddFromProbs(SortedDictionary<double, int> oddProbs, int totalCount)
        {
            int random  = Pcg.Default.Next(0, totalCount);
            int sum     = 0;
            foreach (KeyValuePair<double, int> pair in oddProbs)
            {
                sum += pair.Value;
                if (random < sum)
                    return pair.Key;
            }
            return oddProbs.First().Key;
        }
        protected virtual async Task<BasePPSlotSpinData> selectRandomStop(int agentID, BaseBNGSlotBetInfo betInfo, bool isMoreBet)
        {
            OddAndIDData selectedOddAndID = selectRandomOddAndID(agentID, betInfo, isMoreBet);
            return await _spinDatabase.Ask<BasePPSlotSpinData>(new SelectSpinDataByIDRequest(selectedOddAndID.ID), TimeSpan.FromSeconds(10.0));
        }
        protected virtual OddAndIDData selectRandomOddAndID(int agentID, BaseBNGSlotBetInfo betInfo, bool isMoreBet)
        {
            double payoutRate = getPayoutRate(agentID);
            
            double randomDouble = Pcg.Default.NextDouble(0.0, 100.0);
            double selectedOdd = 0.0;
            if (randomDouble >= payoutRate || payoutRate == 0.0)
            {
                selectedOdd = 0.0;
            }
            else if (SupportMoreBet && isMoreBet)
            {
                int random  = Pcg.Default.Next(0, _naturalSpinCount - _anteBetMinusZeroCount);
                int sum     = 0;
                selectedOdd = _naturalSpinOddProbs.Keys.First();
                foreach (KeyValuePair<double, int> pair in _naturalSpinOddProbs)
                {
                    if (pair.Key == 0.0)
                        sum += (pair.Value - _anteBetMinusZeroCount);
                    else
                        sum += pair.Value;
                    if (random < sum)
                    {
                        selectedOdd = pair.Key;
                        break;
                    }
                }
            }
            else
            {
                selectedOdd = selectOddFromProbs(_naturalSpinOddProbs, _naturalSpinCount);
            }

            if (!_totalSpinOddIds.ContainsKey(selectedOdd))
                return null;

            int selectedID = _totalSpinOddIds[selectedOdd][Pcg.Default.Next(0, _totalSpinOddIds[selectedOdd].Length)];
            OddAndIDData selectedOddAndID = new OddAndIDData();
            selectedOddAndID.ID     = selectedID;
            selectedOddAndID.Odd    = selectedOdd;
            return selectedOddAndID;
        }

        protected int pickNaturalIDFromTotalOddIds(double selectedOdd)
        {
            int maxCount = 0;
            for (int i = 0; i < _totalSpinOddIds[selectedOdd].Length; i++)
            {
                if (_totalSpinOddIds[selectedOdd][i] > _normalMaxID)
                    break;
                maxCount++;
            }
            return _totalSpinOddIds[selectedOdd][Pcg.Default.Next(0, maxCount)];
        }
        protected double getBestMatchOdd(double maxOdd)
        {
            maxOdd = Math.Round(maxOdd, 2);
            if (_totalSpinOddIds.ContainsKey(maxOdd))
                return maxOdd;

            double bestMatchedOdd = 0.0;    //제일 작은 오드값으로 초기화한다.
            foreach (KeyValuePair<double, int[]> pair in _totalSpinOddIds)
            {
                if (maxOdd < pair.Key)
                    break;
                bestMatchedOdd = pair.Key;
            }
            return bestMatchedOdd;
        }
        protected virtual async Task<BasePPSlotSpinData> selectEmptySpin(int agentID, BaseBNGSlotBetInfo betInfo)
        {
            int id = _emptySpinIDs[Pcg.Default.Next(0, _emptySpinIDs.Count)];
            return await _spinDatabase.Ask<BasePPSlotSpinData>(new SelectSpinDataByIDRequest(id), TimeSpan.FromSeconds(10.0));
        }
        protected virtual async Task<BasePPSlotSpinData> selectPurchaseFreeSpin(int agentID, BaseBNGSlotBetInfo betInfo, double baseBet, UserBonus userBonus)
        {
            if (userBonus != null && userBonus is UserRangeOddEventBonus)
            {
                UserRangeOddEventBonus rangeOddBonus = userBonus as UserRangeOddEventBonus;
                OddAndIDData oddAndID = selectOddAndIDFromProbsWithRange(_totalFreeSpinOddIds, rangeOddBonus.MinOdd, rangeOddBonus.MaxOdd);
                if (oddAndID != null)
                {
                    BasePPSlotSpinData spinDataEvent = await _spinDatabase.Ask<BasePPSlotSpinData>(new SelectSpinDataByIDRequest(oddAndID.ID), TimeSpan.FromSeconds(10.0));
                    spinDataEvent.IsEvent = true;
                    return spinDataEvent;
                }
            }

            double payoutRate = _config.PayoutRate;
            if (_agentPayoutRates.ContainsKey(agentID))
                payoutRate = _agentPayoutRates[agentID];

            double targetC = PurchaseFreeMultiple * payoutRate / 100.0;
            if (targetC >= _totalFreeSpinWinRate)
                targetC = _totalFreeSpinWinRate;

            if (targetC < _minFreeSpinWinRate)
                targetC = _minFreeSpinWinRate;

            double x = (_totalFreeSpinWinRate - targetC) / (_totalFreeSpinWinRate - _minFreeSpinWinRate);
            double y = 1.0 - x;

            BasePPSlotSpinData spinData = null;
            if (Pcg.Default.NextDouble(0.0, 1.0) <= x)
                spinData = await selectMinStartFreeSpinData(betInfo);
            else
                spinData = await selectRandomStartFreeSpinData(betInfo);
            return spinData;
        }

        protected virtual async Task<BasePPSlotSpinData> selectRandomStartFreeSpinData(BaseBNGSlotBetInfo betInfo)
        {
            try
            {
                OddAndIDData oddAndID = selectOddAndIDFromProbs(_totalFreeSpinOddIds, _freeSpinTotalCount);
                return await _spinDatabase.Ask<BasePPSlotSpinData>(new SelectSpinDataByIDRequest(oddAndID.ID), TimeSpan.FromSeconds(10.0));
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occurred in BaseBNGSlotGame::selectRandomStartFreeSpinData {0}", ex);
                return null;
            }
        }
        protected virtual async Task<BasePPSlotSpinData> selectMinStartFreeSpinData(BaseBNGSlotBetInfo betInfo)
        {
            try
            {
                OddAndIDData oddAndID = selectOddAndIDFromProbsWithRange(_totalFreeSpinOddIds, _minFreeSpinTotalCount, PurchaseFreeMultiple * 0.2, PurchaseFreeMultiple * 0.5);
                return await _spinDatabase.Ask<BasePPSlotSpinData>(new SelectSpinDataByIDRequest(oddAndID.ID), TimeSpan.FromSeconds(10.0));
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occurred in BaseBNGSlotGame::selectMinStartFreeSpinData {0}", ex);
                return null;
            }
        }
        #endregion

        public virtual async Task<BasePPSlotSpinData> selectRandomStop(int agentID, UserBonus userBonus, double baseBet, BaseBNGSlotBetInfo betInfo, BNGActionTypes action)
        {

            //프리스핀구입을 먼저 처리한다.
            if (this.SupportPurchaseFree && action == BNGActionTypes.BUYSPIN)
                return await selectPurchaseFreeSpin(agentID, betInfo, baseBet, userBonus);

            if (userBonus != null && userBonus is UserRangeOddEventBonus)
            {
                UserRangeOddEventBonus rangeOddBonus = userBonus as UserRangeOddEventBonus;
                OddAndIDData oddAndID = selectOddAndIDFromProbsWithRange(_totalSpinOddIds, rangeOddBonus.MinOdd, rangeOddBonus.MaxOdd);
                if (oddAndID != null)
                {
                    BasePPSlotSpinData spinDataEvent = await _spinDatabase.Ask<BasePPSlotSpinData>(new SelectSpinDataByIDRequest(oddAndID.ID), TimeSpan.FromSeconds(10.0));
                    spinDataEvent.IsEvent = true;
                    return spinDataEvent;
                }
            }

            if (SupportMoreBet && betInfo.MoreBet)
                return await selectRandomStop(agentID, betInfo, true);
            else
                return await selectRandomStop(agentID, betInfo, false);
        }
        
        protected override async Task<bool> loadUserHistoricalData(int agentID, string strUserID, bool isNewEnter)
        {
            try
            {
                string strGlobalUserID  = string.Format("{0}_{1}", agentID, strUserID);
                
                string strKey           = string.Format("{0}_{1}", strGlobalUserID, _gameID);
                byte[] betInfoData = await RedisDatabase.RedisCache.StringGetAsync(strKey);
                if (betInfoData != null)
                {
                    using (var stream = new MemoryStream(betInfoData))
                    {
                        BinaryReader reader = new BinaryReader(stream);
                        BaseBNGSlotBetInfo betInfo = restoreBetInfo(strGlobalUserID, reader);
                        if (betInfo != null)
                            _dicUserBetInfos[strGlobalUserID] = betInfo;
                    }
                }

                strKey = string.Format("{0}_{1}_result", strGlobalUserID, _gameID);
                byte[] resultInfoData = await RedisDatabase.RedisCache.StringGetAsync(strKey);
                if (resultInfoData != null)
                {
                    using (var stream = new MemoryStream(resultInfoData))
                    {
                        BinaryReader reader = new BinaryReader(stream);
                        BaseBNGSlotSpinResult resultInfo = restoreResultInfo(strGlobalUserID, reader);
                        if (resultInfo != null)
                            _dicUserResultInfos[strGlobalUserID] = resultInfo;
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occurred in BaseBNGSlotGame::loadUserHistoricalData {0}", ex);
                return false;
            }
            return await base.loadUserHistoricalData(agentID, strUserID, isNewEnter);
        }
        
        protected virtual BaseBNGSlotBetInfo restoreBetInfo(string strGlobalUserID, BinaryReader reader)
        {
            BaseBNGSlotBetInfo betInfo = new BaseBNGSlotBetInfo();
            betInfo.SerializeFrom(reader);
            return betInfo;
        }
        
        protected virtual BaseBNGSlotSpinResult restoreResultInfo(string strGlobalUserID, BinaryReader reader)
        {
            BaseBNGSlotSpinResult result = new BaseBNGSlotSpinResult();
            result.SerializeFrom(reader);
            return result;
        }
        
        protected virtual void saveBetResultInfo(string strGlobalUserID)
        {
            try
            {
                if (_dicUserBetInfos.ContainsKey(strGlobalUserID))
                {
                    byte[] betInfoBytes = _dicUserBetInfos[strGlobalUserID].convertToByte();
                    _redisWriter.Tell(new UserBetInfoWrite(strGlobalUserID, _gameID, betInfoBytes, false));
                }
                if (_dicUserResultInfos.ContainsKey(strGlobalUserID))
                {
                    byte[] resultInfoBytes = _dicUserResultInfos[strGlobalUserID].convertToByte();
                    _redisWriter.Tell(new UserResultInfoWrite(strGlobalUserID, _gameID, resultInfoBytes, false));
                }
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occurred in BaseBNGSlotGame::saveBetInfo {0}", ex);
            }
        }
        
        protected override async Task onUserExitGame(int agentID, string strUserID, bool userRequested)
        {
            try
            {
                string strGlobalUserID = string.Format("{0}_{1}", agentID, strUserID);
                if (_dicUserBetInfos.ContainsKey(strGlobalUserID))
                {
                    byte[] betInfoBytes = _dicUserBetInfos[strGlobalUserID].convertToByte();
                    await _redisWriter.Ask(new UserBetInfoWrite(strGlobalUserID, _gameID, betInfoBytes, true));
                    _dicUserBetInfos.Remove(strGlobalUserID);
                }
                if (_dicUserResultInfos.ContainsKey(strGlobalUserID))
                {
                    byte[] resultInfoBytes = _dicUserResultInfos[strGlobalUserID].convertToByte();
                    await _redisWriter.Ask(new UserResultInfoWrite(strGlobalUserID, _gameID, resultInfoBytes, true));
                    _dicUserResultInfos.Remove(strGlobalUserID);
                }
                
                _dicUserLastBackupResultInfos.Remove(strGlobalUserID);
                _dicUserLastBackupBetInfos.Remove(strGlobalUserID);
                _dicUserLastBackupHistory.Remove(strGlobalUserID);
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occurred in BaseBNGSlotGame::onUserExitGame GameID:{0} {1}", _gameID, ex);
            }

            await base.onUserExitGame(agentID, strUserID, userRequested);
        }
    }
    public class BNGLoginResponse
    {
        public string       command     { get; set; }
        public List<string> modes       { get; set; }
        public string       request_id  { get; set; }
        public string       session_id  { get; set; }
        public BNGStatus    status      { get; set; }
        public BNGUser      user        { get; set; }
    }
    public class BNGStartResponse
    {
        public string               command     { get; set; }
        public dynamic              context     { get; set; }
        public List<string>         modes       { get; set; }
        public BNGStartOriginData   origin_data { get; set; }
        public string               request_id  { get; set; }
        public string               session_id  { get; set; }
        public dynamic              settings    { get; set; }
        public BNGStatus            status      { get; set; }
        public BNGUser              user        { get; set; }
    }
    public class BNGPlayResponse
    {
        public string               command     { get; set; }
        public dynamic              context     { get; set; }
        public List<string>         modes       { get; set; }
        public BNGPlayOriginData    origin_data { get; set; }
        public string               request_id  { get; set; }
        public string               session_id  { get; set; }
        public BNGStatus            status      { get; set; }
        public BNGUser              user        { get; set; }
    }
    public class BNGStatus
    {
        public string code { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string type { get; set; }
    }
    public class BNGUser
    {
        public double   balance         { get; set; }
        public long     balance_version { get; set; }
        public string   currency        { get; set; }
        public string   huid            { get; set; }
        public bool     show_balance    { get; set; }
    }
    public class BNGPlayOriginData
    {
        public bool autogame    { get; set; }
        public Data data        { get; set; }
        public bool feature     { get; set; }
        public bool mobile      { get; set; }
        public bool portrait    { get; set; }
        public bool quick_spin  { get; set; }
        public bool quickspin   { get; set; }
        public bool sound       { get; set; }
        public class Data
        {
            public bool quick_spin { get; set; }
        }
    }
    public class BNGStartOriginData
    {
        public bool quick_spin  { get; set; }
        public Data data        { get; set; }
        public class Data
        {
            public bool quick_spin { get; set; }
        }
    }
    public class BNGBetParams
    {
        public double   bet_per_line { get; set; }
        public int      lines        { get; set; }
    }
}
